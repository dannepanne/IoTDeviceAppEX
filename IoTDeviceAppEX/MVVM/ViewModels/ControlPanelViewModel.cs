using IoTDeviceAppEX.MVVM.Cores;
using IoTDeviceAppEX.MVVM.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IoTDeviceAppEX.MVVM.ViewModels
{
    internal class ControlPanelViewModel : ObservableObject
    {
        private DispatcherTimer timer;
        private ObservableCollection<DeviceItem> _deviceItems;

        public ObservableCollection<DeviceItem> deviceItem
        {
            get => _deviceItems;
            set 
            { 
                deviceItem = value;
                OnPropertyChanged();
            }
        }

        //knapp ta bort device

        //database connection - nuget sql, spara, data i databas mottagen data

        private List<DeviceItem> _tempList;
        private readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString("HostName=IoThubKyh0907.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=X3wFcDFbyisE8Wu0tYutUrLuv1zyYSo0Qe8kCBFzrQg=");
        
        
        private string? _onScreenText;
        public string? onScreenText
        {
            get => _onScreenText; 
            set 
            { 
                _onScreenText = value;
                OnPropertyChanged();
            }
        }

        public ControlPanelViewModel()
        {
            _tempList = new List<DeviceItem>();
            _deviceItems = new ObservableCollection<DeviceItem>();
            PopulateDeviceItemsAsync().ConfigureAwait(false);
            SetInterval(TimeSpan.FromSeconds(3));
        }


        public IEnumerable<DeviceItem> DeviceItems => _deviceItems;



        private void SetInterval(TimeSpan interval)
        {
            timer = new DispatcherTimer()
            {
                Interval = interval
            };

            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }

        private async void timer_tick(object sender, EventArgs e)
        {
            await PopulateDeviceItemsAsync();
            await UpdateDeviceItemsAsync();
            await UpdateText();

        }

        public async Task UpdateText()
        {
            var result = registryManager.CreateQuery("select * from devices WHERE properties.reported.deviceName = 'ScreenWriter'");


            foreach (Twin twin in await result.GetNextAsTwinAsync())
            {
                try
                {
                    _deviceItems[0].DeviceText = twin.Properties.Reported["screenWriterText"];
                }
                catch { }
             
            }

            onScreenText = _deviceItems[0].DeviceText;
        }


        private async Task UpdateDeviceItemsAsync()
        {
            _tempList.Clear();

            foreach (var item in _deviceItems)
            {
                var device = await registryManager.GetDeviceAsync(item.DeviceId);
                if (device == null)
                    _tempList.Add(item);
            }

            foreach (var item in _tempList)
            {
                _deviceItems.Remove(item);
            }
        }

       

        private async Task PopulateDeviceItemsAsync()
        {
            var result = registryManager.CreateQuery("select * from devices WHERE properties.reported.deviceName = 'ScreenWriter'"); 

            if (result.HasMoreResults)
            {
                foreach (Twin twin in await result.GetNextAsTwinAsync())
                {
                    var device = _deviceItems.FirstOrDefault(x => x.DeviceId == twin.DeviceId);

                    if (device == null)
                    {
                        device = new DeviceItem
                        {
                            DeviceId = twin.DeviceId,
                           
                        };

                        try { device.DeviceName = twin.Properties.Reported["deviceName"]; }
                        catch { device.DeviceName = device.DeviceId; }
                        try { device.DeviceType = twin.Properties.Reported["deviceType"]; }
                        catch { }
                        try { device.DeviceText = twin.Properties.Reported["screenWriterText"]; }
                        catch { }
                        
                                device.IconActive = "\uf2a1";
                                device.IconInActive = "\uf6aa";
                                device.StateActive = "ENABLE";
                                device.StateInActive = "DISABLE";


                        _deviceItems.Add(device);
                    }
                    else 
                    {
                        onScreenText = twin.Properties.Reported["screenWriterText"];
                        device.DeviceText = onScreenText;

                    }
                }
            }
            else
            {
                _deviceItems.Clear();
            }
        }
    }
}
