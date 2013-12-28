using System.Xml.Serialization;

namespace HWRG.Input
{
    [XmlType(AnonymousType = true)]
    public class FileReference
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("line")]
        public int Line { get; set; }

        [XmlAttribute("size")]
        public int Size { get; set; }
    }
}