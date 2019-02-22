using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Open Vario BLE altimeter service proxy
    /// </summary>
    class AltimeterService : OpenVarioBleService
    {
        /// <summary>
        /// GUID of the service
        /// </summary>
        public static Guid Guid { get { return new Guid("516c5737-8250-493b-bb95-b2a16f65110e"); } }


        /// <summary>
        /// GUID of the Main Altitude characteristic
        /// </summary>
        private static Guid MainAltiGuid = new Guid("f033de08-eda3-46a2-9918-19e123297152");

        /// <summary>
        /// GUID of the Altitude 1 characteristic
        /// </summary>
        private static Guid Alti1Guid = new Guid("b176dd1b-d98e-4707-b51d-d0e31223f776");

        /// <summary>
        /// GUID of the Altitude 2 characteristic
        /// </summary>
        private static Guid Alti2Guid = new Guid("e4c54ec3-e4b3-43a3-9eb0-9790615f68c3");

        /// <summary>
        /// GUID of the Altitude 3 characteristic
        /// </summary>
        private static Guid Alti3Guid = new Guid("2a0934e3-7127-46c0-90a9-d6a5cbb51fa6");

        /// <summary>
        /// GUID of the Altitude 4 characteristic
        /// </summary>
        private static Guid Alti4Guid = new Guid("80b81b29-791a-4b98-bc76-c6a85539b844");


        /// <summary>
        /// Altitude values computed by the altimeter service
        /// </summary>
        public enum Altitude
        {
            /// <summary>
            /// Main altitude
            /// </summary>
            MainAltitude,
            /// <summary>
            /// Altitude 1
            /// </summary>
            Altitude1,
            /// <summary>
            /// Altitude 2
            /// </summary>
            Altitude2,
            /// <summary>
            /// Altitude 3
            /// </summary>
            Altitude3,
            /// <summary>
            /// Altitude 4
            /// </summary>
            Altitude4
        };

        /// <summary>
        /// Altitude changed event handler
        /// </summary>
        /// <param name="altitude">Altitude which has been updated</param>
        /// <param name="value">New altitude value</param>
        public delegate void AltitudeChangedEventHandler(Altitude altitude, Int16 value);

        /// <summary>
        /// Event triggered on Main Altitude value modification
        /// </summary>
        public event AltitudeChangedEventHandler MainAltitudeChanged;

        /// <summary>
        /// Event triggered on Altitude 1 value modification
        /// </summary>
        public event AltitudeChangedEventHandler Altitude1Changed;

        /// <summary>
        /// Event triggered on Altitude 2 value modification
        /// </summary>
        public event AltitudeChangedEventHandler Altitude2Changed;

        /// <summary>
        /// Event triggered on Altitude 3 value modification
        /// </summary>
        public event AltitudeChangedEventHandler Altitude3Changed;

        /// <summary>
        /// Event triggered on Altitude 4 value modification
        /// </summary>
        public event AltitudeChangedEventHandler Altitude4Changed;




        /// <summary>
        /// List of altitude characteristics
        /// </summary>
        private Dictionary<Altitude, BleGattCharacteristic> _altitude_characteristics;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ble_device">BLE device proxy to use to communicate with the Open Vario device</param>
        /// <param name="ble_service">BLE service representing the altimeter service</param>
        public AltimeterService(IBleDevice ble_device, BleGattService ble_service)
            : base(ble_device, ble_service)
        {
            _altitude_characteristics = new Dictionary<Altitude, BleGattCharacteristic>(10);
        }

        /// <summary>
        /// Start notification on a specified altitude value
        /// </summary>
        /// <param name="altitude">Altitude to start being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StartNotification(Altitude altitude)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic alti_characteristic = _altitude_characteristics[altitude];
                ret = await BleDevice.RegisterValueNotificationAsync(alti_characteristic, OnAltitudeNotification);
            }

            return ret;
        }

        /// <summary>
        /// Stop notification on a specified altitude value
        /// </summary>
        /// <param name="altitude">Altitude to stop being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StopNotification(Altitude altitude)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic alti_characteristic = _altitude_characteristics[altitude];
                ret = await BleDevice.UnregisterValueNotificationAsync(alti_characteristic, OnAltitudeNotification);
            }

            return ret;
        }

        /// <summary>
        /// Modify a specified altitude value
        /// </summary>
        /// <param name="altitude">Altitude to modified</param>
        /// <param name="value">Altitude value</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> WriteAltitude(Altitude altitude, Int16 value)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic alti_characteristic = _altitude_characteristics[altitude];
                ret = await BleDevice.WriteValueAsync(alti_characteristic, new BleValue(value));
            }

            return ret;
        }


        /// <summary>
        /// Result of a ReadAltitude() operation
        /// </summary>
        public class ReadAltitudeResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ReadAltitudeResult()
            {
                Success = false;
                Value = 0;
            }

            /// <summary>
            /// true if the operation succeeded, false otherwise
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// Altitude value
            /// </summary>
            public Int16 Value { get; set; }
        }

        /// <summary>
        /// Read a specified altitude value
        /// </summary>
        /// <param name="altitude">Altitude to read</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<ReadAltitudeResult> ReadAltitude(Altitude altitude)
        {
            ReadAltitudeResult ret = new ReadAltitudeResult();

            // List characteristics
            ret.Success = await InitCharacteristics();
            if (ret.Success)
            {
                // Look for the selected characteristic
                BleValue val = new BleValue();
                BleGattCharacteristic alti_characteristic = _altitude_characteristics[altitude];
                ret.Success = await BleDevice.ReadValueAsync(alti_characteristic, val);
                if (ret.Success)
                {
                    ret.Value = val.ToInt16();
                }
            }

            return ret;
        }

        /// <summary>
        /// Initialize the altitude characteristics list
        /// </summary>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        private async Task<bool> InitCharacteristics()
        {
            bool ret = true;

            // Check if the list has been initialized
            if (_altitude_characteristics.Count == 0)
            {
                // List characteristics
                ret = await ListCharacteristics();
                if (ret)
                {
                    try
                    {
                        _altitude_characteristics[Altitude.MainAltitude] = BleCharacteristics[MainAltiGuid];
                        _altitude_characteristics[Altitude.Altitude1] = BleCharacteristics[Alti1Guid];
                        _altitude_characteristics[Altitude.Altitude2] = BleCharacteristics[Alti2Guid];
                        _altitude_characteristics[Altitude.Altitude3] = BleCharacteristics[Alti3Guid];
                        _altitude_characteristics[Altitude.Altitude4] = BleCharacteristics[Alti4Guid];
                    }
                    catch(KeyNotFoundException)
                    {
                        ret = false;
                        _altitude_characteristics.Clear();
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Called when a altitude value is being notified
        /// </summary>
        /// <param name="characteristic">Characteristic which notified the value</param>
        /// <param name="value">New characteristic value</param>
        private void OnAltitudeNotification(BleGattCharacteristic characteristic, BleValue value)
        {
            if (characteristic.Guid == MainAltiGuid)
            {
                MainAltitudeChanged?.Invoke(Altitude.MainAltitude, value.ToInt16());
            }
            else if(characteristic.Guid == Alti1Guid)
            {
                Altitude1Changed?.Invoke(Altitude.Altitude1, value.ToInt16());
            }
            else if (characteristic.Guid == Alti2Guid)
            {
                Altitude2Changed?.Invoke(Altitude.Altitude2, value.ToInt16());
            }
            else if (characteristic.Guid == Alti3Guid)
            {
                Altitude3Changed?.Invoke(Altitude.Altitude3, value.ToInt16());
            }
            else if (characteristic.Guid == Alti4Guid)
            {
                Altitude4Changed?.Invoke(Altitude.Altitude4, value.ToInt16());
            }
            else
            {}
        }


    }
}
