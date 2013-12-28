using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace HWRG
{
    public class Options
    {
        [OptionArray('f', "files", Required=true, HelpText = "Files to parse.")]
        public string[] Files { get; set; }

        [Option('t', "type", DefaultValue = "html", HelpText = "Output report type.")]
        public string Type { get; set; }

        [Option('o', "outdir", Required = true, HelpText = "Output directory..")]
        public string OutputDir { get; set; }

        [Option('v', "verbose", HelpText = "Display information messages.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
