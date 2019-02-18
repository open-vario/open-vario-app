using System;
using System.Collections.Generic;
using System.Text;

namespace OpenVario
{
    /// <summary>
    /// Interface for all functionalities which are platform dependant
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Retrieve the platform specific BLE stack object
        /// </summary>
        IBleStack BleStack { get; }
    }
}
