using System.Xml.Serialization;

namespace HWRG.Input
{
    [XmlType(AnonymousType = true)]
    public class File
    {
        [XmlElement("annotation")]
        public Annotation[] Annotations { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}