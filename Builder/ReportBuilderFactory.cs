using HWRG.Input;
using HWRG.Processor;

namespace HWRG.Builder
{
    public class ReportBuilderFactory : IReportBuilderFactory
    {
        public IReportBuilder Create()
        {
            IReportProvider reportProvider = new XmlReportProvider();
            IReportMerger reportMerger = new ReportMerger();
            IProcessorFactory processorFactory = new ProcessorFactory();

            return new ReportBuilder(reportProvider, reportMerger, processorFactory);
        }
    }
}
