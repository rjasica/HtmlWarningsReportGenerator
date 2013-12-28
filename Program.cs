using System;
using Common.Logging;
using HWRG.Builder;

namespace HWRG
{
    internal class Program
    {
        private static ILog Log { get; set; }

        private static int Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                LogFactory.IsVerbose = options.Verbose;
                Log = LogManager.GetCurrentClassLogger();
                try
                {
                    IReportBuilderFactory builderFactory = new ReportBuilderFactory();
                    var builder = builderFactory.Create();
                    builder.Build(options);
                }
                catch (AppException ex)
                {
                    Log.Error(ex.Message);
                    Log.Info("Error occurs. Run sign -help to get more info.", ex);
                    return ex.ErrorCode;
                }
                catch (Exception ex)
                {
                    Log.Error("Internal error. Run with -verbose to see more details");
                    Log.Info(ex.Message, ex);
                    return ErrorCodes.InternalError;
                }

                return ErrorCodes.Ok;
            }
            return ErrorCodes.ParserError;
        }
    }
}
