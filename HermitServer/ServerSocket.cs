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
            ConsoleAssistance.WriteLine($"[Socket] Accept {cache.EndPoint}'s connection and its Guid is {cache.Guid}.");
            ConsoleAssistance.WriteLine($"[Socket] {cache.Guid} starts to handshake...");

            handshakeList.Add(cache.Guid, cache);

            //accept next
            s.BeginAccept(new AsyncCallback(AcceptCallback), s);
        }

        #endregion

        #region client

        Dictionary<string, ServerSocketItem> handshakeList;
        Dictionary<string, ServerSocketItem> normalList;
        List<ServerSocketItem> rtrList;



        #endregion

    }

}
