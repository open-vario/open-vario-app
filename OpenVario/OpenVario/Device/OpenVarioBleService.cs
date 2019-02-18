using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Base class for Open Vario BLE service proxies
    /// </summary>
    class OpenVarioBleService
    {

        /// <summary>
        /// BLE device
        /// </summary>
        protected IBleDevice BleDevice { get; private set; }

        /// <summary>
        /// BLE service
        /// </summary>
        protected BleGattService BleService { get; private set; }

        /// <summary>
        /// BLE characteristics
        /// </summary>
        protected IDictionary<Guid, BleGattCharacteristic> BleCharacteristics { get; private set; }




        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ble_device">BLE device proxy to use to communicate with the Open Vario device</param>
        /// <param name="ble_service">BLE service representing the service</param>
        public OpenVarioBleService(IBleDevice ble_device, BleGattService ble_service)
        {
            BleDevice = ble_device;
            BleService = ble_service;
            BleCharacteristics = new Dictionary<Guid, BleGattCharacteristic>(20);
        }
        

        /// <summary>
        /// List all the BLE characteristics contained into the service
        /// </summary>
        public async Task<bool> ListCharacteristics()
        {
            // Check if characteristics have already been retrieved
            if (BleCharacteristics.Count == 0)
            {
                IList<BleGattCharacteristic> ble_characteristics = await BleDevice.GetCharacteristicsAsync(BleService);
                foreach (BleGattCharacteristic ble_characteristic in ble_characteristics)
                {
                    // Add characteristic to the list
                    BleCharacteristics.Add(ble_characteristic.Guid, ble_characteristic);
                }
            }

            return true;
        }



    }
}
