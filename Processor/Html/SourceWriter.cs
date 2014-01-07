using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Common.Logging;
using HWRG.Input;
using File = HWRG.Input.File;

namespace HWRG.Processor.Html
{
    internal class SourceWriter
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Regex trimSpaces = new Regex( @"(?<=^\s*)\s", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Write(string outputDir, File file, TypeDefinition[] types)
        {
            var filename = GetFileName(file.Name) + ".html";
            using (var sw = new StreamWriter(Path.Combine(outputDir, filename)))
            {
                WriteHeader(sw, file, types);
                WriteSource(sw, file, types);
                WriteFooter(sw);
            }

            return filename;
        }

        private class Line
        {
            public Line()
            {
                Markers = new HashSet<string>();
                LineAnnotations = new List<LineAnnotation>();
            }

            public string Source { get; set; }
            public int Number { get; set; }
            public ISet<string> Markers { get; set; }
            public int Count { get; set; }
            public IList<LineAnnotation> LineAnnotations { get; set; }
        }

        private class LineAnnotation
        {
            public string Type { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
            public FileReference[] References { get; set; }
        }

        private void WriteSource(StreamWriter sw, File file, TypeDefinition[] types)
        {
            var sourceLines = new string[0];
            try
            {
                sourceLines = System.IO.File.ReadAllLines(file.Name);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("File read error {0}: {1}", file.Name, ex.Message);
                sw.WriteLine("<div><div class='code'>File read error {0}</div></div>", WebUtility.HtmlEncode(ex.Message));
            }

            List<LineAnnotation> endAnnotations;
            List<LineAnnotation> beginAnnotations;
            Line[] lines;
            CreateFileModel(file, sourceLines, out beginAnnotations, out endAnnotations, out lines);

            sw.WriteLine("<table class='lineAnalysis'>");
            sw.WriteLine("<thead><tr><th></th><th>#</th><th>Line</th><th>Source</th></tr></thead>");
            sw.WriteLine("<tbody>");

            foreach (var annotation in beginAnnotations)
            {
                WriteAnnotationDiv(sw, annotation);
            }

            foreach (var line in lines)
            {
                WriteSourceLine(sw, line);
            }

            foreach (var annotation in endAnnotations)
            {
                WriteAnnotationDiv(sw, annotation);
            }

            sw.WriteLine("</tbody>");
            sw.WriteLine("</table>");
        }

        private void WriteSourceLine(StreamWriter sw, Line line)
        {
            sw.WriteLine("<tr class='{0}' id='line{1}'><td class='colormargin'>&nbsp;</td>" +
                         "<td class='leftmargin rightmargin right'><div class='code'>{3}</code></td>" +
                         "<td class='rightmargin right'><div class='code'>{1}</dic></td>" +
                         "<td class='codecolor'><div class='code'>{2}</div></td></tr>", 
                         line.Markers != null && line.Markers.Count != 0 ? string.Join(" ", line.Markers) : "none", 
                         line.Number,
                         trimSpaces.Replace( WebUtility.HtmlEncode( line.Source ), "&nbsp;" ), 
                         line.Count != 0 ? line.Count.ToString() : "");

            foreach (var annotation in line.LineAnnotations)
            {
                WriteAnnotationDiv(sw, annotation);
            }
        }

        private static void CreateFileModel(
            File file, 
            string[] sourceLines, 
            out List<LineAnnotation> beginAnnotations,
            out List<LineAnnotation> endAnnotations, 
                out Line[] lines)
        {
            beginAnnotations = new List<LineAnnotation>();
            endAnnotations = new List<LineAnnotation>();
            lines = sourceLines.Select((x, i) => new Line() {Source = x, Number = i + 1}).ToArray();

            foreach (var annotation in file.Annotations)
            {
                var lineAnnotation = new LineAnnotation()
                {
                    Category = annotation.Category,
                    Message = annotation.Message,
                    References = annotation.References,
                    Type = annotation.Type,
                };
                if (annotation.Line <= 0)
                {
                    beginAnnotations.Add(lineAnnotation);
                }
                else if (annotation.Line > lines.Length)
                {
                    endAnnotations.Add(lineAnnotation);
                }
                else
                {
                    var size = annotation.Size > 0 ? annotation.Size : 1;
                    for (var i = annotation.Line; i < annotation.Line + size; i++)
                    {
                        var line = lines[i-1];
                        line.Count++;
                        line.Markers.Add(annotation.Type);
                    }
                    lines[Math.Min(annotation.Line + size - 2, lines.Length-1)].LineAnnotations.Add(lineAnnotation);
                }
            }
        }

        private void WriteAnnotationDiv(StreamWriter sw, LineAnnotation annotation)
        {
            sw.WriteLine(
                "<tr><td colspan='3'>&nbsp;</td><td><div class='annotationdiv {0}'><div class='code'>{1}<br/>{2}</div>",
                annotation.Type,
                annotation.Category,
                WebUtility.HtmlEncode(annotation.Message));
            if (annotation.References != null && annotation.References.Length > 0)
            {
                sw.WriteLine("<ul><li class='heading'>See also:</li>");
                foreach (var reference in annotation.References)
                {
                    var filename = GetFileName(reference.Name) + ".html#line" + reference.Line;
                    
                    string[] lines = null;
                    try
                    {
                        lines = System.IO.File.ReadAllLines(reference.Name);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("Read reference error {0}: {1}", reference.Name, ex.Message);
                    }

                    if(lines == null)
                        continue;
                    
                    sw.WriteLine(
                        "<li><a href='{0}'>{1}</a><br/>", 
                        filename, 
                        WebUtility.HtmlEncode(reference.Name));
                    
                    sw.WriteLine("<div class='annotationdiv {0}'><table class='lineAnalysis'><tbody>", annotation.Type);

                    var size = reference.Size > 0 ? reference.Size : 1;
                    for(var i = reference.Line; (i < reference.Line + size) && i-1 < lines.Length && i >= 1; i++)
                    {
                        sw.WriteLine(
                            "<tr><td class='rightmargin right'><div class='code'>{0}</div></td><td><code>{1}</code></td></tr>", 
                            i,
                            WebUtility.HtmlEncode(lines[i-1]).Replace(" ", "&nbsp;"));
                    }
                    sw.WriteLine("</tbody></table></div>");
                    sw.WriteLine("</li>");
                }
                
                sw.WriteLine("</ul>");
            }
            sw.WriteLine("</td></tr>");
        }

        private void WriteHeader(StreamWriter sw, File file, TypeDefinition[] types)
        {
            var encodedfilename = WebUtility.HtmlEncode(file.Name);
            var total = file.Annotations.Length;

            sw.WriteLine("<html>");
            sw.WriteLine("<head>");
            sw.WriteLine("<meta charset='utf-8' />");
            sw.WriteLine("<title>{0} Report</title>", encodedfilename);
            sw.WriteLine("<link rel='stylesheet' type='text/css' href='report.css' />");
            sw.WriteLine("</head><body><div class='container'>");
            sw.WriteLine("<h1>Summary</h1>");
            sw.WriteLine("<table class='overview'>");
            sw.WriteLine("<colgroup>");
            sw.WriteLine("<col width='160' />");
            sw.WriteLine("<col />");
            sw.WriteLine("</colgroup>");
            sw.WriteLine("<tbody>");
            sw.WriteLine("<tr><th>File:</th><td>" + encodedfilename + "</td></tr>");

            var stats = file.Annotations.Select(x => x.Type)
                .GroupBy(x => x)
                .Select(x => new {Type = types.First(y => y.Name == x.Key).Display, Count = x.Count()})
                .OrderBy(x => x.Type)
                .ToArray();
            foreach (var stat in stats)
            {
                sw.WriteLine("<tr><th>{0}:</th><td>{1}</td></tr>", WebUtility.HtmlEncode(stat.Type), stat.Count);
            }
            if (stats.Length != 1)
            {
                sw.WriteLine("<tr><th>Total:</th><td>" + total + "</td></tr>");
            }
            sw.WriteLine("</tbody>");
            sw.WriteLine("</table>");
            sw.WriteLine("<h1>Sourcode</h1>");
        }

        private void WriteFooter(StreamWriter sw)
        {
            sw.WriteLine("<div class='footer'>Generated by: HtmlWarningReportGenerator " + Assembly.GetEntryAssembly().GetName().Version + "<br />" +
                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "<br /><a href='https://github.com/rjasica/HtmlWarningsReportGenerator'>Homepage</a></div></div>");
            sw.WriteLine("</body></html>");
        }

        private string GetFileName(string name)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(name, "_");
        }
    }
}
