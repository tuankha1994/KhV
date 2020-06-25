using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System.IO;

namespace KhV.Ultis
{
    public static class KhVLogging
    {
        public static void ConfigLog(this IConfiguration configuration, string startMessage)
        {
            //Switch To SeriLogs
            var logPath = configuration.GetValue<string>("serilog:write-to:RollingFile.pathlog");
            var buffersize = configuration.GetValue<int>("serilog:write-to:RollingFile.bufferSize");
            var share = configuration.GetValue<bool>("serilog:write-to:RollingFile.shared");
            var retainedFileCountLimit = configuration.GetValue<int>("serilog:write-to:RollingFile.retainedFileCountLimit");
            var rollOnFileSizeLimit = configuration.GetValue<bool>("serilog:write-to:RollingFile.rollOnFileSizeLimit");
            var fileSizeLimitBytes = configuration.GetValue<long>("serilog:write-to:RollingFile.fileSizeLimitBytes");
            buffersize = buffersize > 0 ? buffersize : 5000;

            //Configue 
            var logger = new LoggerConfiguration()
                .ReadFrom.AppSettings();

            if (string.IsNullOrEmpty(logPath))
            {
                logPath = "Logging/log-{Date}.json";
            }

            if (rollOnFileSizeLimit)
            {
                logPath = logPath.Replace("{Date}", "");
            }

            if (retainedFileCountLimit <= 0)
            {
                retainedFileCountLimit = 31;
            }

            if (fileSizeLimitBytes <= 0)
            {
                fileSizeLimitBytes = 1073741824;
            }

            if (!Path.IsPathRooted(logPath))
            {

            }

            if (!string.IsNullOrEmpty(logPath))
            {
                if (rollOnFileSizeLimit)
                {
                    logger = logger.WriteTo.Async(a => a.File(new JsonFormatter(), logPath, shared: share, retainedFileCountLimit: retainedFileCountLimit, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: fileSizeLimitBytes), buffersize);
                }
                else
                {
                    logger = logger.WriteTo.Async(a => a.RollingFile(new JsonFormatter(), logPath, shared: share, retainedFileCountLimit: retainedFileCountLimit), buffersize);
                }

            }
            Log.Logger = logger.Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithMemoryUsage()
                .Enrich.WithThreadId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithThreadName()
                .CreateLogger();

            if (!string.IsNullOrEmpty(startMessage))
            {
                Log.Information(startMessage);
            }
        }
    }
}
