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
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    async options => await Execute(options),
                    errors => Task.FromResult(-1));
        }

        private static async Task<int> Execute(Options options)
        {
            var logger = CreateLogger(options);
                        
            var dataCreator = new DataCreator(
                options.BackOfficeUrl,
                options.User, 
                options.Password,
                options.NumberOfContentNodes,
                options.ChunkSize,
                options.MaxItemsPerLevel,
                logger);

            await dataCreator.Start();
                            
          
            logger.Information("Done. Press any key to close down");
            Console.ReadKey();
            return 0;
        }

        private static Logger CreateLogger(Options options) =>
            new LoggerConfiguration()
                .MinimumLevel.Is(options.Verbosity)
                .WriteTo.ColoredConsole()
                .CreateLogger();

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Options
        {
            public Options(string backOfficeUrl,
                uint numberOfContentNodes, 
                string user,
                string password,
                LogEventLevel verbosity,
                uint chunkSize, 
                uint maxItemsPerLevel)
            {
                BackOfficeUrl = backOfficeUrl;
                NumberOfContentNodes = numberOfContentNodes;
                User = user;
                Password = password;
                Verbosity = verbosity;
                ChunkSize = chunkSize;
                MaxItemsPerLevel = maxItemsPerLevel;
            }

            [Option('b', "backoffice", Required = true,
                HelpText = "Set the public url-path to the Umbraco Backoffice.")]
            public string BackOfficeUrl { get; }

            [Option('n', "number", Required = true, HelpText = "The number of content to create.")]
            public uint NumberOfContentNodes { get; }

            [Option('u', "user", Required = true, HelpText = "The user/email to login to Umbraco Backoffice.")]
            public string User { get; }

            [Option('p', "password", Required = true, HelpText = "The password to login to Umbraco Backoffice.")]
            public string Password { get;  }
            
            [Option('v', "verbosity", Required = false, HelpText = "The log verbosity.", Default = LogEventLevel.Information)]
            public LogEventLevel Verbosity { get; }

            [Option('c', "chunk-size", Required = false, HelpText = "The maximum chunk size when making parallel requests", Default = 100U)]
            public uint ChunkSize { get;  }
            
            [Option('m', "max-items-per-level", Required = false, HelpText = "The maximum children of each parent", Default = 10U)]
            public uint MaxItemsPerLevel { get;  }
            
        }
    }
}