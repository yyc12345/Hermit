using System;
using System.Collections.Generic;
using System.Text;

namespace HermitLib.Algorithm.AsymmetricEncryption {
    public abstract class AsymmetricEncryption {

        public virtual byte[] Encrypt(byte[] data) => throw new NotImplementedException();
        public virtual byte[] Decrypt(byte[] data) => throw new NotImplementedException();

        public virtual Dictionary<string,string> GenerateKey() => throw new NotImplementedException();

    }

    public class Raw : AsymmetricEncryption {

        public override byte[] Decrypt(byte[] data) {
            return data;
        }

        public override byte[] Encrypt(byte[] data) {
            return data;
        }

        public override (Dictionary<string, string> pub, Dictionary<string, string> pri) GenerateKey() {
            var cache = new Dictionary<string, string>();
            var cache2 = new Dictionary<string, string>();
            cache.Add("placeHolder", "");
            cache2.Add("placeHolder", "");
            return (cache, cache2);
        }
    }


}
