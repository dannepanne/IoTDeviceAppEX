using Microsoft.Azure.Devices.Client;
using System.Net.WebSockets;
using System.Text;


namespace Device.ScreenWriter;


class Program
{

    private static readonly DeviceClient _deviceClient = DeviceClient.CreateFromConnectionString("\"HostName=IoThubKyh0907.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=X3wFcDFbyisE8Wu0tYutUrLuv1zyYSo0Qe8kCBFzrQg=");
    public string DeviceName { get; set; }
    public string ScreenMessage { get; set; }
    public string DeviceId { get; set; }


    public static void Main()
    {


        //TIMER
        //Uppdatera twin i denna


        for (int i = 0; i < 20; i++)
        {
            Console.WriteLine(RandomStringReturner());
        }



        System.Console.ReadKey();

    }
    //Twin prop!
    //    register!
    //    kolla conn string - bool connected
    //    uppdatera twin!



    public static string RandomStringReturner()
    {
        string randomString ="";
        Random randLenght = new Random();
        int randInt = randLenght.Next(0, 70);
        for (int i = 0; i < randInt; i++)
        {
            randomString += GetChar();
        }

        return randomString;
    }


    public static string GetChar()
    {
        string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
        Random rand = new Random();
        int num = rand.Next(0, chars.Length);
        return chars[num].ToString();
    }

    

    

}