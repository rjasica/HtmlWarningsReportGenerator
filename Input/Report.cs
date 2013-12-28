using System.Xml.Serialization;

namespace HWRG.Input
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "report")]
    public class Report
    {
        [XmlArray("types")]
        [XmlArrayItem("type", IsNullable = false)]
        public TypeDefinition[] Types { get; set; }

        [XmlArray("files")]
        [XmlArrayItem("file", IsNullable = false)]
        public File[] Files { get; set; }
    }
}