using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

namespace com.ahyim.planet
{
    public class DebugConnection : NetworkConnection
    {
        public override void TransportReceive(byte[] bytes, int numBytes, int channelId)
        {
            StringBuilder msg = new StringBuilder();
            for (int i = 0; i < numBytes; i++)
            {
                var s = String.Format("{0:X2}", bytes[i]);
                msg.Append(s);
                
            }
            UnityEngine.Debug.LogError("TransportReceive h:" + hostId + " con:" + connectionId + " bytes:" + numBytes + " " + msg);

            HandleBytes(bytes, numBytes, channelId);
        }

        public override bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
        {
            StringBuilder msg = new StringBuilder();
            for (int i = 0; i < numBytes; i++)
            {
                var s = String.Format("{0:X2}", bytes[i]);
                msg.Append(s);

            }
            UnityEngine.Debug.LogError("TransportSend h:" + hostId + " con:" + connectionId + " bytes:" + numBytes + " " + msg);

            return base.TransportSend(bytes, numBytes, channelId, out error);
        }
    }
}
