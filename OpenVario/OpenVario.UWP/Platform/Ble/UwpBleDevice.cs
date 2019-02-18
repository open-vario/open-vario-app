using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace OpenVario.UWP
{

    /// <summary>
    /// BLE device proxy implementaton for UWP platform
    /// </summary>
    public class UwpBleDevice : IBleDevice
    {
        /// <summary>
        /// Device information
        /// </summary>
        private BleDeviceInformation _ble_device_info;

        /// <summary>
        /// BLE device
        /// </summary>
        private BluetoothLEDevice _ble_device;

        /// <summary>
        /// List of the listener to BLE characteristics notifications
        /// </summary>
        private Dictionary<BleGattCharacteristic, Action<BleGattCharacteristic, BleValue>> _ble_notification_listeners;

        /// <summary>
        /// Lit of BLE characteristics
        /// </summary>
        private Dictionary<GattCharacteristic, BleGattCharacteristic> _ble_characteristics;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device_info"></param>
        public UwpBleDevice(BleDeviceInformation ble_device_info)
        {
            // Save device information
            _ble_device_info = ble_device_info;

            // Initialize listener list
            _ble_notification_listeners = new Dictionary<BleGattCharacteristic, Action<BleGattCharacteristic, BleValue>>(100);
            _ble_characteristics = new Dictionary<GattCharacteristic, BleGattCharacteristic>(100);
        }

        /// <summary>
        /// Initialize the BLE device proxy object
        /// </summary>
        /// <returns>true if the initialization succeeded, false otherwise</returns>
        public async Task<bool> Initialize()
        {
            bool ret = false;

            _ble_device = await BluetoothLEDevice.FromIdAsync(deviceId: _ble_device_info.Id);
            if (_ble_device != null)
            {
                ret = true;
            }
            return ret;
        }



        /// <summary>
        /// Get the BLE device information
        /// </summary>
        public BleDeviceInformation DeviceInformation { get { return _ble_device_info; } }


        /// <summary>
        /// Get the list of the primary GATT services
        /// </summary>
        /// <returns>List of the primary GATT services</returns>
        public async Task<IList<BleGattService>> GetServicesAsync()
        {
            List<BleGattService> services = new List<BleGattService>();

            GattDeviceServicesResult result = await _ble_device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
            foreach (GattDeviceService gatt_service in result.Services)
            {
                BleGattService service = new BleGattService();
                service.Name = "";
                service.Guid = gatt_service.Uuid;
                service.Context = gatt_service;

                services.Add(service);
            }

            return services;
        }

        /// <summary>
        /// Get the list of the GATT services included in a specific GATT service
        /// </summary>
        /// <returns>List of the included GATT services</returns>
        public async Task<IList<BleGattService>> GetServicesAsync(BleGattService service)
        {
            List<BleGattService> services = new List<BleGattService>();

            GattDeviceService gatt_service = service.Context as GattDeviceService;
            GattDeviceServicesResult result = await gatt_service.GetIncludedServicesAsync(BluetoothCacheMode.Uncached);
            foreach (GattDeviceService included_service in result.Services)
            {
                BleGattService ble_service = new BleGattService();
                ble_service.Name = "";
                ble_service.Guid = gatt_service.Uuid;
                ble_service.Context = included_service;

                services.Add(ble_service);
            }

            return services;
        }

        /// <summary>
        /// Get the list of the GATT characteristics included in a specific GATT service
        /// </summary>
        /// <returns>List of the included GATT characteristics</returns>
        public async Task<IList<BleGattCharacteristic>> GetCharacteristicsAsync(BleGattService service)
        {
            List<BleGattCharacteristic> characteristics = new List<BleGattCharacteristic>();

            GattDeviceService gatt_service = service.Context as GattDeviceService;
            GattCharacteristicsResult result = await gatt_service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
            foreach (GattCharacteristic characteristic in result.Characteristics)
            {
                BleGattCharacteristic ble_characteristic = new BleGattCharacteristic();
                ble_characteristic.Name = "";
                ble_characteristic.Guid = characteristic.Uuid;
                ble_characteristic.Context = characteristic;
                ble_characteristic.CanRead = characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read);
                ble_characteristic.CanWrite = ((characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) ||
                                               (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse)));
                ble_characteristic.CanNotify = ((characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) ||
                                                (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate)));

                characteristics.Add(ble_characteristic);
                _ble_characteristics.Add(characteristic, ble_characteristic);
            }

            return characteristics;
        }

        /// <summary>
        /// Get the list of the GATT descriptors included in a specific GATT characteristic
        /// </summary>
        /// <returns>List of the included GATT descriptors</returns>
        public async Task<IList<BleGattDescriptor>> GetDescriptorsAsync(BleGattCharacteristic characteristic)
        {
            List<BleGattDescriptor> descriptors = new List<BleGattDescriptor>();

            GattCharacteristic gatt_characteristic = characteristic.Context as GattCharacteristic;
            GattDescriptorsResult result = await gatt_characteristic.GetDescriptorsAsync(BluetoothCacheMode.Uncached);
            foreach (GattDescriptor descriptor in result.Descriptors)
            {
                BleGattDescriptor ble_descriptor = new BleGattDescriptor();
                ble_descriptor.Name = "";
                ble_descriptor.Guid = descriptor.Uuid;
                ble_descriptor.Context = descriptor;

                descriptors.Add(ble_descriptor);
            }

            return descriptors;
        }

        /// <summary>
        /// Write a value into a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="value">Value</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> WriteValueAsync(BleGattCharacteristic characteristic, BleValue value)
        {
            bool ret = false;

            GattCharacteristic gatt_characteristic = characteristic.Context as GattCharacteristic;
            IBuffer buffer = CryptographicBuffer.CreateFromByteArray(value.Value);
            GattWriteResult result = await gatt_characteristic.WriteValueWithResultAsync(buffer, GattWriteOption.WriteWithResponse);
            if (result.Status == GattCommunicationStatus.Success)
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Read a value from a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="value">Value</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> ReadValueAsync(BleGattCharacteristic characteristic, BleValue value)
        {
            bool ret = false;

            GattCharacteristic gatt_characteristic = characteristic.Context as GattCharacteristic;
            GattReadResult result = await gatt_characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
            if (result.Status == GattCommunicationStatus.Success)
            {
                byte[] val;
                CryptographicBuffer.CopyToByteArray(result.Value, out val);
                value.Value = val;
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Register to notifications from a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="listener">Method which will be called on each value notification</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> RegisterValueNotificationAsync(BleGattCharacteristic characteristic, Action<BleGattCharacteristic, BleValue> listener)
        {
            bool ret = false;

            GattCharacteristic gatt_characteristic = characteristic.Context as GattCharacteristic;
            GattClientCharacteristicConfigurationDescriptorValue value;
            if (gatt_characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                value = GattClientCharacteristicConfigurationDescriptorValue.Notify;
            }
            else if (gatt_characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
            {
                value = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            }
            else
            {
                value = GattClientCharacteristicConfigurationDescriptorValue.None;
            }
            if (value != GattClientCharacteristicConfigurationDescriptorValue.None)
            {
                if (!_ble_notification_listeners.ContainsKey(characteristic))
                {
                    GattWriteResult result = await gatt_characteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(value);
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        lock (_ble_notification_listeners)
                        {
                            _ble_notification_listeners.Add(characteristic, listener);
                        }
                        gatt_characteristic.ValueChanged += OnCharacteristicNotification;
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Unregister from notifications from a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="listener">Method which is called on each value notification</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        public async Task<bool> UnregisterValueNotificationAsync(BleGattCharacteristic characteristic, Action<BleGattCharacteristic, BleValue> listener)
        {
            bool ret = false;

            if (!_ble_notification_listeners.ContainsKey(characteristic))
            {
                GattCharacteristic gatt_characteristic = characteristic.Context as GattCharacteristic;
                await gatt_characteristic.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                lock(_ble_notification_listeners)
                {
                    _ble_notification_listeners.Remove(characteristic);
                }
                gatt_characteristic.ValueChanged -= OnCharacteristicNotification;
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Called on notification on a GATT characteristic
        /// </summary>
        /// <param name="sender">GATT characteristic</param>
        /// <param name="args">Notified value</param>
        private void OnCharacteristicNotification(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            try
            {
                BleGattCharacteristic ble_characteristic = _ble_characteristics[sender];
                IBuffer value = args.CharacteristicValue;
                CryptographicBuffer.CopyToByteArray(value, out byte[] val);

                Action<BleGattCharacteristic, BleValue> listener = null;
                lock (_ble_notification_listeners)
                {
                    listener = _ble_notification_listeners[ble_characteristic];
                }
                listener.Invoke(ble_characteristic, new BleValue(val));
            }
            catch (KeyNotFoundException)
            {}
        }
    }
}
