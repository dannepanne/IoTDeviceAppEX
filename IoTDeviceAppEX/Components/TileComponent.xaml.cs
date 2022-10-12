using IoTDeviceAppEX.MVVM.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace IoTDeviceAppEX.Components
{
    /// <summary>
    /// Interaction logic for TileComponent.xaml
    /// </summary>
    public partial class TileComponent : UserControl
    {




        public TileComponent()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty DeviceNameProperty = DependencyProperty.Register("DeviceName", typeof(string), typeof(TileComponent));
        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty DeviceTextProperty = DependencyProperty.Register("DeviceText", typeof(string), typeof(TileComponent));
        public string DeviceText
        {
            get { return (string)GetValue(DeviceTextProperty); }
            set 
            { 
                SetValue(DeviceTextProperty, value);
            }
        }




        private async void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            using var _registryManager = RegistryManager.CreateFromConnectionString("HostName=IoThubKyh0907.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=X3wFcDFbyisE8Wu0tYutUrLuv1zyYSo0Qe8kCBFzrQg=");
            var button = sender as Button;
            var deviceItem = (DeviceItem)button.DataContext;
            await _registryManager.RemoveDeviceAsync(deviceItem.DeviceId);
        }







    }
}
