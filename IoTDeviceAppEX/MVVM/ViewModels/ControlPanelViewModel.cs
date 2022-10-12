using Dapper;
using IoTDeviceAppEX.MVVM.Cores;
using IoTDeviceAppEX.MVVM.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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

       


        //knapp ta bort device

        //database connection - nuget sql, spara, data i databas mottagen data

        private List<DeviceItem> _tempList;
        private readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString("HostName=IoTHubSystemDanielEX.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=JF/dHu9W5hWBfvCFrhVB26BVHvjYYSK1ImAflQTkKiQ=");
        private readonly string dbConnection = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\danne\\source\\repos\\IotDeviceWPF\\IotDeviceWPF\\Data\\device_db.mdf;Integrated Security=True;Connect Timeout=30";

       

        public ControlPanelViewModel()
        {
            _tempList = new List<DeviceItem>();
            _deviceItems = new ObservableCollection<DeviceItem>();
            PopulateDeviceItemsAsync().ConfigureAwait(false);
            SetInterval(TimeSpan.FromSeconds(5));
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
            

        }

        private async Task UpdateDeviceItemsAsync()
        {
            if (_deviceItems != null)
            {
                _tempList.Clear();
                try
                {
                    foreach (var item in _deviceItems)
                    {
                        var device = await registryManager.GetDeviceAsync(item.DeviceId);
                        var twin = await registryManager.GetTwinAsync(item.DeviceId);

                        DeviceText = twin.Properties.Reported["screenWriterText"].ToString();


                        if (device == null)
                            _tempList.Add(item);
                    }
                }
                catch { }
                foreach (var item in _tempList)
                {
                    _deviceItems.Remove(item);
                }
            }
            
        }

       

        private async Task PopulateDeviceItemsAsync()
        {
            using var _registryManager = RegistryManager.CreateFromConnectionString("HostName=IoTHubSystemDanielEX.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=JF/dHu9W5hWBfvCFrhVB26BVHvjYYSK1ImAflQTkKiQ=");
            var result = _registryManager.CreateQuery("select * from devices WHERE properties.reported.deviceName = 'ScreenWriter'"); 

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
                        try { device.ConnectionString = twin.Properties.Reported["DeviceConnectionString"]; }
                        catch { }


                        _deviceItems.Add(device);
                    }
                    else 
                    {
                        
                        _deviceItems.Clear();
                        _deviceItems.Add(device);
                        DeviceText = twin.Properties.Reported["screenWriterText"].ToString();
                        device.DeviceText = DeviceText;
                        await UpdateData(device.DeviceId, device.DeviceName, device.ConnectionString, device.DeviceText);

                    }
                }
            }
            else
            {
                _deviceItems.Clear();
            }
        }


        private string deviceText;

        public string DeviceText
        {
            get { return deviceText; }
            set
            {
                deviceText = value;
                OnPropertyChanged(nameof(DeviceText));
            }
        }

        

        public async Task UpdateData(string deviceId, string deviceName, string connectionString, string stringData)
        {
            using IDbConnection conn = new SqlConnection(dbConnection);

            await conn.ExecuteAsync("INSERT INTO DataRecieved (dataId, deviceId,deviceName,connectionString,stringData,timeData) VALUES (@DataId, @DeviceId, @DeviceName, @ConnectionString, @StringData, @TimeData)", new 
            { 
                DataId = Guid.NewGuid().ToString(),
                DeviceId = deviceId, 
                DeviceName = deviceName, 
                ConnectionString = connectionString, 
                StringData = stringData, 
                TimeData =  DateTime.Now.ToString()
            });

        }

    }
} 
