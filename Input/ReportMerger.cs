using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace HWRG.Input
{
    public class ReportMerger : IReportMerger
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        public Report GetReport(IEnumerable<Report> reports)
        {
            Log.InfoFormat("Merging {0} reports", reports.Count());

            var files = reports.SelectMany(x => x.Files)
                .GroupBy(x => x.Name)
                .Select(x => new File()
                {
                    Name = x.Key,
                    Annotations = x.SelectMany(y => y.Annotations).ToArray(),
                })
                .ToArray();

            var types = reports.SelectMany(x => x.Types).GroupBy(x => x.Name).Select(x => x.First()).OrderBy(x => x.Display).ToArray();
            var typeList = new List<TypeDefinition>(types);
            var allTypes = files.SelectMany(x => x.Annotations).Select(x => x.Type).Distinct();
            foreach (var type in allTypes.Where(x => types.All(y => !y.Name.Equals(x))))
            {
                typeList.Add(new TypeDefinition()
                {
                    Name = type,
                    Color = "#ff0000",
                    Background = "#f7dede",
                    Display = type,
                });
            }
            var used = new HashSet<string>(reports.SelectMany(x => x.Files).SelectMany(x => x.Annotations).Select(x => x.Type).Distinct());

            return new Report()
            {
                Files = files,
                Types = types.Where(x => used.Contains(x.Name)).ToArray(),
            };
        }
    }
}
