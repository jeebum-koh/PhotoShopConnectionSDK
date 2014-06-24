/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System;
using System.Security.Cryptography;

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// provides Triple DES algorithm. "DESede/CBC/PKCS5Padding" with PBKDF2 key
    /// </summary>
    public class EncryptDecrypt
    {
        /// <summary>
        /// provides encryption
        /// </summary>
        private ICryptoTransform _encryptor;

        /// <summary>
        /// provides decryption
        /// </summary>
        private ICryptoTransform _decryptor;

        /// <summary>
        /// EncryptDecrypt constructor
        /// </summary>
        /// <param name="password">used with PBKDF2 key</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown when password argument contains characters 
        /// except ascii visible characters ([\x20-\x7E]) 
        /// </exception>
        public EncryptDecrypt(
            string password)
        {
            using (var tripleDES = TripleDES.Create())
            {
                tripleDES.Key = password.DerivePBKDF2Key();
                tripleDES.Mode = CipherMode.CBC;
                tripleDES.Padding = PaddingMode.PKCS7;
                tripleDES.IV = new byte[8];

                _encryptor = tripleDES.CreateEncryptor();
                _decryptor = tripleDES.CreateDecryptor();
            }
        }

        /// <summary>
        /// provides encryption of given byte array
        /// </summary>
        /// <param name="plainBytes">plain byte array to encrypt</param>
        /// <returns>encrypted byte array</returns>
        public byte[] Encrypt(
            byte[] plainBytes)
        {
            return 
                _encryptor.
                    TransformFinalBlock(
                        plainBytes, 
                        0, 
                        plainBytes.Length);
        }

        /// <summary>
        /// provides decryption of given encrypted byte array
        /// </summary>
        /// <param name="encodedBytes">encrypted byte array to decrypt</param>
        /// <returns>decrypted plain byte array</returns>
        public byte[] Decrypt(
            byte[] encodedBytes)
        {
            return 
                _decryptor.
                    TransformFinalBlock(
                        encodedBytes, 
                        0, 
                        encodedBytes.Length);
        }

    }
}
