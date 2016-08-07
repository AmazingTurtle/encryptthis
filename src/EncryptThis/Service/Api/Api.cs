using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CefSharp.WinForms;
using EncryptThis.Service.Crypto;

namespace EncryptThis.Service.Api
{
    public class Api : IApi
    {

        private readonly ChromiumWebBrowser browser;
        public string ContainerDirectory { get; private set; }
        public string LockFile { get; private set; }

        public Api(ChromiumWebBrowser browser, string containerDirectory)
        {
            this.browser = browser;
            ContainerDirectory = Path.GetFullPath(containerDirectory);
            if (!Directory.Exists(containerDirectory))
            {
                Directory.CreateDirectory(containerDirectory);
            }
            LockFile = Path.Combine(ContainerDirectory, ".lock");
        }

        public void OpenContainer()
        {
            Process.Start(ContainerDirectory);
        }

        public StatusData GetStatusData()
        {
            var files = Directory.GetFiles(ContainerDirectory, "*", SearchOption.AllDirectories);
            var directories = Directory.GetDirectories(ContainerDirectory, "*", SearchOption.AllDirectories);
            var isLocked = IsLocked();
            return new StatusData(isLocked,
                files.Length - (isLocked ? 1 : 0),
                directories.Length,
                files.Sum(fileName => new FileInfo(fileName).Length));
        }

        public bool IsLocked()
        {
            return File.Exists(LockFile);
        }

        public bool CheckPassword(string password)
        {
            if (!File.Exists(LockFile))
            {
                return true;
            }
            var allBytes = File.ReadAllBytes(LockFile);
            var crypto = new Crypto.Crypto(password, allBytes.Take(32).ToArray());
            return crypto.CheckPassword(allBytes.Skip(32).ToArray());
        }

        public async void ToggleLock(string password)
        {
            if (IsLocked())
            {
                var crypto = new Crypto.Crypto(password, File.ReadAllBytes(LockFile).Take(32).ToArray());
                crypto.Progress += Crypto_Progress;
                File.Delete(LockFile);
                await crypto.DecryptFolder(ContainerDirectory);
            }
            else
            {
                var crypto = new Crypto.Crypto(password);
                using (var stream = File.Create(LockFile))
                {
                    var encrypted = crypto.EncryptData(Encoding.UTF8.GetBytes(Crypto.Crypto.PasswordCheckString));
                    stream.Write(crypto.Salt, 0, crypto.Salt.Length);
                    stream.Write(encrypted, 0, encrypted.Length);
                    stream.Flush();
                    stream.Close();
                }
                crypto.Progress += Crypto_Progress;
                await crypto.EncryptFolder(ContainerDirectory);
            }
            Finish();
        }

        private void Crypto_Progress(object sender, ProgressEventArgs e)
        {
            var frame = browser.GetBrowser().MainFrame;
            frame.ExecuteJavaScriptAsync("script.update(" + Math.Round(e.Progress * 100) + ");");
        }

        private void Finish()
        {
            var frame = browser.GetBrowser().MainFrame;
            frame.ExecuteJavaScriptAsync("script.toggleStatusOverlay(false);");
            frame.ExecuteJavaScriptAsync("script.updateState();");
        }

    }
}
