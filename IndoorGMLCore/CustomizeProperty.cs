using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IndoorGML.Core
{
	[XmlRoot(ElementName = "property")]
	public class Property
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlText]
		public string Text { get; set; }

		public Property(){ }
		public Property(string _name, string _text)
        {
			Name = _name;
			Text = _text;
		}
	}

	[XmlRoot(ElementName = "space")]
	public class Space
	{
		public Space()
        {
			Property = new List<Property>();
		}
		public Space(string _id, List<Property> _property)
		{
			Property = _property;
			Id = _id;
		}
		[XmlElement(ElementName = "property")]
		public List<Property> Property { get; set; }

		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "customizeProperty")]
	public class CustomizeProperty
	{
		public CustomizeProperty()
        {
			Space = new List<Space>();
		}
		public CustomizeProperty(List<Space> _space)
		{
			Space = _space;
		}
		[XmlElement(ElementName = "space")]
		public List<Space> Space { get; set; }
	}
}
