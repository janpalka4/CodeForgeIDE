namespace CodeForgeIDE.Core.Services
{
    public interface ILoggerService
    {
        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message"></param>
        public void LogInfo(string message, string context);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message"></param>
        public void LogWarning(string message, string context);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message"></param>
        public void LogError(string message, string context);
    }
    public class LoggerService : ILoggerService
    {
        public void LogError(string message, string context)
        {
            //throw new NotImplementedException();
        }

        public void LogInfo(string message, string context)
        {
            //throw new NotImplementedException();
        }

        public void LogWarning(string message, string context)
        {
            //throw new NotImplementedException();
        }
    }
}
