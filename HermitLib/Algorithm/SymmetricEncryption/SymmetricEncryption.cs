using System;
using System.Collections.Generic;
using System.Text;
using HermitLib.DataStruct;

namespace HermitLib.Algorithm.SymmetricEncryption {
    public abstract class SymmetricEncryption {

        public virtual void ApplyParameter(List<EncryptionParameter> parameter) => throw new NotImplementedException();

        public virtual bool SupportAEAD {
            get { throw new NotImplementedException(); }
        }

        public virtual bool SupportRotor {
            get { throw new NotImplementedException(); }
        }

        public virtual bool SupportPackage {
            get { throw new NotImplementedException(); }
        }

        public virtual int KeyLength {
            get { throw new NotImplementedException(); }
        }

        public virtual byte[] Encrypt(byte[] data) => throw new NotImplementedException();
        public virtual (bool isRight, byte[] data) Decrypt(byte[] data) => throw new NotImplementedException();

        public virtual void FlushKey(string newKey) => throw new NotImplementedException();
        public virtual string RearrangeKey(List<RotorKeyReaderStructure> arrange, List<string> keyList) => throw new NotImplementedException();
        public virtual string CombineKeyWithPackage(string key, byte[] hash) => throw new NotImplementedException();

        public virtual string GenerateKey() => throw new NotImplementedException();

    }

    public class Raw : SymmetricEncryption {

        public override void ApplyParameter(List<EncryptionParameter> parameter) {
            return;
        }

        public override bool SupportAEAD {
            get { return true; }
        }

        public override bool SupportRotor {
            get { return true; }
        }

        public override bool SupportPackage {
            get { return true; }
        }

        public override int KeyLength {
            get { return 1024; }
        }

        public override (bool isRight, byte[] data) Decrypt(byte[] data) {
            return (true, data);
        }

        public override byte[] Encrypt(byte[] data) {
            return data;
        }

        public override void FlushKey(string newKey) {
            return;
        }

        public override string RearrangeKey(List<RotorKeyReaderStructure> arrange, List<string> keyList) {
            return "";
        }

        public override string CombineKeyWithPackage(string key, byte[] hash) {
            return key;
        }

        public override string GenerateKey() {
            return "";
        }
    }


}
