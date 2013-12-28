using System.Xml.Serialization;

namespace HWRG.Input
{
    [XmlType(AnonymousType = true)]
    public class TypeDefinition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("display")]
        public string Display { get; set; }

        [XmlAttribute("color")]
        public string Color { get; set; }

        [XmlAttribute("background")]
        public string Background { get; set; }
    }
}