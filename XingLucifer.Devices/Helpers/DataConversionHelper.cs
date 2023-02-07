using System;
using System.Collections.Generic;
using System.Text;

namespace XingLucifer.Devices.Helpers
{
    public static class DataConversionHelper
    {
        /// <summary>
        /// byte数组转MAC地址
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <returns></returns>
        public static long BytesToMAC(this byte[] data) => data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24) | (data[4] << 32) | (data[5] << 40);

        /// <summary>
        /// MAC地址转byte数组
        /// </summary>
        /// <param name="value">长整型</param>
        /// <returns></returns>
        public static byte[] MACToBytes(this long value)
        {
            byte[] bytes = new byte[6];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(value >> (i * 8));
            }
            return bytes;
        }
    }
}
