using System;
using System.Security.Cryptography;
using System.Text;
using Globals;

namespace ProdigyAPI.Utils
{
    public class CryptoLibrary
    {
        public String EncryptWithRijndael(String plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged()));
        }

        public String DecryptWithRijndael(String encryptedText)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged()));
        }

        public string EncryptUsingHash(string plainText, string salt)
        {
            return CGlobals.GetEncrptedPassword(plainText, salt);
        }

        private RijndaelManaged GetRijndaelManaged()
        {
            var keyBytes = new byte[16];
            string secretKey = "4KVJIuTUt7n2IijBbVRnfrNyrUTRR7a64N43vcJ7Xy9Zy";
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

       
    }
}