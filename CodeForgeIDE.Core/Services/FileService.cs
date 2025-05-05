namespace CodeForgeIDE.Core.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Opens a file.
        /// </summary>
        /// <param name="filePath">Path relative to current workspace</param>
        public Stream OpenFile(string filePath);

        /// <summary>
        /// Reads a file.
        /// </summary>
        /// <param name="filePath">Path relative to current workspace</param>
        /// <returns></returns>
        public string ReadFile(string filePath);

        /// <summary>
        /// Writes to a file.
        /// </summary>
        /// <param name="filePath">Path relative to current workspace</param>
        /// <param name="content"></param>
        public void WriteFile(string filePath, string content);

        /// <summary>
        /// Writes to a file.
        /// </summary>
        /// <param name="filePath">Path relative to current workspace</param>
        /// <param name="content"></param>
        public void WriteFile(string filePath, byte[] content);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="filePath">Path relative to current workspace</param>
        /// <returns></returns>
        public bool FileExists(string filePath);

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="filePath">Path relative to current workspace</param>
        public void DeleteFile(string filePath);

        /// <summary>
        /// Gets the directory name of a file path.
        /// </summary>
        /// <param name="directoryPath">Path relative to current workspace</param>
        /// <returns></returns>
        public string[] GetFiles(string directoryPath);
    }

    public class FileService : IFileService
    {
        public void WriteFile(string filePath, byte[] content)
        {
            throw new NotImplementedException();
        }
        public void WriteFile(string filePath, string content)
        {
            throw new NotImplementedException();
        }
        public string ReadFile(string filePath)
        {
            throw new NotImplementedException();
        }
        public Stream OpenFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string filePath)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public string[] GetFiles(string directoryPath)
        {
            throw new NotImplementedException();
        }
    }
}
