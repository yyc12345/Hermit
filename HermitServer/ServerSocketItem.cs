using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HermitLib;
using HermitLib.Algorithm.AsymmetricEncryption;
using HermitLib.Algorithm.SymmetricEncryption;
using HermitLib.Algorithm;
using HermitLib.DataStruct;
using System.Threading.Tasks;
using System.Threading;

namespace HermitServer {

    public class ServerSocketItem {

        public ServerSocketItem(Socket s, string Guid) {
            cacheData = new List<byte[]>();

            this.Guid = Guid;
            client = s;
            Status = ServerSocketStatus.WaitHandshake1;

            buffer = new byte[DataLength.Handshake1HeadLength];
            s.BeginReceive(buffer, 0, DataLength.Handshake1HeadLength, SocketFlags.None, new AsyncCallback(ReceiveData), s);
        }

        //user info
        public string Guid;
        public string UserName;
        public string EndPoint {
            get {
                return client.RemoteEndPoint.ToString();
            }
        }

        //socket
        Socket client;
        ServerSocketStatus Status { get; set; }
        byte[] buffer;
        List<byte[]> cacheData;
        public event Action<ServerSocketBelongTo, ServerSocketItem> StatusChanged;
        void OnChangeStatus(ServerSocketBelongTo to) {
            StatusChanged?.Invoke(to, this);
        }

        //connect to manager
        public ServerSocketBelongTo BelongTo { get; set; }
        public int TimeClips { get; set; }

        //encryption
        List<IEncryptionStackCore> symmetricEncryptionList;
        AsymmetricEncryption asymmetricEncryptionCore;


        void ReceiveData(IAsyncResult result) {
            Socket ts = (Socket)result.AsyncState;
            ts.EndReceive(result);

            //receive head. check status and process it
            byte[] data;
            switch (this.Status) {
                case ServerSocketStatus.WaitHandshake1:
                    data = this.asymmetricEncryptionCore.Decrypt(this.buffer);
                    break;
                case ServerSocketStatus.WaitHandshake3:
                    data = this.asymmetricEncryptionCore.Decrypt(this.buffer);
                    break;
                case ServerSocketStatus.Normal:
                    var isRight = false;
                    data = buffer;
                    foreach (var item in symmetricEncryptionList) {
                        (isRight, data) = item.Decrypt(data);
                        if (!isRight) {
                            //illegal data. rtr
                            OnRTR();
                            return;
                        }
                    }
                    break;
                case ServerSocketStatus.RTR:
                    //pass
                    goto finish_process;
                case ServerSocketStatus.Closed:
                    //pass
                    goto finish_process;
                default:
                    throw new ArgumentException();
            }

            finish_process:


            //reset time clips because of the message
            TimeClips = 3;
        }

        void WriteDataEx(byte[] data) {
            //if (this.Status != ServerSocketStatus.Normal) return;

            try {
                this.client.Send(data);
            } catch (Exception) {
                //error. close client
                this.Close();
            }
        }

        void WriteDataRTR() {
            Task.Run(() => {
                var rnd = new HermitLib.Random();

                while (this.Status == ServerSocketStatus.RTR) {
                    var cache = new byte[rnd.Next(64, 1024)];
                    rnd.NextBytes(ref cache);
                    WriteDataEx(cache);

                    //30s-60s
                    Thread.Sleep(rnd.Next(10 * 1000, 60 * 1000));
                }
            });
        }

        void OnRTR() {
            //change status
            this.Status = ServerSocketStatus.RTR;
            OnChangeStatus(ServerSocketBelongTo.RTR);

            //start writing random data
            WriteDataRTR();
        }

        public void Close() {
            this.Status = ServerSocketStatus.Closed;
            client.Close();
            OnChangeStatus(ServerSocketBelongTo.Close);
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
