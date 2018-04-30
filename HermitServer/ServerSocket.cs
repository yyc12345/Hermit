using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HermitLib;
using System.Threading;

namespace HermitServer {

    public class ServerSocket {

        public ServerSocket() {
            handshakeList = new Dictionary<string, ServerSocketItem>();
            normalList = new Dictionary<string, ServerSocketItem>();
            rtrList = new List<ServerSocketItem>();

            //start cleaner
            var cache = new Thread(this.clientListCleaner);
            cache.IsBackground = true;
            cache.Start();
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

            cache.TimeClips = 3;
            handshakeList.Add(cache.Guid, cache);
            cache.BelongTo = ServerSocketBelongTo.Handshake;
            OnUserCountChanged();

            //accept next
            s.BeginAccept(new AsyncCallback(AcceptCallback), s);
        }





        #endregion

        #region client

        object lock_clientList = new Object();

        Dictionary<string, ServerSocketItem> handshakeList;
        Dictionary<string, ServerSocketItem> normalList;
        List<ServerSocketItem> rtrList;

        private void client_StatusChanged(ServerSocketBelongTo arg2, ServerSocketItem arg3) {
            lock (lock_clientList) {
                if (arg3.BelongTo == ServerSocketBelongTo.Handshake && arg2 == ServerSocketBelongTo.Normal) {
                    handshakeList.Remove(arg3.Guid);
                    normalList.Add(arg3.UserName, arg3);
                    arg3.BelongTo = arg2;
                    return;
                }
                if (arg3.BelongTo == ServerSocketBelongTo.Handshake && arg2 == ServerSocketBelongTo.RTR) {
                    handshakeList.Remove(arg3.Guid);
                    rtrList.Add(arg3);
                    arg3.BelongTo = arg2;
                    return;
                }
                if (arg3.BelongTo == ServerSocketBelongTo.Normal && arg2 == ServerSocketBelongTo.RTR) {
                    normalList.Remove(arg3.UserName);
                    rtrList.Add(arg3);
                    arg3.BelongTo = arg2;
                    return;
                }
                if (arg2 == ServerSocketBelongTo.Close) {
                    switch (arg3.BelongTo) {
                        case ServerSocketBelongTo.Handshake:
                            normalList.Remove(arg3.Guid);
                            break;
                        case ServerSocketBelongTo.Normal:
                            normalList.Remove(arg3.UserName);
                            break;
                        case ServerSocketBelongTo.RTR:
                            rtrList.Remove(arg3);
                            break;
                        case ServerSocketBelongTo.Close:
                            break;
                        default:
                            throw new ArgumentException();
                    }
                    arg3.BelongTo = arg2;
                    OnUserCountChanged();
                    return;
                }

                //give up
                return;
            }
        }

        public (int handshakeUser, int normalUser, int rtrUser) GetUserCount() {
            lock (lock_clientList) {
                return (handshakeList.Count, normalList.Count, rtrList.Count);
            }
        }

        private void OnUserCountChanged() {
            (int a, int b, int c) = General.serverSocket.GetUserCount();
            if (a + b + c >= int.Parse(General.serverConfig["maxUser"])) {
                this.StopListen();
            } else {
                this.StartListen();
            }
        }

        void clientListCleaner() {
            var rnd = new HermitLib.Random();

            while (true) {
                //1min-5min
                Thread.Sleep(rnd.Next(1 * 60 * 1000, 5 * 60 * 1000));

                lock (lock_clientList) {
                    //time clips --
                    foreach (var item in handshakeList.Values) {
                        item.TimeClips--;
                    }
                    foreach (var item in normalList.Values) {
                        item.TimeClips--;
                    }
                    foreach (var item in rtrList) {
                        item.TimeClips--;
                    }

                    //evaluate client
                    foreach (var item in handshakeList.Values) {
                        if(item.TimeClips < 0) {
                            item.Close();
                        }
                    }
                    foreach (var item in normalList.Values) {
                        if (item.TimeClips < 0) {
                            item.Close();
                        }
                    }
                    foreach (var item in rtrList) {
                        if (item.TimeClips < 0) {
                            item.Close();
                        }
                    }
                }
            }
        }

        public void Close() {
            lock (lock_clientList) {
                foreach (var item in handshakeList.Values) {
                    item.Close();
                }
                foreach (var item in normalList.Values) {
                    item.Close();
                }
                foreach (var item in rtrList) {
                    item.Close();
                }
            }
        }

        #endregion

    }

}
