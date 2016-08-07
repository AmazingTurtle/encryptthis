using System;
using System.Threading.Tasks;
using EncryptThis.Service.Crypto;

namespace EncryptThis.Service
{
    /// <summary>
    /// Manages encryption, decryption and checks.
    /// Algorithm used: AES-256-CBC
    /// </summary>
    public interface ICrypto
    {

        /// <summary>
        /// The salt for the AES Key and IV (random for each lock file)
        /// </summary>
        byte[] Salt { get; }

        /// <summary>
        /// Checks if the data can be decrypted and equals to a password check string
        /// </summary>
        /// <param name="encrypted">The data encrypted with our password and salt which should be the password check string</param>
        /// <returns>True if check was successful</returns>
        bool CheckPassword(byte[] encrypted);

        /// <summary>
        /// Decrypt a byte array in memory
        /// </summary>
        /// <param name="encrypted">The data to be decrypted</param>
        /// <returns>Decrypted data</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        byte[] DecryptData(byte[] encrypted);

        /// <summary>
        /// Encrypt a byte array in memory
        /// </summary>
        /// <param name="decrypted">The data to be encrypted</param>
        /// <returns>Encrypted data</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        byte[] EncryptData(byte[] decrypted);

        /// <summary>
        /// Encrypts a folder asynchronously
        /// </summary>
        /// <param name="containerDirectory">The folder to be encrypted</param>
        /// <returns>IO task</returns>
        Task EncryptFolder(string containerDirectory);

        /// <summary>
        /// Decrypts a folder asynchronously
        /// </summary>
        /// <param name="containerDirectory">The folder to be decrypted</param>
        /// <returns>IO task</returns>
        Task DecryptFolder(string containerDirectory);

        /// <summary>
        /// An event for progress reporting of the current operation (encryption or decryption)
        /// </summary>
        event EventHandler<ProgressEventArgs> Progress;

    }
}