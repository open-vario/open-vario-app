using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Open Vario BLE navigation service proxy
    /// </summary>
    class NavigationService : OpenVarioBleService
    {
        /// <summary>
        /// GUID of the service
        /// </summary>
        public static Guid Guid { get { return new Guid("530b9c7a-3185-49f0-9bb5-8e7b88a9df09"); } }


        /// <summary>
        /// GUID of the Speed characteristic
        /// </summary>
        private static Guid SpeedGuid = new Guid("609a0afe-59a2-4837-b4fe-46d2ddfec0dd");

        /// <summary>
        /// GUID of the Latitude characteristic
        /// </summary>
        private static Guid LatitudeGuid = new Guid("c75a49f0-cfe0-4d93-8109-3dbc588c2243");

        /// <summary>
        /// GUID of the Longitude characteristic
        /// </summary>
        private static Guid LongitudeGuid = new Guid("3692fbda-6a27-4422-a307-5f852658cae0");

        /// <summary>
        /// GUID of the Track Angle characteristic
        /// </summary>
        private static Guid TrackAngleGuid = new Guid("c6502b8c-5aae-489f-bb23-04eabc389f58");


        /// <summary>
        /// Values computed by the navigation service
        /// </summary>
        public enum NavigationValue
        {
            /// <summary>
            /// Speed
            /// </summary>
            Speed,
            /// <summary>
            /// Latitude
            /// </summary>
            Latitude,
            /// <summary>
            /// Longitude
            /// </summary>
            Longitude,
            /// <summary>
            /// Track angle
            /// </summary>
            TrackAngle
        };

        /// <summary>
        /// Speed changed event handler
        /// </summary>
        /// <param name="value">New speed value</param>
        public delegate void SpeedChangedEventHandler(UInt16 value);

        /// <summary>
        /// Latitude changed event handler
        /// </summary>
        /// <param name="value">New latitude value</param>
        public delegate void LatitudeChangedEventHandler(double value);

        /// <summary>
        /// Longitude changed event handler
        /// </summary>
        /// <param name="value">New longitude value</param>
        public delegate void LongitudeChangedEventHandler(double value);

        /// <summary>
        /// Track angle changed event handler
        /// </summary>
        /// <param name="value">New track angle value</param>
        public delegate void TrackAngleChangedEventHandler(UInt16 value);

        /// <summary>
        /// Event triggered on Speed value modification
        /// </summary>
        public event SpeedChangedEventHandler SpeedChanged;

        /// <summary>
        /// Event triggered on Latitude value modification
        /// </summary>
        public event LatitudeChangedEventHandler LatitudeChanged;

        /// <summary>
        /// Event triggered on Longitude value modification
        /// </summary>
        public event LongitudeChangedEventHandler LongitudeChanged;

        /// <summary>
        /// Event triggered on Track Angle value modification
        /// </summary>
        public event TrackAngleChangedEventHandler TrackAngleChanged;
        



        /// <summary>
        /// List of navigation characteristics
        /// </summary>
        private Dictionary<NavigationValue, BleGattCharacteristic> _navigation_characteristics;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ble_device">BLE device proxy to use to communicate with the Open Vario device</param>
        /// <param name="ble_service">BLE service representing the navigation service</param>
        public NavigationService(IBleDevice ble_device, BleGattService ble_service)
            : base(ble_device, ble_service)
        {
            _navigation_characteristics = new Dictionary<NavigationValue, BleGattCharacteristic>(10);
        }

        /// <summary>
        /// Start notification on a specified navigation value
        /// </summary>
        /// <param name="value">Navigation value to start being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StartNotification(NavigationValue value)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic nav_characteristic = _navigation_characteristics[value];
                ret = await BleDevice.RegisterValueNotificationAsync(nav_characteristic, OnNavigationNotification);
            }

            return ret;
        }

        /// <summary>
        /// Stop notification on a specified navigation value
        /// </summary>
        /// <param name="value">Navigation to stop being notified</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> StopNotification(NavigationValue value)
        {
            bool ret;

            // List characteristics
            ret = await InitCharacteristics();
            if (ret)
            {
                // Look for the selected characteristic
                BleGattCharacteristic nav_characteristic = _navigation_characteristics[value];
                ret = await BleDevice.UnregisterValueNotificationAsync(nav_characteristic, OnNavigationNotification);
            }

            return ret;
        }

        /// <summary>
        /// Result of a ReadNavigationValue() operation
        /// </summary>
        public class ReadNavigationValueResult
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ReadNavigationValueResult()
            {
                Success = false;
                SpeedValue = 0;
                LatitudeValue = 0;
                LongitudeValue = 0;
                TrackAngleValue = 0;
            }

            /// <summary>
            /// true if the operation succeeded, false otherwise
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// Speed value
            /// </summary>
            public UInt16 SpeedValue { get; set; }
            /// <summary>
            /// Latitude value
            /// </summary>
            public double LatitudeValue { get; set; }
            /// <summary>
            /// Longitude value
            /// </summary>
            public double LongitudeValue { get; set; }
            /// <summary>
            /// Track angle value
            /// </summary>
            public UInt16 TrackAngleValue { get; set; }
        }

        /// <summary>
        /// Read a specified navigation value
        /// </summary>
        /// <param name="value">Navigation value to read</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<ReadNavigationValueResult> ReadNavigationValue(NavigationValue value)
        {
            ReadNavigationValueResult ret = new ReadNavigationValueResult();

            // List characteristics
            ret.Success = await InitCharacteristics();
            if (ret.Success)
            {
                // Look for the selected characteristic
                BleValue val = new BleValue();
                BleGattCharacteristic nav_characteristic = _navigation_characteristics[value];
                ret.Success = await BleDevice.ReadValueAsync(nav_characteristic, val);
                if (ret.Success)
                {
                    switch (value)
                    {
                        case NavigationValue.Speed:
                            {
                                ret.SpeedValue = val.ToUInt16();
                                break;
                            }

                        case NavigationValue.Latitude:
                            {
                                ret.LatitudeValue = val.ToFloat64();
                                break;
                            }

                        case NavigationValue.Longitude:
                            {
                                ret.LongitudeValue = val.ToFloat64();
                                break;
                            }

                        case NavigationValue.TrackAngle:
                            {
                                ret.TrackAngleValue = val.ToUInt16();
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
        /// Initialize the navigation characteristics list
        /// </summary>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        private async Task<bool> InitCharacteristics()
        {
            bool ret = true;

            // Check if the list has been initialized
            if (_navigation_characteristics.Count == 0)
            {
                // List characteristics
                ret = await ListCharacteristics();
                if (ret)
                {
                    try
                    {
                        _navigation_characteristics[NavigationValue.Speed] = BleCharacteristics[SpeedGuid];
                        _navigation_characteristics[NavigationValue.Latitude] = BleCharacteristics[LatitudeGuid];
                        _navigation_characteristics[NavigationValue.Longitude] = BleCharacteristics[LongitudeGuid];
                        _navigation_characteristics[NavigationValue.TrackAngle] = BleCharacteristics[TrackAngleGuid];
                    }
                    catch(KeyNotFoundException)
                    {
                        ret = false;
                        _navigation_characteristics.Clear();
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Called when a navigation value is being notified
        /// </summary>
        /// <param name="characteristic">Characteristic which notified the value</param>
        /// <param name="value">New characteristic value</param>
        private void OnNavigationNotification(BleGattCharacteristic characteristic, BleValue value)
        {
            if (characteristic.Guid == SpeedGuid)
            {
                SpeedChanged?.Invoke(value.ToUInt16());
            }
            else if(characteristic.Guid == LatitudeGuid)
            {
                LatitudeChanged?.Invoke(value.ToFloat64());
            }
            else if (characteristic.Guid == LongitudeGuid)
            {
                LongitudeChanged?.Invoke(value.ToFloat64());
            }
            else if (characteristic.Guid == TrackAngleGuid)
            {
                TrackAngleChanged?.Invoke(value.ToUInt16());
            }
            else
            {}
        }


    }
}
