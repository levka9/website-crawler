using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Console.Configuration
{
    public static class SerilogConfig
    {
        public static Serilog.Core.Logger Get(IConfigurationRoot configuration)
        {
            return new LoggerConfiguration().ReadFrom
                                            .Configuration(configuration)
                                            .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(logEvent => logEvent.Level == Serilog.Events.LogEventLevel.Information)
                                                                    .WriteTo.File(formatter: new CompactJsonFormatter(),
                                                                                  //outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3}] {Username} {Message:lj}{NewLine}{Exception}",
                                                                                  fileSizeLimitBytes: 100000,
                                                                                  rollingInterval: RollingInterval.Day,
                                                                                  path: "../../../Logs/information-.json",
                                                                                  encoding: Encoding.UTF8),
                                                            Serilog.Events.LogEventLevel.Information)
                                            .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(logEvent => logEvent.Exception is Exception)
                                                                    .WriteTo.File(formatter: new CompactJsonFormatter(),
                                                                                  //outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3}] {Username} {Message:lj}{NewLine}{Exception}",
                                                                                  fileSizeLimitBytes: 100000,
                                                                                  rollingInterval: RollingInterval.Day,
                                                                                  path: "../../../Logs/exceptions-.json",
                                                                                  encoding: Encoding.UTF8),
                                                            Serilog.Events.LogEventLevel.Information)
                                            .CreateLogger();
        }
    }
}
