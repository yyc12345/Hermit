using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HermitLib;

namespace HermitServer {
    public class ServerSocketItem {

        public ServerSocketItem(Socket s, string Guid) {
            this.Guid = Guid;
            client = s;
        }

        public string Guid;
        public string EndPoint {
            get {
                return client.RemoteEndPoint.ToString();
            }
        }


        Socket client;

    }
}
