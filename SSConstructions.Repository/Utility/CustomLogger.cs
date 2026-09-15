using Microsoft.Extensions.Options;
using SSConstructions.Repository.Models;

namespace SSConstructions.Repository.Utility
{
    public class CustomLogger
    {
        private readonly string _logDirectory;
        private static readonly object _lock = new object();

        public CustomLogger(IOptions<LogSettings> settings)
        {
            _logDirectory = settings.Value.LogFilePath ??
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        public void Log(string message, string logType = "INFO")
        {
            try
            {
                string logFile = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}.txt");
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logType}] {message}{Environment.NewLine}";

                lock (_lock)
                {
                    File.AppendAllText(logFile, logMessage);
                }
            }
            catch
            {
                // Never throw exceptions from logger
            }
        }

        public void LogError(Exception ex)
        {
            string errorMessage = $"Exception: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}";
            Log(errorMessage, "ERROR");
        }
    }
}
