#!/usr/bin/env python3
import os
import ssl
from base64 import b64decode, b64encode
from hashlib import sha256
from time import time
from urllib import parse
from hmac import HMAC
from azure.keyvault.secrets import SecretClient
from azure.identity import DefaultAzureCredential

from paho.mqtt import client as mqtt

def generate_device_sas_token(uri, device, key, policy_name, expiry=3600):
    ttl = time() + expiry
    sign_key = '%s\n%d' % ((parse.quote_plus(uri+'%2Fdevices%2F'+device_id)), int(ttl))
    print(sign_key)
    signature = b64encode(HMAC(b64decode(key), sign_key.encode('utf-8'), sha256).digest())

    raw_token = {
        'sr': uri + '%2Fdevices%2F' + device,
        'sig': signature,
        'se': str(int(ttl))
    }
    if policy_name is not None:
        raw_token['skn'] = policy_name

    return 'SharedAccessSignature ' + parse.urlencode(raw_token)

kv_name = 'kvDeepNet'
kv_uri = f'https://{kv_name}.vault.azure.net'
credentials = DefaultAzureCredential()
secret_client = SecretClient(vault_url=kv_uri, credential=credentials)


device_id = "simDevice"
key = secret_client.get_secret(f'{device_id}PK')
iot_hub_name = "rtpos"
sas_token = generate_device_sas_token(iot_hub_name + '.azure-devices.net',device_id, key.value, None)


def on_connect(client, userdata, flags, rc):
    print("Device connected with result code: " + str(rc))


def on_disconnect(client, userdata, rc):
    print("Device disconnected with result code: " + str(rc))


def on_publish(client, userdata, mid):
    print("Device sent message")


client = mqtt.Client(client_id=device_id, protocol=mqtt.MQTTv311, clean_session=0)

client.on_connect = on_connect
client.on_disconnect = on_disconnect
client.on_publish = on_publish

client.username_pw_set(
    username=iot_hub_name
    + ".azure-devices.net/"
    + device_id
    + "/?api-version=2018-06-30",
    password=sas_token,
)

client.tls_set(
    certfile=None,
    keyfile=None,
    cert_reqs=ssl.CERT_REQUIRED,
    tls_version=ssl.PROTOCOL_TLSv1_2,
    ciphers=None,
)
client.tls_insecure_set(False)
client.reconnect_delay_set(5)
client.connect(iot_hub_name + ".azure-devices.net", port=8883)
message = 'ÆØÅ~#%2Fñ'
client.publish("devices/" + device_id + "/messages/events/", bytes(message, 'utf-8'), qos=1)
client.loop_forever()
