using IoTDeviceAppEX.MVVM.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceAppEX.MVVM.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            ControlPanelViewModel = new();
            ControlPanelViewCommand = new RelayCommand(x => { CurrentView = ControlPanelViewModel; });

            CurrentView = ControlPanelViewModel;
        }

        public RelayCommand ControlPanelViewCommand { get; set; }
        public ControlPanelViewModel ControlPanelViewModel { get; set; }
  





        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
    }
}
