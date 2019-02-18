using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{

    /// <summary>
    /// BLE GATT attribute
    /// </summary>
    public class BleGattAttribute
    {
        /// <summary>
        /// BLE GATT attribute types
        /// </summary>
        public enum GattType
        {
            /// <summary>
            /// Service
            /// </summary>
            Service,
            /// <summary>
            /// Characteristic
            /// </summary>
            Characteristic,
            /// <summary>
            /// Descriptor
            /// </summary>
            Descriptor
        }

        /// <summary>
        /// Attribute's GUID
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Attribute's name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Attribute's type
        /// </summary>
        public GattType Type { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Attribute's type</param>
        public BleGattAttribute(GattType type)
        {
            Type = type;
        }
    }

    /// <summary>
    /// BLE GATT service
    /// </summary>
    public class BleGattService : BleGattAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BleGattService() : base(GattType.Service)
        { }

        /// <summary>
        /// Stack implementation specific context
        /// </summary>
        public object Context { get; set; }
    }

    /// <summary>
    /// BLE GATT characteristic
    /// </summary>
    public class BleGattCharacteristic : BleGattAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BleGattCharacteristic() : base(GattType.Characteristic)
        { }

        /// <summary>
        /// Indicate if the characteristic is readable
        /// </summary>
        public bool CanRead { get; set; }

        /// <summary>
        /// Indicate if the characteristic is writable
        /// </summary>
        public bool CanWrite { get; set; }

        /// <summary>
        /// Indicate if the characteristic can send notifications
        /// </summary>
        public bool CanNotify { get; set; }

        /// <summary>
        /// Stack implementation specific context
        /// </summary>
        public object Context { get; set; }
    }

    /// <summary>
    /// BLE GATT descriptor
    /// </summary>
    public class BleGattDescriptor : BleGattAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BleGattDescriptor() : base(GattType.Descriptor)
        { }

        /// <summary>
        /// Stack implementation specific context
        /// </summary>
        public object Context { get; set; }
    }



    /// <summary>
    /// Interface for all BLE device proxy implementation
    /// </summary>
    public interface IBleDevice
    {
        /// <summary>
        /// Get the BLE device information
        /// </summary>
        BleDeviceInformation DeviceInformation { get; }


        /// <summary>
        /// Get the list of the primary GATT services
        /// </summary>
        /// <returns>List of the primary GATT services</returns>
        Task<IList<BleGattService>> GetServicesAsync();

        /// <summary>
        /// Get the list of the GATT services included in a specific GATT service
        /// </summary>
        /// <returns>List of the included GATT services</returns>
        Task<IList<BleGattService>> GetServicesAsync(BleGattService service);

        /// <summary>
        /// Get the list of the GATT characteristics included in a specific GATT service
        /// </summary>
        /// <param name="service">GATT service</param>
        /// <returns>List of the included GATT characteristics</returns>
        Task<IList<BleGattCharacteristic>> GetCharacteristicsAsync(BleGattService service);

        /// <summary>
        /// Get the list of the GATT descriptors included in a specific GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <returns>List of the included GATT descriptors</returns>
        Task<IList<BleGattDescriptor>> GetDescriptorsAsync(BleGattCharacteristic characteristic);

        /// <summary>
        /// Write a value into a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="value">Value</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        Task<bool> WriteValueAsync(BleGattCharacteristic characteristic, BleValue value);

        /// <summary>
        /// Read a value from a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="value">Value</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        Task<bool> ReadValueAsync(BleGattCharacteristic characteristic, BleValue value);

        /// <summary>
        /// Register to notifications from a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="listener">Method which will be called on each value notification</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        Task<bool> RegisterValueNotificationAsync(BleGattCharacteristic characteristic, Action<BleGattCharacteristic, BleValue> listener);

        /// <summary>
        /// Unregister from notifications from a GATT characteristic
        /// </summary>
        /// <param name="characteristic">GATT characteristic</param>
        /// <param name="listener">Method which is called on each value notification</param>
        /// <returns>true if the operation succeeded, false otherwise</returns>
        Task<bool> UnregisterValueNotificationAsync(BleGattCharacteristic characteristic, Action<BleGattCharacteristic, BleValue> listener);
    }
}
