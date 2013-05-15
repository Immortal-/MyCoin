/*
* Copyright (C) 2011, Dextrey (0xDEADDEAD)
* Removing this copyright notice is prohibited without permission from author
* It is allowed to modify and use this code in software (commercial or not) if above copyright notice is 
* preserved 
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Dextrey.Crypto
{
    class PolymorphicCryptWrapper
    {

        const int saltSize = 32;

        System.Security.Cryptography.SymmetricAlgorithm algo;
        System.Security.Cryptography.RNGCryptoServiceProvider rngAlgo;
        byte[] salt;
        public PolymorphicCryptWrapper(System.Security.Cryptography.SymmetricAlgorithm algorithm)
        {
            this.algo = algorithm;
            this.algo.Mode = System.Security.Cryptography.CipherMode.CBC;

            this.rngAlgo = new System.Security.Cryptography.RNGCryptoServiceProvider();
        }

        private void InitializeSecureParameters(byte[] key)
        {
            // init IV
            this.algo.GenerateIV();
            salt = new byte[saltSize];
            rngAlgo.GetBytes(salt);
            System.Security.Cryptography.Rfc2898DeriveBytes pwDeriveAlg = new System.Security.Cryptography.Rfc2898DeriveBytes(key, salt, 2000);
            this.algo.Key = pwDeriveAlg.GetBytes(algo.KeySize / 8);
        }

        private void LoadSecureParameters(byte[] key, byte[] encIv, byte[] encSalt)
        {
            this.algo.IV = encIv;
            this.salt = encSalt;
            System.Security.Cryptography.Rfc2898DeriveBytes pwDeriveAlg = new System.Security.Cryptography.Rfc2898DeriveBytes(key, salt, 2000);
            this.algo.Key = pwDeriveAlg.GetBytes(algo.KeySize / 8);
        }

        public string Encrypt(string plainText, string key)
        {
            return Convert.ToBase64String(this.Encrypt(UnicodeEncoding.UTF8.GetBytes(plainText), UnicodeEncoding.UTF8.GetBytes(key)));
        }

        public string Decrypt(string cipherText, string key)
        {
            return UnicodeEncoding.UTF8.GetString(this.Decrypt(Convert.FromBase64String(cipherText), UnicodeEncoding.UTF8.GetBytes(key)));
        }

        public byte[] Encrypt(byte[] plainText, byte[] key)
  {
    InitializeSecureParameters(key);
    System.Security.Cryptography.ICryptoTransform encTransform = algo.CreateEncryptor();
            return ConcatDataToCipherText(ConcatDataToCipherText(encTransform.TransformFinalBlock(plainText,0,plainText.Length),salt),algo.IV);
  }

        public byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            byte[] cipherTextWithSalt = new byte[1];
            byte[] encSalt = new byte[1];
            byte[] origCipherText = new byte[1];
            byte[] encIv = new byte[1];

            SliceCipherTextIntoParts(cipherText, algo.BlockSize / 8, ref cipherTextWithSalt, ref encIv);
            SliceCipherTextIntoParts(cipherTextWithSalt, saltSize, ref origCipherText, ref encSalt);
            LoadSecureParameters(key, encIv, encSalt);
            System.Security.Cryptography.ICryptoTransform decTransform = algo.CreateDecryptor();
            byte[] plainText = decTransform.TransformFinalBlock(origCipherText, 0, origCipherText.Length);

            return plainText;
        }

        private byte[] ConcatDataToCipherText(byte[] cipherText, byte[] iv)
        {
            int origLength = cipherText.Length;
            Array.Resize(ref cipherText, cipherText.Length + iv.Length);
            Buffer.BlockCopy(iv, 0, cipherText, origLength, iv.Length);
            return cipherText;
        }
        private void SliceCipherTextIntoParts(byte[] cipherText, int secondPartLen, ref byte[] origCipherText, ref byte[] iv)
        {
            Array.Resize(ref iv, secondPartLen);
            Buffer.BlockCopy(cipherText, (int)(cipherText.Length - secondPartLen), iv, 0, secondPartLen);
            Array.Resize(ref origCipherText, (int)(cipherText.Length - secondPartLen));
            Buffer.BlockCopy(cipherText, 0, origCipherText, 0, (int)(cipherText.Length - secondPartLen));
        }
    }
}