using System;
using System.Collections.Generic;
using System.Text;

namespace OpenVario
{
    /// <summary>
    /// Represent a value exchanged using BLE GATT protocol
    /// </summary>
    public class BleValue
    {
        /// <summary>
        /// Type of a BLE value
        /// </summary>
        public enum BleType
        {
            /// <summary>
            /// 8 bits unsigned integer
            /// </summary>
            UInt8,
            /// <summary>
            /// 8 bits signed integer
            /// </summary>
            Int8,
            /// <summary>
            /// 16 bits unsigned integer
            /// </summary>
            UInt16,
            /// <summary>
            /// 16 bits signed integer
            /// </summary>
            Int16,
            /// <summary>
            /// 32 bits unsigned integer
            /// </summary>
            UInt32,
            /// <summary>
            /// 32 bits signed integer
            /// </summary>
            Int32,
            /// <summary>
            /// 64 bits unsigned integer
            /// </summary>
            UInt64,
            /// <summary>
            /// 64 bits signed integer
            /// </summary>
            Int64,
            /// <summary>
            /// 32 bits floating point value
            /// </summary>
            Float32,
            /// <summary>
            /// 64 bits floating point value
            /// </summary>
            Float64,
            /// <summary>
            /// Boolean
            /// </summary>
            Bool,
            /// <summary>
            /// String
            /// </summary>
            String,
            /// <summary>
            /// Array of bytes
            /// </summary>
            Array
        }

        /// <summary>
        /// Type of the BLE value
        /// </summary>
        public BleType Type { get; set; }

        /// <summary>
        /// BLE value as an array of bytes
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// Bit converter needed to convert the value to an array of bytes
        /// </summary>
        private BitConverterEx _bit_converter = new BitConverterEx(BitConverterEx.Endianess.Little);


        /// <summary>
        /// Constructor
        /// </summary>
        public BleValue()
        {
            Type = BleType.Array;
            Value = new byte[] { };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(byte val)
        {
            Type = BleType.UInt8;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(sbyte val)
        {
            Type = BleType.Int8;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(UInt16 val)
        {
            Type = BleType.UInt16;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(Int16 val)
        {
            Type = BleType.Int16;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(UInt32 val)
        {
            Type = BleType.UInt32;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(Int32 val)
        {
            Type = BleType.Int32;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(UInt64 val)
        {
            Type = BleType.UInt64;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(Int64 val)
        {
            Type = BleType.Int64;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(float val)
        {
            Type = BleType.Float32;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(double val)
        {
            Type = BleType.Float64;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(bool val)
        {
            Type = BleType.Bool;
            Value = _bit_converter.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(string val)
        {
            Type = BleType.String;
            Value = Encoding.UTF8.GetBytes(val);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="val">BLE value</param>
        public BleValue(byte[] val)
        {
            Type = BleType.Array;
            Value = val.Clone() as byte[];
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as an 8 bits unsigned integer</returns>
        public byte ToUInt8()
        {
            return _bit_converter.ToByte(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as an 8 bits signed integer</returns>
        public sbyte ToInt8()
        {
            return _bit_converter.ToSByte(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 16 bits unsigned integer</returns>
        public UInt16 ToUInt16()
        {
            return _bit_converter.ToUInt16(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 16 bits signed integer</returns>
        public Int16 ToInt16()
        {
            return _bit_converter.ToInt16(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 32 bits unsigned integer</returns>
        public UInt32 ToUInt32()
        {
            return _bit_converter.ToUInt32(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 32 bits signed integer</returns>
        public Int32 ToInt32()
        {
            return _bit_converter.ToInt32(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 64 bits unsigned integer</returns>
        public UInt64 ToUInt64()
        {
            return _bit_converter.ToUInt64(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 64 bits signed integer</returns>
        public Int64 ToInt64()
        {
            return _bit_converter.ToInt64(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 32 bits floating point value</returns>
        public float ToFloat32()
        {
            return _bit_converter.ToSingle(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a 64 bits floating point value</returns>
        public double ToFloat64()
        {
            return _bit_converter.ToDouble(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a boolean</returns>
        public bool ToBoolean()
        {
            return _bit_converter.ToBoolean(Value);
        }

        /// <summary>
        /// Get the BLE value
        /// </summary>
        /// <returns>BLE value as a string</returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(Value);
        }
    }
}
