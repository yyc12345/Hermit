using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto;

namespace HermitLib.Algorithm.Hash {

    public enum HashType {
        MD5,
        SHA256,
        SHA512,
        SHA3,
        CRC32
    }


    public class Hash {

        /// <summary>
        /// the block's length of computing hash with stream
        /// </summary>
        private const int StreamHashComputingBlockSize = 4096;

        #region normal hash

        public static byte[] GetHashRaw(byte[] data, IDigest digest) {
            try {
                digest.BlockUpdate(data, 0, data.Length);

                byte[] result = new byte[digest.GetDigestSize()];
                digest.DoFinal(result, 0);
                return result;
            } catch (Exception) {
                return null;
            }
        }

        public static byte[] GetHashRaw(Stream data, IDigest digest) {
            try {
                //update data
                var buffer = new byte[StreamHashComputingBlockSize];
                long count = data.Length / StreamHashComputingBlockSize;
                var remainedData = new byte[data.Length % StreamHashComputingBlockSize];
                for (int i = 0; i < count; i++) {
                    data.Read(buffer, 0, StreamHashComputingBlockSize);
                    digest.BlockUpdate(buffer, 0, StreamHashComputingBlockSize);
                }
                data.Read(remainedData, 0, remainedData.Length);
                digest.BlockUpdate(remainedData, 0, remainedData.Length);

                //get result
                byte[] result = new byte[digest.GetDigestSize()];
                digest.DoFinal(result, 0);
                return result;
            } catch (Exception) {
                return null;
            }
        }

        #endregion

        #region crc32

        public static byte[] GetCRC32HashRaw(Stream data) {

            try {
                var cache = new CRC32();
                return cache.ComputeHash(data);

            } catch (Exception) {
                return null;
            }
        }

        public static byte[] GetCRC32HashRaw(byte[] data) {
            try {
                var cache = new CRC32();
                return cache.ComputeHash(data);

            } catch (Exception) {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// get hash
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="origin">the type of data</param>
        /// <param name="hashType">the hash type which you want to use</param>
        /// <returns></returns>
        public static byte[] GetHash(dynamic data, HashType hashType) {

            //get origin data
            byte[] input = null;
            Stream inputStream = null;
            switch (data) {
                case FilePathBuilder file:
                    inputStream = new FileStream(file.Path(), FileMode.Open, FileAccess.Read);
                    break;
                case byte[] bytes:
                    input = bytes;
                    break;
                case string words:
                    input = Information.UniversalEncoding.GetBytes(words);
                    break;
                case Stream stream:
                    inputStream = stream;
                    break;
                default:
                    throw new ArgumentException();
            }

            //calculate hash
            byte[] result = null;
            switch (hashType) {
                case HashType.MD5:
                case HashType.SHA256:
                case HashType.SHA512:
                case HashType.SHA3:
                    result = input != null ? GetHashRaw(input, GetHashClass(hashType)) : GetHashRaw(inputStream, GetHashClass(hashType));
                    break;
                case HashType.CRC32:
                    result = input != null ? GetCRC32HashRaw(input) : GetCRC32HashRaw(inputStream);
                    break;
                default:
                    throw new ArgumentException();
            }

            //close stream
            if (inputStream != null)
                inputStream.Close();

            return result;

        }

        /// <summary>
        /// return the length of hash which you selected with Binary length
        /// </summary>
        /// <param name="hashType"></param>
        /// <returns></returns>
        public static int GetHashLength(HashType hashType) {
            switch (hashType) {
                case HashType.MD5:
                case HashType.SHA256:
                case HashType.SHA512:
                case HashType.SHA3:
                    return GetHashClass(hashType).GetDigestSize();
                case HashType.CRC32:
                    return 32;
                default:
                    throw new ArgumentException();
            }
        
        }

        public static IDigest GetHashClass(HashType hashType) {
            switch (hashType) {
                case HashType.MD5:
                    return new MD5Digest();
                case HashType.SHA256:
                    return new Sha256Digest();
                case HashType.SHA512:
                    return new Sha512Digest();
                case HashType.SHA3:
                    return new Sha3Digest();
                case HashType.CRC32:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentException();
            }
        }

    }

}
