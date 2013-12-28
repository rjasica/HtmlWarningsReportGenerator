using System.Xml.Serialization;

namespace HWRG.Input
{
    [XmlType(AnonymousType = true)]
    public class Annotation
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("line")]
        public int Line { get; set; }

        [XmlAttribute("size")]
        public int Size { get; set; }

        [XmlAttribute("category")]
        public string Category { get; set; }

        [XmlAttribute("message")]
        public string Message { get; set; }

        [XmlElement("reference")]
        public FileReference[] References { get; set; }
    }
}