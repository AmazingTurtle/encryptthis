using EncryptThis.Service.Crypto;

namespace EncryptThis.Service
{
    public interface IApi
    {

        /// <summary>
        /// Uses the Crypto Service to check the password
        /// </summary>
        /// <param name="password">The password to be checked</param>
        /// <returns>True if password is correct</returns>
        bool CheckPassword(string password);

        /// <summary>
        /// Sums up the folders, files and filesize in the container directory
        /// </summary>
        /// <returns>A StatusData class with necessary data</returns>
        StatusData GetStatusData();

        /// <summary>
        /// Checks if the lockfile exists
        /// </summary>
        /// <returns>True if lockfile exists (locked)</returns>
        bool IsLocked();

        /// <summary>
        /// Opens the current container directory in the explorer
        /// </summary>
        void OpenContainer();

        /// <summary>
        /// Checks if contianer directory is locked and encrypts or decrypts it
        /// </summary>
        /// <param name="password">The password used for encryption or decryption</param>
        void ToggleLock(string password);

    }
}