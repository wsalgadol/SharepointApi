using NLog;
using Sharepoint.Logs.Interfaces;
using System;

namespace Sharepoint.Logs.Services
{
    public class NLogLoggerService : ILoggerService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message, Exception ex = null)
        {
            if (ex != null)
                _logger.Error(ex, message);
            else
                _logger.Error(message);
        }
    }
}
