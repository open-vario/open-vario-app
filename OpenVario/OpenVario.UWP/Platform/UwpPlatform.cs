using System;
using System.Collections.Generic;
using System.Text;

namespace OpenVario.UWP
{
    /// <summary>
    /// UWP platform dependant functionalities
    /// </summary>
    public class UwpPlatform : IPlatform
    {
        /// <summary>
        /// Retrieve the platform specific BLE stack object
        /// </summary>
        public IBleStack BleStack { get { return _ble_stack; } }

        /// <summary>
        /// BLE stack
        /// </summary>
        private UwpBleStack _ble_stack;

        /// <summary>
        /// Constructor
        /// </summary>
        public UwpPlatform()
        {
            _ble_stack = new UwpBleStack();
        }
    }
}
