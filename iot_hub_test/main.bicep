param storage_name string = 'deepblob2'
param storage_sku_name string = 'Standard_LRS'
param container_name string = 'rtdata'

param iot_hub_name string = 'rtpos2'
param iot_hub_sku_capacity int = 2
param iot_hub_sku_name string = 'S1'

param rg_location string = resourceGroup().location

resource message_storage 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storage_name
  location: rg_location
  kind: 'StorageV2'
  sku: {
    name: storage_sku_name
  }
}

var storage_connection_string = 'DefaultEndpointsProtocol=https;AccountName=${message_storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(message_storage.id, message_storage.apiVersion).keys[0].value}'

resource message_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${message_storage.name}/default/${container_name}'
  dependsOn: [
    message_storage
  ]
}

resource hub 'Microsoft.Devices/IotHubs@2021-07-01' = {
  name: iot_hub_name
  location: rg_location
  sku: {
    capacity: iot_hub_sku_capacity
    name: iot_hub_sku_name
  }
  properties: {
    routing: {
      endpoints: {
        storageContainers: [
          {
            name: storage_name
            containerName: container_name
            connectionString: storage_connection_string
            fileNameFormat: '{iothub}/{YYYY}/{MM}/{DD}/{HH}/{mm}_{partition}.avro'
          }
        ]
        eventHubs: []
      }
      routes: [
        {
          name: 'StorageRoute'
          source: 'DeviceMessages'
          endpointNames: [
            storage_name
          ]
          isEnabled: true
        }
        {
          name: 'EventHubRoute'
          source: 'DeviceMessages'
          endpointNames: [
            'events'
          ]
          isEnabled: true
        }
      ]
    }
  }
}
