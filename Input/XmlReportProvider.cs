using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Common.Logging;

namespace HWRG.Input
{
    public class XmlReportProvider : IReportProvider
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        public Report GetReport(string path)
        {
            var fullPath = Path.GetFullPath(path);
            Log.InfoFormat("Reading {0}", fullPath);

            try
            {
                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    var readerSettings = new XmlReaderSettings() {ConformanceLevel = ConformanceLevel.Document};
                    var reader = XmlReader.Create(fs, readerSettings);
                    var serializer = new XmlSerializer(typeof (Report));
                    return serializer.Deserialize(reader) as Report;
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
                throw new AppException("File read error: {0}", ex, ErrorCodes.Read);
            }
        }
    }
}
