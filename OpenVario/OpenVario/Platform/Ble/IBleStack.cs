using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVario
{
    /// <summary>
    /// Contain information on a discovered BLE device
    /// </summary>
    public class BleDeviceInformation
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Device's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Device's MAC address
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// Indicate if the device is already connected
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Indicate if a connection to the device can be established
        /// </summary>
        public bool IsConnectable { get; set; }

        /// <summary>
        /// Stack implementation specific context
        /// </summary>
        public object Context { get; set; }
    }

    /// <summary>
    /// Status of the BLE device discovery process
    /// </summary>
    public enum BleDiscoveryStatus
    {
        /// <summary>
        /// Discovery stopped
        /// </summary>
        BDS_STOPPED,
        /// <summary>
        /// Discovery in progress
        /// </summary>
        BDS_IN_PROGRESS,
        /// <summary>
        /// Discovery complete
        /// </summary>
        BDS_COMPLETE,
        /// <summary>
        /// Discovery complete
        /// </summary>
        BDS_CANCELED,
        /// <summary>
        /// Discovery failed
        /// </summary>
        BDS_FAILED
    }


    /// <summary>
    /// Handler for the device discovery events
    /// </summary>
    /// <param name="sender">Stack instance which triggered the event</param>
    /// <param name="device_info">BLE device information associated to the event</param>
    public delegate void DeviceDiscoveryHandler(IBleStack sender, BleDeviceInformation device_info);

    /// <summary>
    /// Handler for the discovery completion event
    /// </summary>
    /// <param name="sender">Stack instance which triggered the event</param>
    /// <param name="status">Discovery status</param>
    public delegate void DiscoveryCompleteHandler(IBleStack sender, BleDiscoveryStatus status);


    /// <summary>
    /// Interface for all BLE stack implementations
    /// </summary>
    public interface IBleStack
    {
        #region BLE device discovery

        /// <summary>
        /// Start the discovery of BLE devices
        /// </summary>
        /// <returns>true if the discovery has started, fals otherwise</returns>
        bool StartDeviceDiscovery();

        /// <summary>
        /// Stop the discovery of BLE device
        /// </summary>
        /// <returns>true if the discovery is stopping, false otherwise</returns>
        bool StopDeviceDiscovery();

        /// <summary>
        /// Get the current discovery status
        /// </summary>
        BleDiscoveryStatus DiscoveryStatus { get; }

        /// <summary>
        /// Event triggered when a BLE device has been discovered
        /// </summary>
        event DeviceDiscoveryHandler DeviceDiscovered;

        /// <summary>
        /// Event triggered when information on a discovered BLE device has been updated
        /// </summary>
        event DeviceDiscoveryHandler DeviceUpdated;

        /// <summary>
        /// Event triggered when a discovered BLE device is no longer reachable
        /// </summary>
        event DeviceDiscoveryHandler DeviceLost;

        /// <summary>
        /// Event triggered at the end of the discovery process
        /// </summary>
        event DiscoveryCompleteHandler DiscoveryComplete;

        #endregion


        #region BLE device creation

        /// <summary>
        /// Create a BLE device proxy object from a BLE device information
        /// </summary>
        /// <param name="device_info">BLE device information to use</param>
        /// <returns>BLE device proxy object on success, null otherwise</returns>
        Task<IBleDevice> CreateDeviceAsync(BleDeviceInformation device_info);


        #endregion
    }
}
