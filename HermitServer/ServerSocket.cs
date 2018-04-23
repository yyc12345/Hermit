using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HermitLib;

namespace HermitServer {

    public class ServerSocket {

        public ServerSocket() {
            handshakeList = new Dictionary<string, ServerSocketItem>();
            normalList = new Dictionary<string, ServerSocketItem>();
            rtrList = new List<ServerSocketItem>();
        }

        #region listen

        Socket socket4;
        Socket socket6;

        bool isListening = false;

        public void StartListen() {
            if (isListening) return;

            socket4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            var endPoint4 = new IPEndPoint(IPAddress.Any, int.Parse(General.serverConfig["ipv4Port"]));
            var endPoint6 = new IPEndPoint(IPAddress.IPv6Any, int.Parse(General.serverConfig["ipv6Port"]));

            socket4.Bind(endPoint4);
            socket6.Bind(endPoint6);

            socket4.Listen(5);
            socket4.BeginAccept(new AsyncCallback(AcceptCallback), socket4);
            ConsoleAssistance.WriteLine($"[Socket] Listening on port {General.serverConfig["ipv4Port"]} for ipv4 connection.");
            socket6.Listen(5);
            socket4.BeginAccept(new AsyncCallback(AcceptCallback), socket6);
            ConsoleAssistance.WriteLine($"[Socket] Listening on port {General.serverConfig["ipv6Port"]} for ipv6 connection.");
        }

        public void StopListen() {
            if (!isListening) return;

            socket4.Close();
            ConsoleAssistance.WriteLine("[Socket] Stop listening ipv4 connection.");
            socket6.Close();
            ConsoleAssistance.WriteLine("[Socket] Stop listening ipv6 connection.");
        }

        void AcceptCallback(IAsyncResult ar) {
            Socket s = (Socket)ar.AsyncState;
            var client = s.EndAccept(ar);
            var cache = new ServerSocketItem(client, System.Guid.NewGuid().ToString());
            cache.StatusChanged += client_StatusChanged;
            ConsoleAssistance.WriteLine($"[Socket] Accept {cache.EndPoint}'s connection and its Guid is {cache.Guid}.");
            ConsoleAssistance.WriteLine($"[Socket] {cache.Guid} starts to handshake...");

            handshakeList.Add(cache.Guid, cache);

            //accept next
            s.BeginAccept(new AsyncCallback(AcceptCallback), s);
        }





        #endregion

        #region client

        object lock_clientList = new Object();

        Dictionary<string, ServerSocketItem> handshakeList;
        Dictionary<string, ServerSocketItem> normalList;
        List<ServerSocketItem> rtrList;

        private void client_StatusChanged(ServerSocketBelongTo arg1, ServerSocketBelongTo arg2, ServerSocketItem arg3) {
            if(arg1==ServerSocketBelongTo.Handshake && arg2 == ServerSocketBelongTo.Normal) {
                handshakeList.Remove(arg3.Guid);
                normalList.Add(arg3.UserName, arg3);
                return;
            }
            if (arg1 == ServerSocketBelongTo.Handshake && arg2 == ServerSocketBelongTo.RTR) {
                handshakeList.Remove(arg3.Guid);
                rtrList.Add(arg3);
                return;
            }
            if (arg1 == ServerSocketBelongTo.Normal && arg2 == ServerSocketBelongTo.RTR) {
                normalList.Remove(arg3.UserName);
                rtrList.Add(arg3);
                return;
            }
            if (arg1 == ServerSocketBelongTo.Normal && arg2 == ServerSocketBelongTo.Close) {
                normalList.Remove(arg3.UserName);
                return;
            }

            throw new ArgumentException();
        }

        

        #endregion

    }

}
