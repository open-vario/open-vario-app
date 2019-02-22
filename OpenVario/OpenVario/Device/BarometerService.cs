using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Open Vario BLE barometer service proxy
    /// </summary>
    class BarometerService : OpenVarioBleService
    {
        /// <summary>
        /// GUID of the service
        /// </summary>
        public static Guid Guid { get { return new Guid("d29a5ba1-e46c-4e2c-a1b7-05f21091a216"); } }


        /// <summary>
        /// GUID of the Pressure characteristic
        /// </summary>
        private static Guid PressureGuid = new Guid("a59b4f7f-47ec-4515-b561-497209d3e8f2");

        /// <summary>
        /// GUID of the Temperature characteristic
        /// </summary>
        private static Guid TemperatureGuid = new Guid("88db8fd5-8362-429b-bfc8-c74aa6c2de44");
        

        /// <summary>
        /// Values computed by the barometer service
        /// </summary>
        public enum BarometerValue
        {
            /// <summary>
            /// Pressure
            /// </summary>
            Pressure,
            /// <summary>
            /// Temperature
            /// </summary>
            Temperature
        };

        /// <summary>
        /// Pressure changed event handler
        /// </summary>
        /// <param name="value">New pressure value</param>
        public delegate void PressureChangedEventHandler(UInt32 value);

        /// <summary>
        /// Temperature changed event handler
        /// </summary>
        /// <param name="value">New temperature value</param>
        public delegate void TemperatureChangedEventHandler(Int16 value);
        
        /// <summary>
        /// Event triggered on Pressure value modification
        /// </summary>
        public event PressureChangedEventHandler PressureChanged;

        /// <summary>
        /// Event triggered on Temperature value modification
        /// </summary>
        public event TemperatureChangedEventHandler TemperatureChanged;
        

        /// <summary>
        /// List of barometer characteristics
        /// </summary>
        private Dictionary<BarometerValue, BleGattCharacteristic> _barometer_characteristics;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ble_device">BLE device proxy to use to communicate with the Open Vario device</param>
        /// <param name="ble_service">BLE service representing the barometer service</param>
        public BarometerService(IBleDevice ble_device, BleGattService ble_service)
            : base(ble_device, ble_service)
        {
            _barometer_characteristics = new Dictionary<BarometerValue, BleGattCharacteristic>(10);
        }

        /// <summary>
        /// Start notification on a specified barometer value
        /// </summary>
        /// <param name="value">Barometer value to start being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StartNotification(BarometerValue value)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic baro_characteristic = _barometer_characteristics[value];
                ret = await BleDevice.RegisterValueNotificationAsync(baro_characteristic, OnBarometerNotification);
            }

            return ret;
        }

        /// <summary>
        /// Stop notification on a specified barometer value
        /// </summary>
        /// <param name="value">Barometer to stop being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StopNotification(BarometerValue value)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic baro_characteristic = _barometer_characteristics[value];
                ret = await BleDevice.UnregisterValueNotificationAsync(baro_characteristic, OnBarometerNotification);
            }

            return ret;
        }

        /// <summary>
        /// Result of a ReadBarometerValue() operation
        /// </summary>
        public class ReadBarometerValueResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ReadBarometerValueResult()
            {
                Success = false;
                PressureValue = 0;
                TemperatureValue = 0;
            }

            /// <summary>
            /// true if the operation succeeded, false otherwise
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// Pressure value
            /// </summary>
            public UInt32 PressureValue { get; set; }
            /// <summary>
            /// Temperature value
            /// </summary>
            public Int16 TemperatureValue { get; set; }
        }

        /// <summary>
        /// Read a specified barometer value
        /// </summary>
        /// <param name="value">Barometer value to read</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<ReadBarometerValueResult> ReadBarometerValue(BarometerValue value)
        {
            ReadBarometerValueResult ret = new ReadBarometerValueResult();

            // List characteristics
            ret.Success = await InitCharacteristics();
            if (ret.Success)
            {
                // Look for the selected characteristic
                BleValue val = new BleValue();
                BleGattCharacteristic baro_characteristic = _barometer_characteristics[value];
                ret.Success = await BleDevice.ReadValueAsync(baro_characteristic, val);
                if (ret.Success)
                {
                    switch (value)
                    {
                        case BarometerValue.Pressure:
                            {
                                ret.PressureValue = val.ToUInt32();
                                break;
                            }

                        case BarometerValue.Temperature:
                            {
                                ret.TemperatureValue = val.ToInt16();
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
        /// Initialize the barometer characteristics list
        /// </summary>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        private async Task<bool> InitCharacteristics()
        {
            bool ret = true;

            // Check if the list has been initialized
            if (_barometer_characteristics.Count == 0)
            {
                // List characteristics
                ret = await ListCharacteristics();
                if (ret)
                {
                    try
                    {
                        _barometer_characteristics[BarometerValue.Pressure] = BleCharacteristics[PressureGuid];
                        _barometer_characteristics[BarometerValue.Temperature] = BleCharacteristics[TemperatureGuid];
                    }
                    catch(KeyNotFoundException)
                    {
                        ret = false;
                        _barometer_characteristics.Clear();
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Called when a barometer value is being notified
        /// </summary>
        /// <param name="characteristic">Characteristic which notified the value</param>
        /// <param name="value">New characteristic value</param>
        private void OnBarometerNotification(BleGattCharacteristic characteristic, BleValue value)
        {
            if (characteristic.Guid == PressureGuid)
            {
                PressureChanged?.Invoke(value.ToUInt32());
            }
            else if (characteristic.Guid == TemperatureGuid)
            {
                TemperatureChanged?.Invoke(value.ToInt16());
            }
            else
            {}
        }


    }
}
