using Device.ScreenWriterConsole.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Device.ScreenWriterConsole;

class Program
{


    private static readonly DeviceClient _deviceClient = DeviceClient.CreateFromConnectionString("\"HostName=IoThubKyh0907.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=X3wFcDFbyisE8Wu0tYutUrLuv1zyYSo0Qe8kCBFzrQg=");
    public string _deviceName = "ScreenWriter";
    public string _screenWriterText { get; set; } = "";
    public string _deviceId = "ScreenWriter-9cea-9ca9b8cfe6b2";

    private bool _isRunning = false;
    private bool _isConnected = false;


    public void Main()
    {

        Initialize();

        //Uppdatera twin i denna

        while(true)
        {
            UpdateTwinPropertyStringAsync(RandomStringReturner());
            Task.Delay(5000);
            // task delay 5000 UpdateTwinPropertyString
        }
        



        System.Console.ReadKey();

    }
    //Twin prop!
    //    register!
    //    kolla conn string - bool connected
    //    uppdatera twin!



    private async Task UpdateTwinPropertyStringAsync(string screenString)
    {
        _screenWriterText = screenString;

        var twin = await _deviceClient.GetTwinAsync();
        if (twin != null)
        {
            TwinCollection reported = new TwinCollection();
            
            reported["screenWriterText"] = _screenWriterText;

            await _deviceClient.UpdateReportedPropertiesAsync(reported);
        }
    }

    public string RandomStringReturner()
    {
        string randomString = "";
        Random randLenght = new Random();
        int randInt = randLenght.Next(0, 70);
        for (int i = 0; i < randInt; i++)
        {
            randomString += GetChar();
        }

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
                        var data = JsonConvert.DeserializeObject<AddDeviceResponse>(await result.Content.ReadAsStringAsync());
                        using var deviceClient = DeviceClient.CreateFromConnectionString(data.DeviceConnectionString);

                        var twin = await deviceClient.GetTwinAsync();
                        if (twin != null)
                        {
                            TwinCollection reported = new TwinCollection();
                            reported["deviceId"] = _deviceId;
                            reported["deviceType"] = _deviceName;
                            reported["screenWriterText"] = _screenWriterText;

                            await deviceClient.UpdateReportedPropertiesAsync(reported);
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

