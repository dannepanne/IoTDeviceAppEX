using Device.ScreenWriterEX.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Device.ScreenWriterEX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string _deviceName = "ScreenWriter";
        public string _screenWriterText { get; set; } = "";
        public string _deviceId = "ScreenWriter-9cea-9ca9b8cfe6b2";
        private DeviceItem data { get; set; }
        private bool _isRunning = false;
        private bool _isConnected = false;
        DispatcherTimer _timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            Initialize();


            SetInterval(TimeSpan.FromMilliseconds(data.Interval));
            

            
        }

        private void SetInterval(TimeSpan interval)
        {
            _timer = new DispatcherTimer()
            {
                Interval = interval
            };

            _timer.Tick += new EventHandler(timer_tick);
            _timer.Start();
        }

        private async void timer_tick(object sender, EventArgs e)
        {

            await UpdateTwinPropertyStringAsync(RandomStringReturner());
        }


        public async Task UpdateTwinPropertyStringAsync(string screenString)
        {
            _screenWriterText = screenString;
            
            
            using var _deviceClient = DeviceClient.CreateFromConnectionString(data.DeviceConnectionString);
            var twin = await _deviceClient.GetTwinAsync();
            if (twin != null)
            {
                TwinCollection reported = new TwinCollection();

                reported["screenWriterText"] = _screenWriterText;

                await _deviceClient.UpdateReportedPropertiesAsync(reported);
            }
        }

        private string RandomStringReturner()
        {
            string randomString = "";
            Random randLenght = new Random();
            int randInt = randLenght.Next(40, 170);
            for (int i = 0; i < randInt; i++)
            {
                randomString += GetChar();
            }
            tb_box.Text = randomString;
            return randomString;
        }


        public string GetChar()
        {
            string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
            Random rand = new Random();
            int num = rand.Next(0, chars.Length);
            return chars[num].ToString();
        }

        private void Initialize()
        {


            _isConnected = Task.Run(async () =>
            {
                while (!_isConnected)
                {

                    try
                    {
                        using var http = new HttpClient();

                        var result = await http.PostAsJsonAsync("http://localhost:7133/api/devices", new
                        {
                            deviceId = _deviceId,
                            deviceName = _deviceName,
                            screenWriterText = _screenWriterText
                        });


                        if (result.IsSuccessStatusCode || result.StatusCode.ToString() == "Conflict")
                        {
                            data = JsonConvert.DeserializeObject<DeviceItem>(await result.Content.ReadAsStringAsync()); //gör ett deviceitem
                            using var deviceClient = DeviceClient.CreateFromConnectionString(data.DeviceConnectionString);

                            var twin = await deviceClient.GetTwinAsync();
                            if (twin != null)
                            {
                                TwinCollection reported = new TwinCollection();
                                reported["deviceId"] = _deviceId;
                                reported["deviceName"] = _deviceName;
                                reported["screenWriterText"] = _screenWriterText;
                                reported["DeviceConnectionString"] = data.DeviceConnectionString.ToString();


                                await deviceClient.UpdateReportedPropertiesAsync(reported);
                                _isRunning = true;
                                return true;
                            }
                        }
                    }
                    catch { }
                }

                return false;

            }).Result;
        }

    }
    //Twin prop!
}    //    register!
        //    kolla conn string - bool connected
        //    uppdatera twin!
    

