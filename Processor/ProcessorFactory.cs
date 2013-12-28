using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HWRG.Processor.Html;

namespace HWRG.Processor
{
    public class ProcessorFactory : IProcessorFactory
    {
        public IProcessor Create(string name)
        {
            switch (name.ToUpperInvariant())
            {
                case "HTML":
                    return new HtmlProcessor();
                default:
                    throw new AppException(string.Format("Unknown report type: {0}", name), ErrorCodes.ReportType);
            }
        }
    }
}
