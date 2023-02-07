using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using XingLucifer.Devices.Models;

namespace XingLucifer.Devices
{
    public class DeviceBroadcast
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public DeviceBroadcast(string ip)
        {
            _socket.SendTimeout = 1000;
            _socket.EnableBroadcast= true;
            _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), 22345));
        }

        public bool SendTo(byte[] bytes, EndPoint endPoint)
        {
            try
            {
                _socket.SendTo(bytes, SocketFlags.None, endPoint);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检测是否可读
        /// </summary>
        /// <returns></returns>
        public bool Poll() => _socket.Poll(50, SelectMode.SelectRead);

        public FromDataModel ReceiveFrom()
        {
            FromDataModel fromData = new FromDataModel();
            byte[] buffer = new byte[512];
            EndPoint point = new IPEndPoint(0, 0);
            try
            {
                int length = _socket.ReceiveFrom(buffer, SocketFlags.None, ref point);
                fromData.Buffer = buffer.Take(length).ToArray();
                fromData.Point = point;
                
            }
            catch (Exception)
            {
                return null;
            }
            return fromData;
        }
    }
}
