using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Open Vario BLE identification service proxy
    /// </summary>
    class IdentificationService : OpenVarioBleService
    {
        /// <summary>
        /// GUID of the service
        /// </summary>
        public static Guid Guid { get { return new Guid("38df4da7-94f3-44dc-83ad-4e10864fbd44"); } }


        /// <summary>
        /// GUID of the Command characteristic
        /// </summary>
        private static Guid CommandGuid = new Guid("520b42a8-ee29-46ec-9eff-24e732ca0cb5");

        /// <summary>
        /// GUID of the Identification info characteristic
        /// </summary>
        private static Guid IdentificationInfoGuid = new Guid("dea233cc-dabb-4b00-9046-f70a44c1ceda");



        /// <summary>
        /// Open Vario device identification information
        /// </summary>
        public class IdentificationInfo : ICloneable
        {
            /// <summary>
            /// Open Vario GATT version
            /// </summary>
            public string GattVersion { get; set; }

            /// <summary>
            /// Software version
            /// </summary>
            public string SoftwareVersion { get; set; }

            /// <summary>
            /// Software manufacturer name
            /// </summary>
            public string SoftwareManufacturerName { get; set; }

            /// <summary>
            /// Hardware version
            /// </summary>
            public string HardwareVersion { get; set; }

            /// <summary>
            /// Hardware manufacturer name
            /// </summary>
            public string HardwareManufacturerName { get; set; }

            /// <summary>
            /// Hardware serial number
            /// </summary>
            public string HardwareSerialNumber { get; set; }

            /// <summary>
            /// Hardware manufacturing date
            /// </summary>
            public string HardwareManufacturingDate { get; set; }


            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>Copy of the current instance</returns>
            public object Clone()
            {
                IdentificationInfo id_info = new IdentificationInfo();
                id_info.GattVersion = GattVersion;
                id_info.SoftwareVersion = SoftwareVersion;
                id_info.SoftwareManufacturerName = SoftwareManufacturerName;
                id_info.HardwareVersion = HardwareVersion;
                id_info.HardwareManufacturerName = HardwareManufacturerName;
                id_info.HardwareSerialNumber = HardwareSerialNumber;
                id_info.HardwareManufacturingDate = HardwareManufacturingDate;
                return id_info;
            }
        };

        /// <summary>
        /// Open Vario device identification information
        /// </summary>
        private IdentificationInfo _identification_info;




        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ble_device">BLE device proxy to use to communicate with the Open Vario device</param>
        /// <param name="ble_service">BLE service representing the identification service</param>
        public IdentificationService(IBleDevice ble_device, BleGattService ble_service)
            : base(ble_device, ble_service)
        {
            _identification_info = null;
        }

        /// <summary>
        /// Get the Open Vario device identification information
        /// </summary>
        /// <returns>Identification information if the operation succeeded, null otherwise</returns>
        public async Task<IdentificationInfo> GetIdentificationInfo()
        {
            // Check if identification information has already been retrieved
            if (_identification_info == null)
            {
                // List characteristics
                await ListCharacteristics();

                // Look for the command and identification information characteristics
                bool found = true;
                BleGattCharacteristic command_char = null;
                BleGattCharacteristic info_char = null;
                try
                {
                    command_char = BleCharacteristics[CommandGuid];
                    info_char = BleCharacteristics[IdentificationInfoGuid];
                }
                catch (KeyNotFoundException)
                {
                    found = false;
                }
                if (found)
                {
                    // Read identification information
                    bool success = true;
                    BleValue read_val = new BleValue();
                    _identification_info = new IdentificationInfo();
                    success = success && await BleDevice.WriteValueAsync(command_char, new BleValue((byte)0u));
                    success = success && await BleDevice.ReadValueAsync(info_char, read_val);
                    if (success)
                    {
                        _identification_info.GattVersion = read_val.ToString();
                    }
                    success = success && await BleDevice.WriteValueAsync(command_char, new BleValue((byte)1u));
                    success = success && await BleDevice.ReadValueAsync(info_char, read_val);
                    if (success)
                    {
                        _identification_info.SoftwareVersion = read_val.ToString();
                    }
                    success = success && await BleDevice.WriteValueAsync(command_char, new BleValue((byte)2u));
                    success = success && await BleDevice.ReadValueAsync(info_char, read_val);
                    if (success)
                    {
                        _identification_info.SoftwareManufacturerName = read_val.ToString();
                    }
                    success = success && await BleDevice.WriteValueAsync(command_char, new BleValue((byte)3u));
                    success = success && await BleDevice.ReadValueAsync(info_char, read_val);
                    if (success)
                    {
                        _identification_info.HardwareVersion = read_val.ToString();
                    }
                    success = success && await BleDevice.WriteValueAsync(command_char, new BleValue((byte)4u));
                    success = success && await BleDevice.ReadValueAsync(info_char, read_val);
                    if (success)
                    {
                        _identification_info.HardwareManufacturerName = read_val.ToString();
                    }
                    success = success && await BleDevice.WriteValueAsync(command_char, new BleValue((byte)5u));
                    success = success && await BleDevice.ReadValueAsync(info_char, read_val);
                    if (success)
                    {
                        _identification_info.HardwareSerialNumber = read_val.ToString();
                    }
                    success = success && await BleDevice.WriteValueAsync(command_char, new BleValue((byte)6u));
                    success = success && await BleDevice.ReadValueAsync(info_char, read_val);
                    if (success)
                    {
                        _identification_info.HardwareManufacturingDate = read_val.ToString();
                    }
                    if (!success)
                    {
                        _identification_info = null;
                    }
                }
            }

            // Return a copy of the identification information
            IdentificationInfo identification_info = null;
            if (_identification_info != null)
            {
                identification_info = (_identification_info.Clone() as IdentificationInfo);
            }
            return identification_info;
        }

        private void OnNotification(BleGattCharacteristic characteristic, BleValue value)
        {
            string val = value.ToString();
            val += "";
        }




    }
}
