using System;

namespace HermitLib.DataStruct {
    public static class DataLength {

        public static readonly int Handshake1HeadLength = 4 + 4 + 8 + 32;

        public static readonly int Handshake2HeadLength = 4 + 4 + 8 + 32;

        public static readonly int Handshake3HeadLength = 4 + 4 + 8 + 32;

        public static readonly int Handshake4HeadLength = 4 + 4 + 8 + 32;

        public static readonly int NormalHeadLength = 4 + 4 + 8 + 1;
    }



    public class EncryptionJson {
        public bool useEntropy { get; set; }
        public bool useObfuscation { get; set; }
        public EncryptionMethod[] encryptionMethod { get; set; }
    }

    public class EncryptionMethod {
        public string name { get; set; }
        public int rotor { get; set; }
        public bool package { get; set; }
        public string[] keys { get; set; }
        public EncryptionParameter[] parameter { get; set; }
    }

    public class EncryptionParameter {
        public string name { get; set; }
        public string value { get; set; }
    }

}
