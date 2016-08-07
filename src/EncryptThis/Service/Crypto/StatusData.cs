namespace EncryptThis.Service.Crypto
{
    public class StatusData
    {

        public StatusData(bool isLocked, int totalFiles, int totalDirectories, long totalSize)
        {
            IsLocked = isLocked;
            TotalFiles = totalFiles;
            TotalDirectories = totalDirectories;
            TotalSize = totalSize;
        }

        public bool IsLocked { get; private set; }
        public int TotalFiles { get; private set; }
        public int TotalDirectories { get; private set; }
        public long TotalSize { get; private set; }
        
    }
}
