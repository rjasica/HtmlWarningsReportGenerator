using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using HWRG.Input;
using File = HWRG.Input.File;

namespace HWRG.Processor.Html
{
    public class HtmlProcessor : IProcessor
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        public void Write(string outputDir, Report report)
        {
            Log.InfoFormat("Writing html report to {0}", outputDir);

            var sourceWriter = new SourceWriter();
            var indexWriter = new IndexWriter();

            var filesd = new Dictionary<File, string>();
            foreach (var file in report.Files)
            {
                Log.InfoFormat("Writing html for {0}", file.Name);
                var filename = sourceWriter.Write(outputDir, file, report.Types);
                filesd[file] = filename;
            }

            Log.InfoFormat("Writing css", outputDir);
            WriteCss(outputDir, report);

            Log.InfoFormat("Writing index", outputDir);
            indexWriter.Write(outputDir, filesd, report.Types);
        }

        private static void WriteCss(string outputDir, Report report)
        {
            var cssPath = Path.Combine(outputDir, "report.css");
            var sb = new StringBuilder();
            foreach (var type in report.Types)
            {
                WriteTrCss(sb, type);
            }
            var generated = sb.ToString();
            System.IO.File.WriteAllText(cssPath, generated + Properties.Resources.report);
        }

        private static void WriteTrCss(StringBuilder sb, TypeDefinition type)
        {
            sb.AppendFormat("tr.{0} td.colormargin {{ background-color: {1}; }}", type.Name, type.Color);
            sb.AppendLine();
            sb.AppendFormat("tr.{0} td.codecolor {{ background-color: {1};}}", type.Name, type.Background);
            sb.AppendLine();
            sb.AppendFormat("div.{0}.annotationdiv {{ background-color: {1};border-color: {2}; }}", type.Name, type.Background, type.Color);
            sb.AppendLine();
        }
    }
}
