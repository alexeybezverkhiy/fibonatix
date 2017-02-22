using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Cryptography;

namespace Fibonatix.CommDoo.Helpers
{
    public static class ExceptionHelper
    {
        public static Exception SetCode(this Exception e, int value) {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo fieldInfo = typeof(Exception).GetField("_HResult", flags);

            fieldInfo.SetValue(e, value);

            return e;
        }
    }

    public static class Crypto
    {
        public static string sha256(string value) {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(value), 0, Encoding.UTF8.GetByteCount(value));
            foreach (byte theByte in crypto) {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        public static string sha1(string value) {
            System.Security.Cryptography.SHA1Managed crypt = new System.Security.Cryptography.SHA1Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(value), 0, Encoding.UTF8.GetByteCount(value));
            foreach (byte theByte in crypto) {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
