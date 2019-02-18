using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace OpenVario
{
    

    public partial class MainPage : ContentPage
    {
        private ObservableCollection<BleDeviceInformation> BleDevices = new ObservableCollection<BleDeviceInformation>();
        private IBleStack _ble_stack;
        private OpenVarioDevice _open_vario_device;

        public RelayCommand StartDiscoveryCommand { get; private set; }
        public RelayCommand StopDiscoveryCommand { get; private set; }
        public RelayCommand ConnectCommand { get; private set; }
        public RelayCommand NotifCommand { get; private set; }

        public MainPage(IBleStack ble_stack)
        {
            StartDiscoveryCommand = new RelayCommand(StartDiscovery);
            StopDiscoveryCommand = new RelayCommand(StopDiscovery);
            ConnectCommand = new RelayCommand(Connect);
            NotifCommand = new RelayCommand(Notifications);
            BindingContext = this;

            InitializeComponent();

            lvDevices.ItemsSource = BleDevices;

            _ble_stack = ble_stack;
            _ble_stack.DeviceDiscovered += IBleStack_DeviceDiscovered;
            _ble_stack.DeviceLost += IBleStack_DeviceLost;
            _ble_stack.DeviceUpdated += IBleStack_DeviceUpdated;
            _ble_stack.DiscoveryComplete += IBleStack_DiscoveryComplete;

        }

        void StartDiscovery(object param)
        {
            if (_ble_stack.StartDeviceDiscovery())
            {
                lbStatus.Text = "Discovery in progress...";
            }
        }

        void StopDiscovery(object param)
        {
            if (_ble_stack.StopDeviceDiscovery())
            {
                lbStatus.Text = "Stopping discovery...";
            }
        }

        async void Connect(object param)
        {
            if (lvDevices.SelectedItem != null)
            {
            
                lbStatus.Text = "Connecting...";
                IBleDevice ble_device = await _ble_stack.CreateDeviceAsync(lvDevices.SelectedItem as BleDeviceInformation);

                _open_vario_device = new OpenVarioDevice(ble_device);
                await _open_vario_device.Initialize();

                lbStatus.Text = "Connected!";
            }
        }

        async void Notifications(object param)
        {
            if (_open_vario_device != null)
            {
                AltimeterService altimeter_service = _open_vario_device.AltimeterService;
                altimeter_service.MainAltitudeChanged += OnAltitudeChanged;
                altimeter_service.Altitude1Changed += OnAltitudeChanged;
                altimeter_service.Altitude2Changed += OnAltitudeChanged;
                altimeter_service.Altitude3Changed += OnAltitudeChanged;
                altimeter_service.Altitude4Changed += OnAltitudeChanged;
                await altimeter_service.StartNotification(AltimeterService.Altitude.MainAltitude);
                await altimeter_service.StartNotification(AltimeterService.Altitude.Altitude1);
                await altimeter_service.StartNotification(AltimeterService.Altitude.Altitude2);
                await altimeter_service.StartNotification(AltimeterService.Altitude.Altitude3);
                await altimeter_service.StartNotification(AltimeterService.Altitude.Altitude4);

                lbStatus.Text = "Notifications started!";
            }
        }

        private void OnAltitudeChanged(AltimeterService.Altitude altitude, Int16 value)
        {
            switch (altitude)
            {
                case AltimeterService.Altitude.MainAltitude:
                    {
                        ExecuteOnMainThread.BeginInvoke(() => { lblMainAlti.Text = value.ToString() + "m"; });
                        break;
                    }

                case AltimeterService.Altitude.Altitude1:
                    {
                        ExecuteOnMainThread.BeginInvoke(() => { lblAlti1.Text = value.ToString() + "m"; });
                        break;
                    }

                case AltimeterService.Altitude.Altitude2:
                    {
                        ExecuteOnMainThread.BeginInvoke(() => { lblAlti2.Text = value.ToString() + "m"; });
                        break;
                    }

                case AltimeterService.Altitude.Altitude3:
                    {
                        ExecuteOnMainThread.BeginInvoke(() => { lblAlti3.Text = value.ToString() + "m"; });
                        break;
                    }

                case AltimeterService.Altitude.Altitude4:
                    {
                        ExecuteOnMainThread.BeginInvoke(() => { lblAlti4.Text = value.ToString() + "m"; });
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private void IBleStack_DeviceDiscovered(IBleStack sender, BleDeviceInformation device_info)
        {
            ExecuteOnMainThread.Invoke(() => { BleDevices.Add(device_info); });
        }

        private void IBleStack_DeviceLost(IBleStack sender, BleDeviceInformation device_info)
        {
            int index = 0;
            foreach (BleDeviceInformation ble_device_info in BleDevices)
            {
                if (ble_device_info.Id == device_info.Id)
                {
                    break;
                }
                else
                {
                    index++;
                }
            }
            if (index < BleDevices.Count)
            {
                ExecuteOnMainThread.Invoke(() => { BleDevices.RemoveAt(index: index); });
            }
        }

        private void IBleStack_DeviceUpdated(IBleStack sender, BleDeviceInformation device_info)
        {
            foreach (BleDeviceInformation ble_device_info in BleDevices)
            {
                if (ble_device_info.Id == device_info.Id)
                {
                    ble_device_info.MacAddress = device_info.MacAddress;
                    ble_device_info.IsConnectable = device_info.IsConnectable;
                    ble_device_info.IsConnected = device_info.IsConnected;
                    break;
                }
            }
        }

        private void IBleStack_DiscoveryComplete(IBleStack sender,BleDiscoveryStatus status)
        {
            ExecuteOnMainThread.BeginInvoke(() =>
            {
                string discovery_status = "Discovery status : ";
                switch (status)
                {
                    case BleDiscoveryStatus.BDS_COMPLETE:
                    {
                        discovery_status += "Complete!";
                        break;
                    }

                    case BleDiscoveryStatus.BDS_CANCELED:
                    {
                        discovery_status += "Canceled!";
                        break;
                    }

                    case BleDiscoveryStatus.BDS_FAILED:
                    {
                        discovery_status += "Failed!";
                        break;
                    }

                    default:
                    {
                        discovery_status += "Unknown...";
                        break;
                    }
                }
                lbStatus.Text = discovery_status;
            });
        }

        private async void ButtonMainAlti_Clicked(object sender, EventArgs e)
        {
            Int16 value = Convert.ToInt16(entryMainAlti.Text);
            await _open_vario_device.AltimeterService.WriteAltitude(AltimeterService.Altitude.MainAltitude, value);
        }
    }
}
