using System;
using System.Collections.Generic;
using System.Text;
using HermitLib.DataStruct;
using HermitLib.Algorithm.SymmetricEncryption;

namespace HermitLib.Algorithm {

    public interface IEncryptionStackCore {
        void Next(byte[] hash);
        byte[] Encrypt(byte[] data);
        (bool isRight, byte[] data) Decrypt(byte[] data);
    }


    /// <summary>
    /// the implementation of rotor and package key
    /// </summary>
    public class EncryptionStackCore<T> : IEncryptionStackCore where T : HermitLib.Algorithm.SymmetricEncryption.SymmetricEncryption, new() {

        public EncryptionStackCore(bool usePackage, int rotorCount, List<string> keys, List<EncryptionParameter> parameter) {
            //get core encryption
            core = new T();
            core.ApplyParameter(parameter);
            this.keys = keys;

            //set core rotor
            if (rotorCount != 1)
                coreRotor = new RotorIteratorBuilder(rotorCount, core.KeyLength).RotorReadLengthNext().GetEnumerator();

            //256 bit
            var package = new byte[32];
            for (int i = 0; i < 32; i++) {
                package[0] = 0;
            }
            Next(package);

        }

        bool usePackage;
        int rotorCount;
        string currentKey;

        HermitLib.Algorithm.SymmetricEncryption.SymmetricEncryption core;
        List<string> keys;
        IEnumerator<List<int>> coreRotor;

        #region data process

        public (bool isRight, byte[] data) Decrypt(byte[] data) {
            core.FlushKey(this.currentKey);
            return core.Decrypt(data);
        }

        public byte[] Encrypt(byte[] data) {
            core.FlushKey(this.currentKey);
            return core.Encrypt(data);
        }

        public void Next(byte[] hash) {
            string newKey = keys[0];

            if (rotorCount == 1) goto process_package;

            //get new length
            coreRotor.MoveNext();

            //initialize the rotor
            var rotorArrangement = new List<RotorKeyReaderStructure>();
            for (int i = 0; i < rotorCount; i++) {
                rotorArrangement.Add(new RotorKeyReaderStructure() { Position = 0 });
            }
            //flush
            FlushKeyLength(rotorArrangement);
            FlushKeyPosition(rotorArrangement);

            //apply arrange
            newKey = core.RearrangeKey(rotorArrangement, this.keys);

            process_package:
            //apply package
            if (usePackage) newKey = core.CombineKeyWithPackage(newKey, hash);

            //set key
            this.currentKey = newKey;
        }

        #endregion

        #region rotor process

        private void FlushKeyLength(List<RotorKeyReaderStructure> rotorArrangement) {

            var cache = coreRotor.Current;
            for (int i = 0; i < rotorArrangement.Count; i++) {
                rotorArrangement[i] = new RotorKeyReaderStructure() { Position = rotorArrangement[i].Position, Length = cache[i] };
            }

        }

        private void FlushKeyPosition(List<RotorKeyReaderStructure> rotorArrangement) {

            //add 1
            rotorArrangement[0] = new RotorKeyReaderStructure() { Position = rotorArrangement[0].Position + 1, Length = rotorArrangement[0].Length };
            for (int i = 0; i < rotorArrangement.Count; i++) {
                //check overflow
                if (rotorArrangement[i].Position == core.KeyLength) {
                    //set this item's position is 0
                    rotorArrangement[i] = new RotorKeyReaderStructure() { Position = 0, Length = rotorArrangement[i].Length };
                    if (i == (rotorArrangement.Count - 1)) {
                        //last. give up push number
                    } else {
                        //push to next number
                        rotorArrangement[i + 1] = new RotorKeyReaderStructure() { Position = rotorArrangement[i + 1].Position + 1, Length = rotorArrangement[i + 1].Length };
                    }
                } else return;
            }

        }

        #endregion

    }

    #region rotor class

    public struct RotorKeyReaderStructure {
        /// <summary>
        /// the position where reader start reading. started by 0
        /// </summary>
        public int Position;
        /// <summary>
        /// the length of reading with binary length
        /// </summary>
        public int Length;
    }

    /// <summary>
    /// the class get rotor read length
    /// </summary>
    public class RotorIteratorBuilder {

        /// <summary>
        /// create a builder
        /// </summary>
        /// <param name="rotorCount">if you want to use rotor. please have 2 and over key previously</param>
        /// <param name="keyLength"></param>
        public RotorIteratorBuilder(int rotorCount, int keyLength) {
            if (rotorCount <= 1 || keyLength < rotorCount) throw new ArgumentException();

            this.RotorCount = rotorCount;
            this.KeyLength = keyLength;

            //initialize
            pos = new List<int>(this.RotorCount);
            for (int i = 0; i < this.RotorCount; i++) {
                pos.Add(0);
            }
        }

        int RotorCount;
        int KeyLength;

        List<int> pos;

        /* this method's parameter named by a simple math question
         
           remainingGap is the count of remaining gaps
           boardNumber is the number of board. from 0 to RotorCount - 1
         */
        /// <summary>
        /// the assistant function of main method
        /// </summary>
        /// <param name="remainingGap"></param>
        /// <param name="boardNumber"></param>
        /// <param name="previouseGap"></param>
        /// <returns></returns>
        IEnumerable<List<int>> RotorReadLengthNextEx(int remainingGap, int boardNumber, int previouseGap) {
            for (int i = 1; i < remainingGap; i++) {
                pos[boardNumber] = i + previouseGap;
                if (boardNumber != RotorCount - 1) {
                    foreach (var item in RotorReadLengthNextEx(remainingGap - i, boardNumber + 1, i + previouseGap)) {
                        yield return item;
                    }
                } else {
                    yield return pos;
                }
            }
        }

        /// <summary>
        /// a iterator of aes read length with cycle
        /// </summary>
        /// <returns></returns>
        public IEnumerable<List<int>> RotorReadLengthNext() {
            var list = new List<int>();

            while (true) {
                //why the formula is KeyLength - 1 + RotorCount and why -1 in the result
                //this is a simple math question. but i couldn't describe it in there.
                foreach (var item in RotorReadLengthNextEx(KeyLength - 1 + RotorCount, 0, 0)) {
                    list.Clear();
                    for (int i = 0; i < item.Count; i++) {
                        if (i == item.Count - 1) {
                            //last item
                            list.Add(this.KeyLength - item[i] - 1);
                        } else if (i == 0) {
                            //first item
                            list.Add(item[i] - 0);
                        } else {
                            list.Add(item[i + 1] - item[i] - 1);
                        }
                    }
                    yield return list;
                }
            }
        }
    }



    #endregion



}
