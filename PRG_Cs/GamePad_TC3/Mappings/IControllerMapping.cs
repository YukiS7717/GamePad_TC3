using System;
using System.Collections.Generic;
using SharpDX.DirectInput;
using TwinCATUsbControllerApp.Models;

namespace TwinCATUsbControllerApp.Mappings
{
    public interface IControllerMapping
    {
        string Name { get; }
        Dictionary<string, int> GetButtonMapping();
        Dictionary<string, Func<int, short>> GetAxisMapping();
        ST_GamePad MapState(JoystickState state);
    }
}