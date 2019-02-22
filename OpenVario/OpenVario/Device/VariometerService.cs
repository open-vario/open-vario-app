using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Open Vario BLE variometer service proxy
    /// </summary>
    class VariometerService : OpenVarioBleService
    {
        /// <summary>
        /// GUID of the service
        /// </summary>
        public static Guid Guid { get { return new Guid("ae283ac8-786f-42ef-b694-b7faf492cae9"); } }


        /// <summary>
        /// GUID of the Vario characteristic
        /// </summary>
        private static Guid VarioGuid = new Guid("7708157c-132f-4d21-a1d9-c9768732b4e9");

        /// <summary>
        /// GUID of the Acceleration characteristic
        /// </summary>
        private static Guid AccelerationGuid = new Guid("9e13b15f-3582-433b-9034-55ac8881ee4f");
        

        /// <summary>
        /// Values computed by the variometer service
        /// </summary>
        public enum VariometerValue
        {
            /// <summary>
            /// Vario
            /// </summary>
            Vario,
            /// <summary>
            /// Acceleration
            /// </summary>
            Acceleration
        };

        /// <summary>
        /// Vario changed event handler
        /// </summary>
        /// <param name="value">New vario value</param>
        public delegate void VarioChangedEventHandler(Int16 value);

        /// <summary>
        /// Acceleration changed event handler
        /// </summary>
        /// <param name="value">New acceleration value</param>
        public delegate void AccelerationChangedEventHandler(Byte value);

        /// <summary>
        /// Event triggered on Vario value modification
        /// </summary>
        public event VarioChangedEventHandler VarioChanged;

        /// <summary>
        /// Event triggered on Acceleration value modification
        /// </summary>
        public event AccelerationChangedEventHandler AccelerationChanged;
        

        /// <summary>
        /// List of variometer characteristics
        /// </summary>
        private Dictionary<VariometerValue, BleGattCharacteristic> _variometer_characteristics;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ble_device">BLE device proxy to use to communicate with the Open Vario device</param>
        /// <param name="ble_service">BLE service representing the variometer service</param>
        public VariometerService(IBleDevice ble_device, BleGattService ble_service)
            : base(ble_device, ble_service)
        {
            _variometer_characteristics = new Dictionary<VariometerValue, BleGattCharacteristic>(10);
        }

        /// <summary>
        /// Start notification on a specified variometer value
        /// </summary>
        /// <param name="value">Variometer value to start being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StartNotification(VariometerValue value)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic vario_characteristic = _variometer_characteristics[value];
                ret = await BleDevice.RegisterValueNotificationAsync(vario_characteristic, OnVariometerNotification);
            }

            return ret;
        }

        /// <summary>
        /// Stop notification on a specified variometer value
        /// </summary>
        /// <param name="value">Variometer to stop being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StopNotification(VariometerValue value)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic vario_characteristic = _variometer_characteristics[value];
                ret = await BleDevice.UnregisterValueNotificationAsync(vario_characteristic, OnVariometerNotification);
            }

            return ret;
        }

        /// <summary>
        /// Result of a ReadVariometerValue() operation
        /// </summary>
        public class ReadVariometerValueResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ReadVariometerValueResult()
            {
                Success = false;
                VarioValue = 0;
                AccelerationValue = 0;
            }

            /// <summary>
            /// true if the operation succeeded, false otherwise
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// Vario value
            /// </summary>
            public Int16 VarioValue { get; set; }
            /// <summary>
            /// Acceleration value
            /// </summary>
            public Byte AccelerationValue { get; set; }
        }

        /// <summary>
        /// Read a specified variometer value
        /// </summary>
        /// <param name="value">Variometer value to read</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<ReadVariometerValueResult> ReadVariometerValue(VariometerValue value)
        {
            ReadVariometerValueResult ret = new ReadVariometerValueResult();

            // List characteristics
            ret.Success = await InitCharacteristics();
            if (ret.Success)
            {
                // Look for the selected characteristic
                BleValue val = new BleValue();
                BleGattCharacteristic vario_characteristic = _variometer_characteristics[value];
                ret.Success = await BleDevice.ReadValueAsync(vario_characteristic, val);
                if (ret.Success)
                {
                    switch (value)
                    {
                        case VariometerValue.Vario:
                            {
                                ret.VarioValue = val.ToInt16();
                                break;
                            }

                        case VariometerValue.Acceleration:
                            {
                                ret.AccelerationValue = val.ToUInt8();
                                break;
                            }

                        default:
                            {
                                ret.Success = false;
                                break;
                            }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Initialize the variometer characteristics list
        /// </summary>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        private async Task<bool> InitCharacteristics()
        {
            bool ret = true;

            // Check if the list has been initialized
            if (_variometer_characteristics.Count == 0)
            {
                // List characteristics
                ret = await ListCharacteristics();
                if (ret)
                {
                    try
                    {
                        _variometer_characteristics[VariometerValue.Vario] = BleCharacteristics[VarioGuid];
                        _variometer_characteristics[VariometerValue.Acceleration] = BleCharacteristics[AccelerationGuid];
                    }
                    catch(KeyNotFoundException)
                    {
                        ret = false;
                        _variometer_characteristics.Clear();
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Called when a variometer value is being notified
        /// </summary>
        /// <param name="characteristic">Characteristic which notified the value</param>
        /// <param name="value">New characteristic value</param>
        private void OnVariometerNotification(BleGattCharacteristic characteristic, BleValue value)
        {
            if (characteristic.Guid == VarioGuid)
            {
                VarioChanged?.Invoke(value.ToInt16());
            }
            else if (characteristic.Guid == AccelerationGuid)
            {
                AccelerationChanged?.Invoke(value.ToUInt8());
            }
            else
            {}
        }


    }
}
