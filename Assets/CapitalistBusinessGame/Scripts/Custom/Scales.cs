using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ScalesCollection")]
public class Scales
{
    [XmlArray("Scales")]
    [XmlArrayItem("Name")]
    public List<string> scales = new List<string>();
}