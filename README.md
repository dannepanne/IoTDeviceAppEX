# IoTDeviceAppEX
System with smartdevice app that generates random strings, api to connect and add the device to a IoT hub and smart app to display the random strings the smart device sends to the IoT hub.
There are some bugs, like how the smart app updates and re-renders the text on display. Also the app is designed for multiple tiles, one for each device, but the smartapp is made to just show one.
It is possible to remove a device through the smart app (the skull button, regrettably without any feedback to the user as of yet), it then removes the device from the IoT hub and to add it adain the project needs to be restarted. This is also not the best implementation but it does what it is supposed to do.
The smart app saves incoming data in a local database file. This doesn't match how to front end is re-rendered and so some duplicate or empty datastrings will be stored.

Some stuff that needs replacing is: 
*Connection strings in the appsettings.json in the api
*Line 122 in mainwindow.xaml.cs in the device contains the adress to the api, as this is running locally it might need to be changed
*A sql query is included to build the database table used for storing incoming data. A local database file was used when making this so that is recommended.
*In MVVM/Viewmodels/controlpanelviewmodel.xaml.cs line 31 & 101 the connection string needs to be changed to a IoThub on Azure.
*In that same file, line 32, the connection string needs to point at a local db file.
