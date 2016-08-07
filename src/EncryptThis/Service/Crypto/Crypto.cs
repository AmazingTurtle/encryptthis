using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptThis.Service.Crypto
{
    public class Crypto : ICrypto
    {
        #region Consts

        private const int SaltSize = 32;
        private const int KeySize = 256;
        private const int BlockSize = 128;
        private const int BufferSize = 131072;
        private const string EncryptedExtension = ".etlock";
        private const string LockFileName = ".lock";
        public const string PasswordCheckString = "ilikeittalentsbecauseitiscoolandicanwinalotofmoney";

        #endregion

        #region Member

        public byte[] Salt { get; private set; }
        private byte[] key;
        private byte[] iv;

        #endregion

        #region Constructor, Functions

        public Crypto(string password)
        {
            var randomizer = new RNGCryptoServiceProvider();
            Salt = new byte[SaltSize];
            randomizer.GetBytes(Salt);
            PostConstructor(password);
        }

        public Crypto(string password, byte[] salt)
        {
            this.Salt = salt;
            PostConstructor(password);
        }

        private void PostConstructor(string password)
        {
            var deriveBytes = new Rfc2898DeriveBytes(password, Salt);
            key = deriveBytes.GetBytes(KeySize/8);
            iv = deriveBytes.GetBytes(BlockSize/8);
        }

        private RijndaelManaged GetAesContext()
        {
            var aes = new RijndaelManaged();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            return aes;
        }

        public byte[] EncryptData(byte[] decrypted)
        {
            byte[] data;
            using (var aes = GetAesContext())
            using (var encryptor = aes.CreateEncryptor())
            using (var memoryStream = new MemoryStream())
            { 
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(decrypted, 0, decrypted.Length);
                    cryptoStream.FlushFinalBlock();
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

        public byte[] DecryptData(byte[] encrypted)
        {
            byte[] data;
            using (var aes = GetAesContext())
            {
                using (var decryptor = aes.CreateDecryptor())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(encrypted, 0, encrypted.Length);
                            cryptoStream.FlushFinalBlock();
                        }
                        data = memoryStream.ToArray();
                    }
                }
            }
            return data;
        }

        public bool CheckPassword(byte[] encrypted)
        {
            try
            {
                var expected = Encoding.UTF8.GetBytes(PasswordCheckString);
                return DecryptData(encrypted).SequenceEqual(expected);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        public async Task DecryptFolder(string containerDirectory)
        {
            var files = Directory.GetFiles(containerDirectory, "*" + EncryptedExtension, SearchOption.AllDirectories);
            var totalSize = files.Sum(fileName => new FileInfo(fileName).Length);
            long currentProgress = 0;

            using (var aes = GetAesContext())
            using (var decryptor = aes.CreateDecryptor())
            {
                foreach (var fileName in files)
                {
                    using (var sourceStream = new CryptoStream(new BufferedStream(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), 4096), decryptor, CryptoStreamMode.Read))
                    using (var targetStream = new BufferedStream(File.Open(fileName.Substring(0, fileName.Length - EncryptedExtension.Length), FileMode.Create, FileAccess.Write, FileShare.None), BufferSize))
                    {
                        var buffer = new byte[BufferSize];
                        int length;
                        do
                        {
                            length = await sourceStream.ReadAsync(buffer, 0, buffer.Length);
                            if (length > 0)
                            {
                                currentProgress += length;
                                await targetStream.WriteAsync(buffer, 0, length);
                                OnProgress(new ProgressEventArgs(currentProgress, totalSize));
                            }
                        } while (length > 0);
                        await targetStream.FlushAsync();
                        sourceStream.Close();
                        targetStream.Close();
                    }
                    File.Delete(fileName);
                }
            }
        }

        public async Task EncryptFolder(string containerDirectory)
        {
            var files = Directory.GetFiles(containerDirectory, "*", SearchOption.AllDirectories).Where(fileName => new FileInfo(fileName).Name != LockFileName).ToArray();
            var totalSize = files.Sum(fileName => new FileInfo(fileName).Length);

            long currentProgress = 0;
            using (var aes = GetAesContext())
            using (var encryptor = aes.CreateEncryptor())
            {
                foreach (var fileName in files)
                {
                    using (var sourceStream = new BufferedStream(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), BufferSize))
                    using (var targetStream = new CryptoStream(new BufferedStream(File.Open(fileName + EncryptedExtension, FileMode.Create, FileAccess.Write, FileShare.None), BufferSize), encryptor, CryptoStreamMode.Write))
                    {
                        while (sourceStream.Position < sourceStream.Length)
                        {
                            var buffer = new byte[BufferSize];
                            var length = await sourceStream.ReadAsync(buffer, 0, buffer.Length);
                            currentProgress += length;
                            await targetStream.WriteAsync(buffer, 0, length);
                            OnProgress(new ProgressEventArgs(currentProgress, totalSize));
                        }
                        await targetStream.FlushAsync();
                        sourceStream.Close();
                        targetStream.Close();
                    }
                    File.Delete(fileName);
                }
            }
        }

        #endregion

        #region Events

        private void OnProgress(ProgressEventArgs e)
        {
            if (Progress != null)
            {
                Progress(this, e);
            }
        }
        public event EventHandler<ProgressEventArgs> Progress;

        #endregion

    }
}
