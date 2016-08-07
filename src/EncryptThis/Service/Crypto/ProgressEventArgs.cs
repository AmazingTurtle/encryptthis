using System;

namespace EncryptThis.Service.Crypto
{
    public class ProgressEventArgs : EventArgs
    {

        public ProgressEventArgs(long processedBytes, long totalBytes)
        {
            ProcessedBytes = processedBytes;
            TotalBytes = totalBytes;
        }

        public long ProcessedBytes { get; private set; }
        public long TotalBytes { get; private set; }
        public double Progress {
            get { return (double)ProcessedBytes / (double)TotalBytes; }
        }
        
    }
}
