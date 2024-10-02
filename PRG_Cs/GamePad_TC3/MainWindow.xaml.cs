using System;
using System.Windows;
using System.Windows.Controls;

namespace TwinCATUsbControllerApp
{
    public partial class MainWindow : Window
    {
        private ADSCommunication adsCommunication;
        private ControllerManager controllerManager;

        public MainWindow()
        {
            InitializeComponent();
            adsCommunication = new ADSCommunication();
            controllerManager = new ControllerManager();
            RefreshControllerList();

            // コントローラーの検出に時間がかかる場合があるため、少し遅延させて再度更新
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (sender, e) =>
            {
                RefreshControllerList();
                (sender as System.Windows.Threading.DispatcherTimer).Stop();
            };
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Start();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string amsNetId = AmsNetIdTextBox.Text;
            if (!int.TryParse(AdsPortTextBox.Text, out int adsPort))
            {
                MessageBox.Show("Invalid ADS Port number.");
                return;
            }

            if (adsCommunication.Connect(amsNetId, adsPort))
            {
                ConnectionStatus.Text = $"Connected to {amsNetId}:{adsPort}";
                MessageBox.Show($"Connected to TwinCAT at {amsNetId}:{adsPort}");
            }
            else
            {
                ConnectionStatus.Text = "Connection Failed";
            }
        }

        private void RefreshControllers_Click(object sender, RoutedEventArgs e)
        {
            RefreshControllerList();
        }

        private void RefreshControllerList()
        {
            var controllers = controllerManager.RefreshControllerList();
            Console.WriteLine($"Refreshed controller list. Found {controllers.Count} controllers.");

            Controller1ComboBox.ItemsSource = null;
            Controller2ComboBox.ItemsSource = null;

            Controller1ComboBox.ItemsSource = controllers;
            Controller2ComboBox.ItemsSource = controllers;

            Controller1ComboBox.SelectedIndex = -1;
            Controller2ComboBox.SelectedIndex = -1;
        }

        private void Controller1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Controller1ComboBox.SelectedIndex != -1)
            {
                Console.WriteLine($"Attempting to connect Controller 1, index: {Controller1ComboBox.SelectedIndex}");
                if (controllerManager.IsControllerAlreadyConnected(Controller1ComboBox.SelectedIndex))
                {
                    MessageBox.Show("This controller is already connected to another port.");
                    Controller1ComboBox.SelectedIndex = -1;
                    return;
                }
                if (controllerManager.ConnectController(1, Controller1ComboBox.SelectedIndex))
                {
                    Controller1Status.Text = $"Controller 1: Connected - {controllerManager.GetControllerName(1)}";
                    StartPolling();
                }
                else
                {
                    Controller1Status.Text = "Controller 1: Connection Failed";
                }
            }
        }

        private void Controller2ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Controller2ComboBox.SelectedIndex != -1)
            {
                Console.WriteLine($"Attempting to connect Controller 2, index: {Controller2ComboBox.SelectedIndex}");
                if (controllerManager.IsControllerAlreadyConnected(Controller2ComboBox.SelectedIndex))
                {
                    MessageBox.Show("This controller is already connected to another port.");
                    Controller2ComboBox.SelectedIndex = -1;
                    return;
                }
                if (controllerManager.ConnectController(2, Controller2ComboBox.SelectedIndex))
                {
                    Controller2Status.Text = $"Controller 2: Connected - {controllerManager.GetControllerName(2)}";
                    StartPolling();
                }
                else
                {
                    Controller2Status.Text = "Controller 2: Connection Failed";
                }
            }
        }

        private void StartPolling()
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(16); // 約60Hz
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateControllerState(1, Controller1Input);
            UpdateControllerState(2, Controller2Input);
        }

        private void UpdateControllerState(int controllerId, TextBlock textBlock)
        {
            var state = controllerManager.GetControllerState(controllerId);
            if (state != null)
            {
                textBlock.Text = state.ToString();
                adsCommunication.SendGamePadState(controllerId, state);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            controllerManager.Dispose();
            adsCommunication.Dispose();
        }
    }
}