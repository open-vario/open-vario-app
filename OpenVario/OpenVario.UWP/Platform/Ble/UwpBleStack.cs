using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace OpenVario.UWP
{
    /// <summary>
    /// BLE stack implementaton for UWP platform
    /// </summary>
    public class UwpBleStack : IBleStack
	{
        /// <summary>
        /// Discovery status
        /// </summary>
        private BleDiscoveryStatus _discovery_status;

        /// <summary>
        /// Device watcher to enumerate BLE devices
        /// </summary>
        private DeviceWatcher _device_watcher;
        


        /// <summary>
        /// Constructor
        /// </summary>
        public UwpBleStack()
        {
            // Create a device watcher to discover BLE devices only with their associated properties
            string[] requested_properties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };
            string aqs_ble_devices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
            _device_watcher = DeviceInformation.CreateWatcher(aqs_ble_devices,
                                                              requested_properties,
                                                              DeviceInformationKind.AssociationEndpoint);

            // Register to watcher events
            _device_watcher.Added += DeviceWatcher_Added;
            _device_watcher.Updated += DeviceWatcher_Updated;
            _device_watcher.Removed += DeviceWatcher_Removed;
            _device_watcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            _device_watcher.Stopped += DeviceWatcher_Stopped;

            // Intialize discovery status
            _discovery_status = BleDiscoveryStatus.BDS_STOPPED;
        }



        #region BLE device discovery

        /// <summary>
        /// Start the discovery of BLE devices
        /// </summary>
        /// <returns>true if the discovery has started, fals otherwise</returns>
        public bool StartDeviceDiscovery()
        {
            bool ret = false;

            // Check if the discovery process is stopped
            if (_device_watcher.Status != DeviceWatcherStatus.Started)
            {
                // Start the discovery process
                _device_watcher.Start();
                _discovery_status = BleDiscoveryStatus.BDS_IN_PROGRESS;
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Stop the discovery of BLE device
        /// </summary>
        /// <returns>true if the discovery is stopping, false otherwise</returns>
        public bool StopDeviceDiscovery()
        {
            bool ret = false;

            // Check if the discovery process is started
            if (_device_watcher.Status == DeviceWatcherStatus.Started)
            {
                // Stop the discovery process
                _device_watcher.Stop();
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Get the current discovery status
        /// </summary>
        public BleDiscoveryStatus DiscoveryStatus { get { return _discovery_status; } }

        /// <summary>
        /// Event triggered when a BLE device has been discovered
        /// </summary>
        public event DeviceDiscoveryHandler DeviceDiscovered;

        /// <summary>
        /// Event triggered when information on a discovered BLE device has been updated
        /// </summary>
        public event DeviceDiscoveryHandler DeviceUpdated;

        /// <summary>
        /// Event triggered when a discovered BLE device is no longer reachable
        /// </summary>
        public event DeviceDiscoveryHandler DeviceLost;

        /// <summary>
        /// Event triggered at the end of the discovery process
        /// </summary>
        public event DiscoveryCompleteHandler DiscoveryComplete;


        /// <summary>
        /// Called when a BLE device has been discovered
        /// </summary>
        /// <param name="sender">Device watcher which discovered the device</param>
        /// <param name="device_info">BLE device information</param>
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation device_info)
        {
            // Retrieve device information
            BleDeviceInformation ble_device_info = new BleDeviceInformation();
            ble_device_info.Id = device_info.Id;
            ble_device_info.Name = device_info.Name;
            try
            {
                ble_device_info.MacAddress = device_info.Properties["System.Devices.Aep.DeviceAddress"].ToString();
            }
            catch (KeyNotFoundException)
            {
                ble_device_info.MacAddress = "";
            }
            try
            {
                ble_device_info.IsConnected = (bool)device_info.Properties["System.Devices.Aep.IsConnected"];
            }
            catch (KeyNotFoundException)
            {
                ble_device_info.IsConnected = false;
            }
            try
            {
                ble_device_info.IsConnectable = (bool)device_info.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"];
            }
            catch (KeyNotFoundException)
            {
                ble_device_info.IsConnectable = false;
            }
            ble_device_info.Context = device_info;

            // Notify device discovery
            DeviceDiscovered?.Invoke(this, ble_device_info);
        }

        /// <summary>
        /// Called when information on a discovered BLE device has changed
        /// </summary>
        /// <param name="sender">Device watcher which discovered the device</param>
        /// <param name="device_info">BLE device information</param>
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate device_info)
        {
            // Retrieve device information
            BleDeviceInformation ble_device_info = new BleDeviceInformation();
            ble_device_info.Id = device_info.Id;
            try
            {
                ble_device_info.MacAddress = device_info.Properties["System.Devices.Aep.DeviceAddress"].ToString();
            }
            catch (KeyNotFoundException)
            {
                ble_device_info.MacAddress = "";
            }
            try
            {
                ble_device_info.IsConnected = (bool)device_info.Properties["System.Devices.Aep.IsConnected"];
            }
            catch (KeyNotFoundException)
            {
                ble_device_info.IsConnected = false;
            }
            try
            {
                ble_device_info.IsConnectable = (bool)device_info.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"];
            }
            catch (KeyNotFoundException)
            {
                ble_device_info.IsConnectable = false;
            }

            // Notify device update
            DeviceUpdated?.Invoke(this, ble_device_info);
        }

        /// <summary>
        /// Called when a discovered BLE device is no longer reachable
        /// </summary>
        /// <param name="sender">Device watcher which discovered the device</param>
        /// <param name="device_info">BLE device information</param>
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate device_info)
        {
            // Retrieve device information
            BleDeviceInformation ble_device_info = new BleDeviceInformation();
            ble_device_info.Id = device_info.Id;

            // Notify device removal
            DeviceLost?.Invoke(this, ble_device_info);
        }

        /// <summary>
        /// Called when the discovery process is complete
        /// </summary>
        /// <param name="sender">Device watcher which did the discovery process</param>
        /// <param name="e">not used, always null</param>
        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            // Update discovery status
            _discovery_status = BleDiscoveryStatus.BDS_COMPLETE;
            _device_watcher.Stop();

            // Notify end of discovery
            DiscoveryComplete?.Invoke(this, BleDiscoveryStatus.BDS_COMPLETE);
        }

        /// <summary>
        /// Called when the discovery process has been stopped
        /// </summary>
        /// <param name="sender">Device watcher which did the discovery process</param>
        /// <param name="e">not used, always null</param>
        private void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
            // Check if the discovery process is not already complete
            if (_discovery_status != BleDiscoveryStatus.BDS_COMPLETE)
            {
                // Update discovery status
                _discovery_status = BleDiscoveryStatus.BDS_CANCELED;

                // Notify end of discovery
                DiscoveryComplete?.Invoke(this, BleDiscoveryStatus.BDS_CANCELED);
            }
        }


        #endregion


        #region BLE device creation

        /// <summary>
        /// Create a BLE device proxy object from a BLE device information
        /// </summary>
        /// <param name="device_info">BLE device information to use</param>
        /// <returns>BLE device proxy object on success, null otherwise</returns>
        public async Task<IBleDevice> CreateDeviceAsync(BleDeviceInformation device_info)
        {
            IBleDevice ble_device = null;

            UwpBleDevice win_ble_device = new UwpBleDevice(device_info);
            bool init_succeed = await win_ble_device.Initialize();
            if (init_succeed)
            {
                ble_device = win_ble_device;
            }
        
            return ble_device;
        }


        #endregion

    }
}
