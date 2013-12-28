using System.Collections.Generic;

namespace HWRG.Input
{
    public interface IReportMerger
    {
        Report GetReport(IEnumerable<Report> reports);
    }
}