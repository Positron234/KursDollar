using System.Xml.Serialization;

namespace KursDollar.Model
{

    [XmlRoot("ValCurs")]
    public class ValCurs
    {
        [XmlElement("Record")]
        public required Record[] Records { get; set; }
    }

    public class Record
    {
        [XmlAttribute("Date")]
        public required string DateTime { get; set; }
        public required string Value { get; set; }
        public required string VunitRate { get; set; }
    }
}
