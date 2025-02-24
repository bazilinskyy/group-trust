﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;


public static class NetworkUtils
{
    static NetworkUtils()
    {
        stopwatch.Start();
    }


    public static Stopwatch stopwatch = new();


    public static float UInt32ToFloat(uint value)
    {
        return new UIntFloat {intValue = value}.floatValue;
    }


    public static uint FloatToUInt32(float value)
    {
        return new UIntFloat {floatValue = value}.intValue;
    }


    public static Color32 Uint32ToColor32(uint value)
    {
        return new Color32((byte) (value & 0xff), (byte) ((value >> 8) & 0xff), (byte) ((value >> 16) & 0xff), (byte) ((value >> 24) & 0xff));
    }


    public static uint Color32ToUInt32(Color32 value)
    {
        return value.r | (uint) (value.g << 8) | (uint) (value.b << 16) | (uint) (value.a << 24);
    }


    public static double DoubleToUInt64(ulong value)
    {
        return new ULongDouble {longValue = value}.doubleValue;
    }


    public static ulong UInt64ToDouble(double value)
    {
        return new ULongDouble {doubleValue = value}.longValue;
    }


    public static int CalculateRequiredBits(long min, long max)
    {
        return min == max ? 0 : (int) Math.Log(max - min, 2) + 1;
    }


    public static void MemCopy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
    {
        // TODO : Right now we cannot have unsafe code and be able to debug the project
        // at the same time because the generated project file doesn't add the allow unsafe
        // block, so do the
        for (var i = 0; i < count; ++i)
        {
            dst[dstIndex++] = src[srcIndex++];
        }
    }


    public static int MemCmp(byte[] a, int aIndex, byte[] b, int bIndex, int count)
    {
        // TODO : Right now we cannot have unsafe code and be able to debug the project
        // at the same time because the generated project file doesn't add the allow unsafe
        // block, so do the
        for (var i = 0; i < count; ++i)
        {
            var diff = b[bIndex++] - a[aIndex++];

            if (diff != 0)
            {
                return diff;
            }
        }

        return 0;
    }


    public static uint SimpleHash(byte[] array, int count)
    {
        uint hash = 0;

        for (var i = 0; i < count; i++)
        {
            hash = hash * 179 + array[i] + 1;
        }

        return hash;
    }


    public static uint SimpleHashStreaming(uint old_hash, uint value)
    {
        return old_hash * 179 + value + 1;
    }


    public static List<string> GetLocalInterfaceAddresses()
    {
        // Useful to print 'best guess' for local ip, so...
        var addresses = new List<string>();

        foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.OperationalStatus != OperationalStatus.Up)
            {
                continue;
            }

            var type = item.NetworkInterfaceType;

            if (type != NetworkInterfaceType.Ethernet && type != NetworkInterfaceType.Wireless80211)
            {
                continue;
            }

            try
            {
                foreach (var addr in item.GetIPProperties().UnicastAddresses)
                {
                    if (addr.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    addresses.Add(addr.Address.ToString());
                }
            }
            catch (Exception e)
            {
                // NOTE : For some reason this can throw marshal exception in the interop 
                // to native network code on some computers (when running player but not in editor)?
                Debug.Log("Error " + e.Message + " while getting IP properties for " + item.Description);
            }
        }

        return addresses;
    }


    public struct Timer
    {
        public long remaining;


        public Timer(float seconds)
        {
            m_Interval = (long) (seconds * Stopwatch.Frequency);
            m_LastUpdate = stopwatch.ElapsedTicks;

            // By default timer triggers first time it is checked
            remaining = -1;
        }


        public void Reset()
        {
            remaining = m_Interval;
        }


        public void SetRemaining(float seconds)
        {
            remaining = (long) (seconds * Stopwatch.Frequency);
        }


        public void AddRemaining(float seconds)
        {
            remaining += (long) (seconds * Stopwatch.Frequency);
        }


        public bool Update()
        {
            remaining -= stopwatch.ElapsedTicks - m_LastUpdate;
            m_LastUpdate = stopwatch.ElapsedTicks;

            if (remaining < 0)
            {
                remaining = remaining < -m_Interval ? m_Interval : remaining + m_Interval;

                return true;
            }

            return false;
        }


        private readonly long m_Interval;
        private long m_LastUpdate;
    }


    [StructLayout(LayoutKind.Explicit)]
    private struct UIntFloat
    {
        [FieldOffset(0)]
        public float floatValue;
        [FieldOffset(0)]
        public uint intValue;
    }


    [StructLayout(LayoutKind.Explicit)]
    private struct ULongDouble
    {
        [FieldOffset(0)]
        public double doubleValue;
        [FieldOffset(0)]
        public ulong longValue;
    }
}


internal class ByteArrayComp : IEqualityComparer<byte[]>, IComparer<byte[]>
{
    public static readonly ByteArrayComp instance = new();


    public int Compare(byte[] x, byte[] y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentNullException("Trying to compare array with null");
        }

        var xl = x.Length;
        var yl = y.Length;

        if (xl != yl)
        {
            return yl - xl;
        }

        for (var i = 0; i < xl; i++)
        {
            var d = y[i] - x[i];

            if (d != 0)
            {
                return d;
            }
        }

        return 0;
    }


    public bool Equals(byte[] x, byte[] y)
    {
        return Compare(x, y) == 0;
    }


    public int GetHashCode(byte[] x)
    {
        if (x == null)
        {
            throw new ArgumentNullException("Trying to get hash of null");
        }

        var xl = x.Length;

        if (xl >= 4)
        {
            return x[0] + (x[1] << 8) + (x[2] << 16) + (x[3] << 24);
        }

        return 0;
    }
}