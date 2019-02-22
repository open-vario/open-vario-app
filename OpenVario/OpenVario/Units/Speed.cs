using System;
using System.Collections.Generic;
using System.Text;

namespace OpenVario
{
    /// <summary>
    /// Represents a speed value
    /// </summary>
    class Speed : ICloneable
    {
        /// <summary>
        /// Speed units
        /// </summary>
        public enum Unit
        {
            /// <summary>
            /// Meters per second
            /// </summary>
            MeterPerSec,
            /// <summary>
            /// Feets per sec
            /// </summary>
            FeetPerSec,
            /// <summary>
            /// Meters per minute
            /// </summary>
            MeterPerMin,
            /// <summary>
            /// Feets per minute
            /// </summary>
            FeetPerMin,
            /// <summary>
            /// Kilometers per hour
            /// </summary>
            KmPerHour,
            /// <summary>
            /// Miles per hour
            /// </summary>
            MilesPerHour
        }

        /// <summary>
        /// Speed value in the selected unit
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Selected speed unit
        /// </summary>
        public Unit SelectedUnit
        {
            get { return _unit; }
            set
            {
                // Convert to reference unit
                Value = Value / _unit_ratios[_unit];

                // Store unit
                _unit = value;

                // Convert to new unit
                Value = Value * _unit_ratios[_unit];
            }
        }

        /// <summary>
        /// Selected speed unit
        /// </summary>
        private Unit _unit;

        /// <summary>
        /// Ratios to convert units from the reference unit
        /// </summary>
        private static readonly Dictionary<Unit, double> _unit_ratios = new Dictionary<Unit, double>
        {
            { Unit.MeterPerSec, 1.0 },
            { Unit.FeetPerSec, 3.2808 },
            { Unit.MeterPerMin, 60.0 },
            { Unit.FeetPerMin, 196.848 },
            { Unit.KmPerHour, 3.6 },
            { Unit.MilesPerHour, 2.236932 }
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Speed value</param>
        /// <param name="unit">Speed unit</param>
        public Speed(double value, Unit unit)
        {
            SelectedUnit = unit;
            Value = value;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">Speed to copy</param>
        public Speed(Speed copy)
        {
            SelectedUnit = copy._unit;
            Value = copy.Value;
        }

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns>A copy of the object</returns>
        public object Clone()
        {
            return new Speed(this);
        }

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="speed1">First speed to compare</param>
        /// <param name="speed2">Second speed to compare</param>
        /// <returns>true if both speeds are equals, false otherwise</returns>
        public static bool operator ==(Speed speed1, Speed speed2)
        {
            // Convert both speed to the same unit
            Speed speed3 = speed2.Clone() as Speed;
            speed3.SelectedUnit = speed1.SelectedUnit;

            // Compare values
            return (speed1.Value == speed3.Value);
        }

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="speed1">First speed to compare</param>
        /// <param name="speed2">Second speed to compare</param>
        /// <returns>false if both speeds are equals, true otherwise</returns>
        public static bool operator !=(Speed speed1, Speed speed2)
        {
            return !(speed1 == speed2);
        }

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="speed1">First speed to compare</param>
        /// <param name="speed2">Second speed to compare</param>
        /// <returns>true if speed1 is greater than speed2, false otherwise</returns>
        public static bool operator >(Speed speed1, Speed speed2)
        {
            // Convert both speed to the same unit
            Speed speed3 = speed2.Clone() as Speed;
            speed3.SelectedUnit = speed1.SelectedUnit;

            // Compare values
            return (speed1.Value > speed3.Value);
        }

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="speed1">First speed to compare</param>
        /// <param name="speed2">Second speed to compare</param>
        /// <returns>true if speed1 is lower than speed2, false otherwise</returns>
        public static bool operator <(Speed speed1, Speed speed2)
        {
            // Convert both speed to the same unit
            Speed speed3 = speed2.Clone() as Speed;
            speed3.SelectedUnit = speed1.SelectedUnit;

            // Compare values
            return (speed1.Value < speed3.Value);
        }

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="speed1">First speed to compare</param>
        /// <param name="speed2">Second speed to compare</param>
        /// <returns>true if speed1 is greater than or equal to speed2, false otherwise</returns>
        public static bool operator >=(Speed speed1, Speed speed2)
        {
            // Convert both speed to the same unit
            Speed speed3 = speed2.Clone() as Speed;
            speed3.SelectedUnit = speed1.SelectedUnit;

            // Compare values
            return (speed1.Value >= speed3.Value);
        }

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="speed1">First speed to compare</param>
        /// <param name="speed2">Second speed to compare</param>
        /// <returns>true if speed1 is lower than or equal to speed2, false otherwise</returns>
        public static bool operator <=(Speed speed1, Speed speed2)
        {
            // Convert both speed to the same unit
            Speed speed3 = speed2.Clone() as Speed;
            speed3.SelectedUnit = speed1.SelectedUnit;

            // Compare values
            return (speed1.Value <= speed3.Value);
        }
    }
}
