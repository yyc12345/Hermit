using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HermitLib {

    //public static class StringExtension {

    //    /// <summary>
    //    /// extension method of string. it return a string 
    //    /// </summary>
    //    /// <param name="str"></param>
    //    /// <param name="startPosition">based on zero</param>
    //    /// <param name="length"></param>
    //    /// <returns></returns>
    //    public static string SubstringWithCycle(this string str, int startPosition, int length) {
    //        if (str == null || str == string.Empty) return string.Empty;

    //        StringBuilder sb = new StringBuilder();
    //        int lengthLeaved = length;
    //        int allLength = str.Length;
    //        int nowPosition = startPosition;
    //        while (true) {
    //            if (allLength - nowPosition >= lengthLeaved) {
    //                sb.Append(str.Substring(nowPosition, lengthLeaved));
    //                break;
    //            } else {
    //                sb.Append(str);
    //                nowPosition = 0;
    //                lengthLeaved -= allLength;
    //            }
    //        }

    //        return sb.ToString();
    //    }

    //    /// <summary>
    //    /// convert to byte array from hex string whithout 0x
    //    /// </summary>
    //    /// <param name="hexStr"></param>
    //    /// <returns></returns>
    //    public static byte[] ToByteArrayFromHexString(this string hexStr) {
    //        var count = hexStr.Length;

    //        if (count % 2 == 1) {
    //            throw new ArgumentException("Invalid length of bytes:" + count);
    //        }

    //        var byteCount = count / 2;
    //        var result = new byte[byteCount];
    //        for (int ii = 0; ii < byteCount; ++ii) {
    //            var tempBytes = Byte.Parse(hexStr.Substring(2 * ii, 2), System.Globalization.NumberStyles.HexNumber);
    //            result[ii] = tempBytes;
    //        }

    //        return result;
    //    }

    //}

    //public static class ByteArrayExtension {

    //    /// <summary>
    //    /// determin 2 byte array is equal with digital mode
    //    /// </summary>
    //    /// <param name="value1"></param>
    //    /// <param name="value2"></param>
    //    /// <returns></returns>
    //    public static bool IsEqualWithDigitalMode(this byte[] b1, byte[] b2) {
    //        if (b1 == null || b2 == null) return false;
    //        if (b1.Length != b2.Length) return false;
    //        for (int i = 0; i < b1.Length; i++)
    //            if (b1[i] != b2[i])
    //                return false;
    //        return true;
    //    }

    //    /// <summary>
    //    /// return the HEX pattern of this byte array
    //    /// </summary>
    //    /// <param name="bytes"></param>
    //    /// <returns></returns>
    //    public static string ToHexString(this byte[] bytes) {
    //        string returnStr = "";
    //        if (bytes != null) {
    //            for (int i = 0; i < bytes.Length; i++) {
    //                returnStr += bytes[i].ToString("X2");
    //            }
    //        }
    //        return returnStr;
    //    }

    //}



    public class FilePathBuilder {

        private Stack<string> pathStack;

        public FilePathBuilder(string defaultPath, PlatformID os) {
            pathStack = new Stack<string>();

            if (defaultPath == string.Empty) throw new ArgumentException();
            OSType = os;

            switch (os) {
                case PlatformID.Win32NT:
                    foreach (var item in defaultPath.Split('\\')) {
                        if (item != string.Empty && item != "") pathStack.Push(item);
                    }
                    break;
                case PlatformID.Unix:
                    bool isFirst = true;
                    foreach (var item in defaultPath.Split('/')) {
                        if (item != string.Empty)
                            pathStack.Push(item);
                        else {
                            if (isFirst) {
                                pathStack.Push(item);
                                isFirst = false;
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public FilePathBuilder(Stack<string> generalPath) {
            this.pathStack = generalPath;
        }

        /// <summary>
        /// Backtracking to previous path
        /// </summary>
        public void Backtracking(PlatformID os) {
            switch (os) {
                case PlatformID.Win32NT:
                    if (pathStack.Count <= 1) return;
                    break;
                case PlatformID.Unix:
                    if (pathStack.Count <= 1) return;
                    break;
                default:
                    return;
            }

            pathStack.Pop();
        }

        public FilePathBuilder Enter(string name) {
            pathStack.Push(name);
            return this;
        }

        public FilePathBuilder Enter(List<string> name) {
            foreach (var item in name) {
                pathStack.Push(item);
            }
            return this;
        }

        public string Path() {
            return this.Path(Information.OS);
        }

        /// <summary>
        /// get the path without slash
        /// </summary>
        public string Path(PlatformID os) {
            if (pathStack.Count == 0) return "";
            else {
                //reserve the list because the ground of stack is the top of stack
                List<string> GetPathList() {
                    var cache = pathStack.ToList();
                    cache.Reverse();
                    return cache;
                }

                switch (os) {
                    case PlatformID.Win32NT:
                        return string.Join(@"\", GetPathList());
                    case PlatformID.Unix:
                        return string.Join(@"/", GetPathList());
                    default:
                        return "";
                }
            }

        }

        public Stack<string> GeneralPath() {
            return pathStack;
        }
    }

    public class Random {

        public Random() {
            secureRandom = new System.Security.Cryptography.RNGCryptoServiceProvider();
            basicRandom = new System.Random();
        }

        System.Security.Cryptography.RNGCryptoServiceProvider secureRandom;
        System.Random basicRandom;

        public void NextBytes(ref byte[] buffer) {
            secureRandom.GetBytes(buffer);
        }

        public void UnSafeNextBytes(ref byte[] buffer) {
            basicRandom.NextBytes(buffer);
        }

        public int Next(int maxValue) => basicRandom.Next(maxValue);
        public int Next(int minValue, int maxValue) => basicRandom.Next(minValue, maxValue);

    }

}
