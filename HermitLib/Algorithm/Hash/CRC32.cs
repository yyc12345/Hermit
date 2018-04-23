using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermitLib.Algorithm.Hash {

    //origin: Github ballance/UsenetPlayer/UsenetPlayer.dll/Decoders/YencDecoder/CRC32.cs
    //License: MIT
    /// <summary>
    /// compute crc32 class
    /// </summary>
    public class CRC32 : System.Security.Cryptography.HashAlgorithm {

        public const uint DEFAULT_POLY = 0x04C11DB7; //0xEDB88320

        private const uint ONES = 0xffffffff;

        private readonly uint[] _table;
        private uint _crc;

        public CRC32()
            : this(DEFAULT_POLY)
        {
        }

        public CRC32(uint poly) {
            HashSizeValue = 32;
            _table = BuildCrc32Table(poly);

            Initialize();
        }

        private static uint[] BuildCrc32Table(uint poly) {
            var table = new uint[256];


            for (var i = 0; i < 256; i++) {
                var crc = (uint)i;
                for (var j = 8; j > 0; j--) {
                    if ((crc & 1) == 1) {
                        crc = (crc >> 1) ^ poly;
                    } else {
                        crc >>= 1;
                    }
                }

                table[i] = crc;
            }

            return table;
        }

        public override void Initialize() {
            _crc = ONES;
        }

        protected override void HashCore(byte[] buffer, int offset, int count) {
            for (var i = offset; i < count; i++) {
                ulong ptr = (_crc & 0xFF) ^ buffer[i];
                _crc >>= 8;
                _crc ^= _table[ptr];
            }
        }

        protected override byte[] HashFinal() {
            var finalHash = new byte[4];
            ulong finalCrc = _crc ^ ONES;

            finalHash[0] = (byte)((finalCrc >> 0) & 0xFF);
            finalHash[1] = (byte)((finalCrc >> 8) & 0xFF);
            finalHash[2] = (byte)((finalCrc >> 16) & 0xFF);
            finalHash[3] = (byte)((finalCrc >> 24) & 0xFF);

            return finalHash;
        }

        public new byte[] ComputeHash(Stream stream) {
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, 4096)) > 0) {
                HashCore(buffer, 0, bytesRead);
            }

            return HashFinal();
        }

    }
}
