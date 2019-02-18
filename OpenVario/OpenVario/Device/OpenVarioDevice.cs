using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Open Vario device proxy
    /// </summary>
    class OpenVarioDevice
    {
        /// <summary>
        /// BLE device proxy to use to communicate with the Open Vario device
        /// </summary>
        private IBleDevice _ble_device;

        /// <summary>
        /// List of all BLE services inside the device
        /// </summary>
        private Dictionary<Guid, BleGattService> _ble_services;


        /// <summary>
        /// Open Vario BLE identification service
        /// </summary>
        public IdentificationService IdentificationService { get; private set; }

        /// <summary>
        /// Open Vario BLE altimeter service
        /// </summary>
        public AltimeterService AltimeterService { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ble_device">BLE device proxy to use to communicate with the Open Vario device</param>
        public OpenVarioDevice(IBleDevice ble_device)
        {
            _ble_device = ble_device;
            _ble_services = new Dictionary<Guid, BleGattService>(10);
        }


        /// <summary>
        /// Initialize the Open Vario device proxy
        /// </summary>
        /// <returns>true if the proxy has been initialized, false otherwise</returns>
        public async Task<bool> Initialize()
        {
            bool ret = true;

            // List all services
            IList<BleGattService> ble_services = await _ble_device.GetServicesAsync();
            foreach (BleGattService ble_service in ble_services)
            {
                // Add service to the list
                _ble_services.Add(ble_service.Guid, ble_service);
            }

            // Look for the identification service
            try
            {
                BleGattService identification_service = _ble_services[IdentificationService.Guid];
                IdentificationService = new IdentificationService(_ble_device, identification_service);
                IdentificationService.IdentificationInfo id_info = await IdentificationService.GetIdentificationInfo();
                if (id_info != null)
                {
                    ret = true;
                }
            }
            catch(KeyNotFoundException)
            {
                ret = false;
            }
            if (ret)
            {
                // Look for the other services
                try
                {
                    BleGattService altimeter_service = _ble_services[AltimeterService.Guid];
                    AltimeterService = new AltimeterService(_ble_device, altimeter_service);
                }
                catch (KeyNotFoundException)
                {
                    ret = false;
                }
            }
                return ret;
        }
    }
}
