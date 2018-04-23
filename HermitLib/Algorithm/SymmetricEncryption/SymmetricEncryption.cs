using System;
using System.Collections.Generic;
using System.Text;

namespace HermitLib.Algorithm.SymmetricEncryption {
    public abstract class SymmetricEncryption {

        public virtual bool SupportAEAD {
            get { throw new NotImplementedException(); }
        }

        public virtual bool SupportRotor {
            get { throw new NotImplementedException(); }
        }

        public virtual bool SupportPackage {
            get { throw new NotImplementedException(); }
        }

        public virtual byte[] Encrypt(byte[] data) => throw new NotImplementedException();
        public virtual byte[] Decrypt(byte[] data) => throw new NotImplementedException();

        public virtual Dictionary<string, string> GenerateKey() => throw new NotImplementedException();

        public static SymmetricEncryption GetSymmetricEncryption(string name, Dictionary<string, string> parameter) {

        }

    }

    public class Raw : SymmetricEncryption {

        public override bool SupportAEAD {
            get { return true; }
        }

        public override bool SupportRotor {
            get { return true; }
        }

        public override bool SupportPackage {
            get { return true; }
        }

        public override byte[] Decrypt(byte[] data) {
            return data;
        }

        public override byte[] Encrypt(byte[] data) {
            return data;
        }

        public override Dictionary<string, string> GenerateKey() {
            var cache = new Dictionary<string, string>();
            cache.Add("placeHolder", "");
            return cache;
        }
    }


}
