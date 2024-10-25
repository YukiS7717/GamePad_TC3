using System;
using System.Windows;
using System.Windows.Controls;
using TwinCATUsbControllerApp.Controllers;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp
{
    public partial class MainWindow : Window
    {
        private ADSCommunication adsCommunication;
        private ControllerManager controllerManager;
        private ConfigurationManager configManager;
        private System.Windows.Threading.DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            configManager = new ConfigurationManager();
            configManager.LoadConfiguration();
            adsCommunication = new ADSCommunication();
            controllerManager = new ControllerManager(configManager);
            InitializeUI();
        }

        private void InitializeUI()
        {
            RefreshControllerList();
            InitializeMappingComboBoxes();
            InitializeGyroSettings();
        }

        private void InitializeMappingComboBoxes()
        {
            var mappings = controllerManager.GetAvailableMappings();
            Controller1MappingComboBox.ItemsSource = mappings;
            Controller2MappingComboBox.ItemsSource = mappings;
            Controller1MappingComboBox.SelectedItem = configManager.Controller1Mapping;
            Controller2MappingComboBox.SelectedItem = configManager.Controller2Mapping;
        }

        private void InitializeGyroSettings()
        {
            GyroEnabledCheckBox.IsChecked = configManager.GyroEnabled;
            GyroSensitivitySlider.Value = configManager.GyroSensitivity;
            GyroSensitivityValue.Text = configManager.GyroSensitivity.ToString("F1");
            GyroInvertXCheckBox.IsChecked = configManager.GyroInvertX;
            GyroInvertYCheckBox.IsChecked = configManager.GyroInvertY;
            GyroInvertZCheckBox.IsChecked = configManager.GyroInvertZ;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string amsNetId = AmsNetIdTextBox.Text;
            if (!int.TryParse(AdsPortTextBox.Text, out int adsPort))
            {
                MessageBox.Show("Invalid ADS Port number.");
                return;
            }

            try
            {
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
            catch (Exception ex)
            {
                ConnectionStatus.Text = "Connection Failed";
                MessageBox.Show($"Connection error: {ex.Message}");
            }
        }

        private void RefreshControllers_Click(object sender, RoutedEventArgs e)
        {
            RefreshControllerList();
        }

        private void RefreshControllerList()
        {
            var controllers = controllerManager.RefreshControllerList();
            Controller1ComboBox.ItemsSource = controllers;
            Controller2ComboBox.ItemsSource = controllers;
        }

        private void Controller1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Controller1ComboBox.SelectedIndex != -1)
            {
                if (controllerManager.ConnectController(1, Controller1ComboBox.SelectedIndex))
                {
                    UpdateControllerStatus(1);
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
                if (controllerManager.ConnectController(2, Controller2ComboBox.SelectedIndex))
                {
                    UpdateControllerStatus(2);
                    StartPolling();
                }
                else
                {
                    Controller2Status.Text = "Controller 2: Connection Failed";
                }
            }
        }

        private void Controller1MappingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Controller1MappingComboBox.SelectedItem is string selectedMapping)
            {
                controllerManager.SetControllerMapping(1, selectedMapping);
                configManager.Controller1Mapping = selectedMapping;
                configManager.SaveConfiguration();
                UpdateControllerStatus(1);
            }
        }

        private void Controller2MappingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Controller2MappingComboBox.SelectedItem is string selectedMapping)
            {
                controllerManager.SetControllerMapping(2, selectedMapping);
                configManager.Controller2Mapping = selectedMapping;
                configManager.SaveConfiguration();
                UpdateControllerStatus(2);
            }
        }

        private void UpdateControllerStatus(int controllerId)
        {
            var status = controllerId == 1 ? Controller1Status : Controller2Status;
            var controllerName = controllerManager.GetControllerName(controllerId);
            var mapping = controllerId == 1 ? configManager.Controller1Mapping : configManager.Controller2Mapping;
            status.Text = $"Controller {controllerId}: Connected - {controllerName} (Mapping: {mapping})";
        }

        private void StartPolling()
        {
            if (timer == null)
            {
                timer = new System.Windows.Threading.DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(16); // Approximately 60Hz
                timer.Start();
            }
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
                try
                {
                    adsCommunication.SendGamePadState(controllerId, state);
                }
                catch (Exception ex)
                {
                    ConnectionStatus.Text = $"Communication Error: {ex.Message}";
                }
            }
        }

        private void GyroEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            configManager.GyroEnabled = true;
            configManager.SaveConfiguration();
        }

        private void GyroEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            configManager.GyroEnabled = false;
            configManager.SaveConfiguration();
        }

        private void GyroSensitivitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (configManager != null)
            {
                configManager.GyroSensitivity = (float)e.NewValue;
                GyroSensitivityValue.Text = configManager.GyroSensitivity.ToString("F1");
                configManager.SaveConfiguration();
            }
        }

        private void GyroInvertXCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            configManager.GyroInvertX = true;
            configManager.SaveConfiguration();
        }

        private void GyroInvertXCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            configManager.GyroInvertX = false;
            configManager.SaveConfiguration();
        }

        private void GyroInvertYCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            configManager.GyroInvertY = true;
            configManager.SaveConfiguration();
        }

        private void GyroInvertYCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            configManager.GyroInvertY = false;
            configManager.SaveConfiguration();
        }

        private void GyroInvertZCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            configManager.GyroInvertZ = true;
            configManager.SaveConfiguration();
        }

        private void GyroInvertZCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            configManager.GyroInvertZ = false;
            configManager.SaveConfiguration();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timer?.Stop();
            controllerManager.Dispose();
            adsCommunication.Dispose();
        }
    }
}