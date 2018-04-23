using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HermitLib;
using HermitLib.Algorithm.AsymmetricEncryption;
using HermitLib.Algorithm.SymmetricEncryption;

namespace HermitServer {

    public class ServerSocketItem {

        public ServerSocketItem(Socket s, string Guid) {
            this.Guid = Guid;
            client = s;
            Status = ServerSocketStatus.WaitHandshake1;

            
        }

        public string Guid;
        public string UserName;
        public string EndPoint {
            get {
                return client.RemoteEndPoint.ToString();
            }
        }

        Socket client;
        ServerSocketStatus Status { get; set; }
        public event Action<ServerSocketBelongTo, ServerSocketBelongTo, ServerSocketItem> StatusChanged;
        void OnNextStatus(ServerSocketBelongTo from, ServerSocketBelongTo to) {
            StatusChanged?.Invoke(from, to, this);
        }

        List<SymmetricEncryption> symmetricEncryptionList;
        AsymmetricEncryption asymmetricEncryptionCore;

        public void Close() {
            client.Close();
        }

    }

    public enum ServerSocketStatus {
        WaitHandshake1,
        WaitHandshake3,
        Normal,
        RTR,
        Closed
    }

    public enum ServerSocketBelongTo {
        Handshake,
        Normal,
        RTR,
        Close
    }

}
