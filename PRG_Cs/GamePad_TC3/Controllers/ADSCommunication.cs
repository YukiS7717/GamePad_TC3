using System;
using TwinCAT.Ads;
using TwinCATUsbControllerApp.Models;
using TwinCATUsbControllerApp.Utilities;

namespace TwinCATUsbControllerApp.Controllers
{
    public class ADSCommunication : IDisposable
    {
        private AdsClient adsClient;

        public bool Connect(string amsNetId, int port)
        {
            try
            {
                adsClient = new AdsClient();
                adsClient.Connect(amsNetId, port);
                Logger.Log($"Connected to ADS at {amsNetId}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Error connecting to ADS: {ex.Message}");
                return false;
            }
        }

        public void SendGamePadState(int controllerId, ST_GamePad state)
        {
            if (adsClient == null || !adsClient.IsConnected) return;

            try
            {
                string varName = $"MAIN.Controller{controllerId}";
                uint variableHandle = adsClient.CreateVariableHandle(varName);
                adsClient.WriteAny(variableHandle, state);
                adsClient.DeleteVariableHandle(variableHandle);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error sending data to PLC: {ex.Message}");
            }
        }

        public void Dispose()
        {
            adsClient?.Dispose();
        }
    }
}