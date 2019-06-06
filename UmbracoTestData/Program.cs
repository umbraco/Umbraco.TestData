using System;
using System.Threading.Tasks;
using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace UmbracoTestData
{
    internal class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options =>
                    {
                        var logger = CreateLogger(options);
                        
                        var dataCreator = new DataCreator(
                            options.BackOfficeUrl,
                            options.User, 
                            options.Password,
                            options.NumberOfContentNodes,
                            options.ChunkSize,
                            logger);

                        var task = dataCreator.Start();
                            
                        task.GetAwaiter().GetResult(); // TODO Nasty - Solve better way

                        logger.Information("Done. Press any key to close down");
                        Console.ReadKey();
                        return 0;
                    },
                    errors => -1);
        }

        private static Logger CreateLogger(Options options) =>
            new LoggerConfiguration()
                .MinimumLevel.Is(options.Verbosity)
                .WriteTo.ColoredConsole()
                .CreateLogger();

        private class Options
        {
            [Option('b', "backoffice", Required = true,
                HelpText = "Set the public url-path to the Umbraco Backoffice.")]
            public string BackOfficeUrl { get; set; }

            [Option('n', "number", Required = true, HelpText = "The number of content to create.")]
            public uint NumberOfContentNodes { get; set; }

            [Option('u', "user", Required = true, HelpText = "The user/email to login to Umbraco Backoffice.")]
            public string User { get; set; }

            [Option('p', "password", Required = true, HelpText = "The password to login to Umbraco Backoffice.")]
            public string Password { get; set; }
            
            [Option('v', "verbosity", Required = false, HelpText = "The log verbosity.", Default = LogEventLevel.Information)]
            public LogEventLevel Verbosity { get; set; }

            [Option('c', "chunksize", Required = false, HelpText = "The maximum chunk size when making parallel requests", Default = 100U)]
            public uint ChunkSize { get; set; }
        }
    }
}