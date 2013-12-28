using System.IO;
using System.Linq;
using HWRG.Input;
using HWRG.Processor;

namespace HWRG.Builder
{
    public class ReportBuilder : IReportBuilder
    {
        private readonly IReportProvider reportProvider;
        private readonly IReportMerger reportMerger;
        private readonly IProcessorFactory processorFactory;

        public ReportBuilder(IReportProvider reportProvider, IReportMerger reportMerger, IProcessorFactory processorFactory)
        {
            this.reportProvider = reportProvider;
            this.reportMerger = reportMerger;
            this.processorFactory = processorFactory;
        }

        public void Build(Options options)
        {
            var processor = processorFactory.Create(options.Type);
            var reports = options.Files.Select(reportProvider.GetReport).ToArray();
            var report = reportMerger.GetReport(reports);
            Directory.CreateDirectory(options.OutputDir);
            processor.Write(options.OutputDir, report);
        }
    }
}
