using System;
using System.Collections.Generic;
using System.Text;

namespace OpenVario
{

    /// <summary>
    /// Bit converter with endianess awareness
    /// </summary>
    class BitConverterEx
    {
        public enum Endianess
        {
            Little,
            Big
        }

        public bool IsLittleEndian { get { return (_endianess == Endianess.Little); } }
        private readonly Endianess _endianess;

        public BitConverterEx(Endianess endianess)
        {
            _endianess = endianess;
        }

        public byte[] GetBytes(byte val)
        {
            return new byte[] { val };
        }

        public byte[] GetBytes(sbyte val)
        {
            return new byte[] { (byte)val };
        }

        public byte[] GetBytes(UInt16 val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(Int16 val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(UInt32 val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(Int32 val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(UInt64 val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(Int64 val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(float val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(double val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        public byte[] GetBytes(bool val)
        {
            byte[] ret = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(ret);
            }
            return ret;
        }


        public byte ToByte(byte[] val)
        {
            if (val.Length != 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            return val[0];
        }

        public sbyte ToSByte(byte[] val)
        {
            if (val.Length != 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            return (sbyte)val[0];
        }

        public UInt16 ToUInt16(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            UInt16 ret = BitConverter.ToUInt16(val, 0);
            return ret;
        }

        public Int16 ToInt16(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            Int16 ret = BitConverter.ToInt16(val, 0);
            return ret;
        }

        public UInt32 ToUInt32(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            UInt32 ret = BitConverter.ToUInt32(val, 0);
            return ret;
        }

        public Int32 ToInt32(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            Int32 ret = BitConverter.ToInt32(val, 0);
            return ret;
        }

        public UInt64 ToUInt64(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            UInt64 ret = BitConverter.ToUInt64(val, 0);
            return ret;
        }

        public Int64 ToInt64(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            Int64 ret = BitConverter.ToInt64(val, 0);
            return ret;
        }

        public float ToSingle(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            float ret = BitConverter.ToSingle(val, 0);
            return ret;
        }

        public double ToDouble(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            double ret = BitConverter.ToDouble(val, 0);
            return ret;
        }

        public bool ToBoolean(byte[] val)
        {
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(val);
            }
            bool ret = BitConverter.ToBoolean(val, 0);
            return ret;
        }
    }
}
