using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWRG.Input
{
    public interface IReportProvider
    {
        Report GetReport(string path);
    }
}
