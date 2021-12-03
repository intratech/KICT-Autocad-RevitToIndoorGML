/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndoorGML.Core
{
	
	[XmlRoot(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
	public class BoundedBy
	{
		[XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
		public string Nil { get; set; }

		public BoundedBy()
        {
			Nil = "true";
        }

		public BoundedBy(string nil)
		{
			Nil = nil;
		}
	}

	[XmlRoot(ElementName = "pos", Namespace = "http://www.opengis.net/gml/3.2")]
	public class Pos
	{

		[XmlIgnore]
		public double x;
		[XmlIgnore]
		public double y;
		[XmlIgnore]
		public double z;


        public Pos(double x, double y, double z)
        {
			SrsDimension = "3";
			this.x = x;
			this.y = y;
			this.z = z;

			text = $"{Math.Round(x, 5)} {Math.Round(y, 5)} {Math.Round(z, 5)}";

		}

		public double DistanceTo(Pos other)
        {
			return Math.Sqrt((x - other.x)* (x - other.x) + (y - other.y)* (y - other.y) + (z - other.z)* (z - other.z));
        }

		public Pos()
        {

        }

        [XmlAttribute(AttributeName = "srsDimension")]
		public string SrsDimension { get; set; }


		private string text;
		[XmlText]
		public string Text
        {
			get => text;
            set
            {
				text = value;
				try
				{
					var _pos = text.Split(' ');
					x = double.Parse(_pos[0]);
					y = double.Parse(_pos[1]);
					z = double.Parse(_pos[2]);
				}
				catch { }
			}
		}

        public Pos Clone()
        {
			return new Pos(x, y, z);
        }

        public override int GetHashCode()
        {
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
		}

        internal double GetElevation()
        {
			return z;
		}

        public void Set(Pos newPos)
        {
			SrsDimension = "3";
			this.x = newPos.x;
			this.y = newPos.y;
			this.z = newPos.z;

			text = $"{Math.Round(x, 5)} {Math.Round(y, 5)} {Math.Round(z, 5)}";
		}
    }

	[XmlRoot(ElementName = "LinearRing", Namespace = "http://www.opengis.net/gml/3.2")]
	public class LinearRing
	{
		[XmlElement(ElementName = "pos", Namespace = "http://www.opengis.net/gml/3.2")]
		public List<Pos> Pos { get; set; }
	}

	[XmlRoot(ElementName = "exterior", Namespace = "http://www.opengis.net/gml/3.2")]
	public class Exterior
	{
		[XmlElement(ElementName = "LinearRing", Namespace = "http://www.opengis.net/gml/3.2")]
		public LinearRing LinearRing { get; set; }
		[XmlElement(ElementName = "Shell", Namespace = "http://www.opengis.net/gml/3.2")]
		public Shell Shell { get; set; }
	}

	[XmlRoot(ElementName = "Polygon", Namespace = "http://www.opengis.net/gml/3.2")]
	public class Polygon
	{
		[XmlElement(ElementName = "exterior", Namespace = "http://www.opengis.net/gml/3.2")]
		public Exterior Exterior { get; set; }

		[XmlAttributeAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "surfaceMember", Namespace = "http://www.opengis.net/gml/3.2")]
	public class SurfaceMember
	{
		[XmlElement(ElementName = "Polygon", Namespace = "http://www.opengis.net/gml/3.2")]
		public Polygon Polygon { get; set; }

		public SurfaceMember()
        {
			Polygon = new Polygon();
        }
	}

	[XmlRoot(ElementName = "geometry3D", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Geometry3DCellSpace
	{
		[XmlElement(ElementName = "Polygon", Namespace = "http://www.opengis.net/gml/3.2")]
		public Polygon Polygon { get; set; }
	}

	[XmlRoot(ElementName = "Shell", Namespace = "http://www.opengis.net/gml/3.2")]
	public class Shell
	{
		[XmlElement(ElementName = "surfaceMember", Namespace = "http://www.opengis.net/gml/3.2")]
		public List<SurfaceMember> SurfaceMember { get; set; }
	}

	[XmlRoot(ElementName = "Solid", Namespace = "http://www.opengis.net/gml/3.2")]
	public class Solid
	{
		[XmlElement(ElementName = "exterior", Namespace = "http://www.opengis.net/gml/3.2")]
		public Exterior Exterior { get; set; }


		[XmlAttributeAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]		
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "Geometry3D", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Geometry3D
	{
		[XmlElement(ElementName = "Solid", Namespace = "http://www.opengis.net/gml/3.2")]
		public Solid Solid { get; set; }
	}

	[XmlRoot(ElementName = "cellSpaceGeometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class CellSpaceGeometry
	{
		[XmlElement(ElementName = "Geometry3D", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Geometry3D Geometry3D { get; set; }
	}

	[XmlRoot(ElementName = "duality", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Duality
	{
		[XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
		public string Href { get; set; }
	}

	[XmlRoot(ElementName = "CellSpace", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class CellSpace
	{
		[XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }

		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }

		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }

		[XmlElement(ElementName = "cellSpaceGeometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public CellSpaceGeometry CellSpaceGeometry { get; set; }

		[XmlElement(ElementName = "duality", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Duality Duality { get; set; }

		[XmlElement(ElementName = "partialboundedBy", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<PartialboundedBy> PartialboundedBy { get; set; }

		[XmlElement(ElementName = "level", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public string Level { get; set; }

		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

		[XmlIgnore]
        public string Usage { get; set; }

		[XmlIgnore]
		private string _class;
		[XmlIgnore]
		public string Class
        {
			get
            {
				if(_class == null)
                {
					//Get from description
					_class = GetClassFromDescription();
                }
				if(_class != null)
					return _class;
				return "";
            }
            set
            {
				_class = value;
            }
        }

        public string GetClassFromDescription()
        {
			if (string.IsNullOrEmpty(Description))
				return "";
			//var match = Regex.Match(description, "class\\s*=\\s*\"([^:] +)\"");
			var match = Regex.Match(Description, "class\\s*=\\s*\"([^:]+)\"");
			
			if (match.Success)
            {
				return match.Result("$1");
            }
			return "";
        }

        [XmlIgnore]
		public string Function { get; set; }

		public CellSpace()
        {
			BoundedBy = new BoundedBy();
			PartialboundedBy = new List<PartialboundedBy>();

		}

		public void AddPartialbounded(ConnectionBoundary connectionBoundary)
		{
			PartialboundedBy.Add(new IndoorGML.Core.PartialboundedBy() { Href = $"#{connectionBoundary.Id}" });

		}
	}

	[XmlRoot(ElementName = "cellSpaceMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class CellSpaceMember
	{
		//[XmlElement(ElementName = "GeneralSpace",  Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
		//public GeneralSpace GeneralSpace { get; set; }

		[XmlElement(ElementName = "CellSpace", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public CellSpace GeneralSpace { get; set; }
	}

	[XmlRoot(ElementName = "GeneralSpace", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
	public class GeneralSpace
	{
		[XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "cellSpaceGeometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public CellSpaceGeometry CellSpaceGeometry { get; set; }
		[XmlElement(ElementName = "duality", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Duality Duality { get; set; }
		[XmlElement(ElementName = "partialboundedBy", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<PartialboundedBy> PartialboundedBy { get; set; }
		[XmlElement(ElementName = "class", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
		public string Class { get; set; }
		[XmlElement(ElementName = "function", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
		public string Function { get; set; }
		[XmlElement(ElementName = "usage", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
		public string Usage { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

		public GeneralSpace()
        {
			Function = "2550";
			Usage = "2550";
			Class = "1020";
			BoundedBy = new BoundedBy();
			PartialboundedBy = new List<PartialboundedBy>();
		}

        public void AddPartialbounded(ConnectionBoundary connectionBoundary)
        {
			PartialboundedBy.Add(new IndoorGML.Core.PartialboundedBy() { Href = $"#{connectionBoundary.Id}" });

		}
    }

	[XmlRoot(ElementName = "partialboundedBy", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class PartialboundedBy
	{
		[XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
		public string Href { get; set; }
	}
	//[XmlRoot(ElementName = "geometry3D", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	//public class Geometry3D
	//{
	//	[XmlElement(ElementName = "Polygon", Namespace = "http://www.opengis.net/gml/3.2")]
	//	public Polygon Polygon { get; set; }
	//}

	[XmlRoot(ElementName = "cellSpaceBoundaryGeometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class CellSpaceBoundaryGeometry
	{
		[XmlElement(ElementName = "geometry3D", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Geometry3DCellSpace Geometry3D { get; set; }
	}

	[XmlRoot(ElementName = "CellSpaceBoundary", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class CellSpaceBoundary
	{
		[XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "duality", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Duality Duality { get; set; }
		[XmlElement(ElementName = "cellSpaceBoundaryGeometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public CellSpaceBoundaryGeometry CellSpaceBoundaryGeometry { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }
		
		public CellSpaceBoundary()
        {
			BoundedBy = new BoundedBy();
        }


	}

	[XmlRoot(ElementName = "cellSpaceBoundaryMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class CellSpaceBoundaryMember
	{
		[XmlElement(ElementName = "ConnectionBoundary", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
		public ConnectionBoundary ConnectionBoundary { get; set; }
		[XmlElement(ElementName = "AnchorBoundary", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
		public AnchorBoundary AnchorBoundary { get; set; }

		public CellSpaceBoundaryMember()
        {
			ConnectionBoundary = new ConnectionBoundary();
        }
	}

	[XmlRoot(ElementName = "AnchorBoundary", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
	public class AnchorBoundary
	{
		[XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "cellSpaceBoundaryGeometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public CellSpaceBoundaryGeometry CellSpaceBoundaryGeometry { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "ConnectionBoundary", Namespace = "http://www.opengis.net/indoorgml/1.0/navigation")]
	public class ConnectionBoundary
	{

        [XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "duality", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Duality Duality { get; set; }
		[XmlElement(ElementName = "cellSpaceBoundaryGeometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public CellSpaceBoundaryGeometry CellSpaceBoundaryGeometry { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "PrimalSpaceFeatures", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class PrimalSpaceFeatures
	{
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "cellSpaceMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<CellSpaceMember> CellSpaceMember { get; set; }
		[XmlElement(ElementName = "cellSpaceBoundaryMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<CellSpaceBoundaryMember> CellSpaceBoundaryMember { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

		public PrimalSpaceFeatures()
        {
			BoundedBy = new BoundedBy();
			Id = "ID_" + IndoorFeatures.GlobalID;

			CellSpaceMember = new List<CellSpaceMember>();
			CellSpaceBoundaryMember = new List<CellSpaceBoundaryMember>();

		}

        internal void Append(PrimalSpaceFeatures primalSpaceFeatures)
        {

			if(primalSpaceFeatures.CellSpaceMember != null)
				CellSpaceMember.AddRange(primalSpaceFeatures.CellSpaceMember);

			if(primalSpaceFeatures.CellSpaceBoundaryMember != null)
            {
				CellSpaceBoundaryMember.AddRange(primalSpaceFeatures.CellSpaceBoundaryMember);
            }
		}
    }

	[XmlRoot(ElementName = "primalSpaceFeatures", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class PrimalSpaceFeaturesOwner
	{
		[XmlElement(ElementName = "PrimalSpaceFeatures", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public PrimalSpaceFeatures PrimalSpaceFeatures { get; set; }

		public PrimalSpaceFeaturesOwner()
        {
			PrimalSpaceFeatures = new PrimalSpaceFeatures();
		}
	}

	[XmlRoot(ElementName = "connects", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Connects
	{
		[XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
		public string Href { get; set; }
	}

	[XmlRoot(ElementName = "Point", Namespace = "http://www.opengis.net/gml/3.2")]
	public class Point
	{
		[XmlElement(ElementName = "pos", Namespace = "http://www.opengis.net/gml/3.2")]
		public Pos Pos { get; set; }
		[XmlAttributeAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "geometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Geometry
	{
		[XmlElement(ElementName = "Point", Namespace = "http://www.opengis.net/gml/3.2")]
		public Point Point { get; set; }
		[XmlElement(ElementName = "LineString", Namespace = "http://www.opengis.net/gml/3.2")]
		public LineString LineString { get; set; }
		[XmlElement(ElementName = "pos", Namespace = "http://www.opengis.net/gml/3.2")]
		public List<Pos> Pos { get; set; }
	}

	[XmlRoot(ElementName = "State", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class State
	{
		[XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "duality", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Duality Duality { get; set; }
		[XmlElement(ElementName = "connects", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<Connects> Connects { get; set; }
		[XmlElement(ElementName = "geometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Geometry Geometry { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

		public State()
        {
			BoundedBy = new BoundedBy();
			Connects = new List<Connects>();

		}

        public void AddConnect(Transition transation)
        {
			Connects.Add(new IndoorGML.Core.Connects() { Href = $"#{transation.Id}" });
        }
    }

	[XmlRoot(ElementName = "stateMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class StateMember
	{
		[XmlElement(ElementName = "State", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public State State { get; set; }

		[XmlIgnore]
        public double Elevation
        {
			get {
				if (State.Geometry.Point != null)
				{
					return State.Geometry.Point.Pos.GetElevation();
				}
				else
                {
					return -1;
                }
			}
		}

        public StateMember()
        {
			State = new State();
        }

        internal bool SamePlace(StateMember other, double tolerance)
        {
			var pos1 = other.State.Geometry.Point.Pos;
			var pos2 = State.Geometry.Point.Pos;
			var distance = Math.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y));
			if (distance < tolerance)
				return true;
			return false;
		}
    }

	[XmlRoot(ElementName = "nodes", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Nodes
	{
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "stateMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<StateMember> StateMember { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

		public Nodes()
        {
			Id = "ID_" + IndoorFeatures.GlobalID;
			BoundedBy = new BoundedBy();
			StateMember = new List<StateMember>();

		}

        internal void Append(Nodes nodes)
        {
			if (nodes == null)
				return;
			if (nodes.StateMember == null)
				return;

			StateMember.AddRange(nodes.StateMember);

		}
    }

	[XmlRoot(ElementName = "LineString", Namespace = "http://www.opengis.net/gml/3.2")]
	public class LineString
	{
		[XmlAttributeAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
		public string Id { get; set; }

		[XmlElement(ElementName = "pos", Namespace = "http://www.opengis.net/gml/3.2")]
		public List<Pos> Pos { get; set; }

		public LineString()
        {
			Pos = new List<Pos>();
        }
	}

	[XmlRoot(ElementName = "Transition", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Transition
	{
		[XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "weight", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public string Weight { get; set; }
		[XmlElement(ElementName = "connects", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<Connects> Connects { get; set; }
		[XmlElement(ElementName = "duality", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Duality Duality { get; set; }
		[XmlElement(ElementName = "geometry", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Geometry Geometry { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

		public Transition()
        {
			
			BoundedBy = new BoundedBy();
        }

        internal void Append(Transition transition)
        {
			Connects.AddRange(transition.Connects);
			Geometry.LineString.Pos.AddRange(transition.Geometry.LineString.Pos);
			Geometry.LineString.Pos = Geometry.LineString.Pos.Distinct(new PosEqualityComparer()).ToList();

		}

		public void AddConnect(string dualityToState)
		{
			if (Connects == null)
				Connects = new List<Connects>();

			Connects.Add(new IndoorGML.Core.Connects() { Href = dualityToState });
        }
    }

	class PosEqualityComparer : IEqualityComparer<Pos>
	{
		public bool Equals(Pos x, Pos y)
		{
			// Two items are equal if their keys are equal.
			return x.DistanceTo(y) < 0.01;
		}

		public int GetHashCode(Pos x)
		{
			return x.GetHashCode();
		}
	}


	[XmlRoot(ElementName = "transitionMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class TransitionMember
	{
		[XmlElement(ElementName = "Transition", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Transition Transition { get; set; }

		public TransitionMember()
        {
			Transition = new Transition();
        }

        public void Merge(TransitionMember transitionMember)
        {
			Transition.Append(transitionMember.Transition);
		}

        public Pos GetStartPos()
        {
			return Transition.Geometry.LineString.Pos[0];
        }

		public Pos GetEndPos()
		{
			int count = Transition.Geometry.LineString.Pos.Count - 1;

			return Transition.Geometry.LineString.Pos[count];
		}

	}

	[XmlRoot(ElementName = "edges", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class Edges
	{
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "transitionMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public List<TransitionMember> TransitionMember { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

        internal void Append(Edges edges)
        {
           if(edges == null)
            {
				return;
            }

		   if(edges.TransitionMember != null)
            {
				TransitionMember.AddRange(edges.TransitionMember);
            }
        }

		public Edges()
        {
			Id = "ID_" + IndoorFeatures.GlobalID;
			BoundedBy = new BoundedBy();
			TransitionMember = new List<TransitionMember>();

		}
    }

	[XmlRoot(ElementName = "SpaceLayer", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class SpaceLayer
	{
        public SpaceLayer()
        {
			Id = "base";
			BoundedBy = new BoundedBy();
			Nodes = new Nodes();
			Edges = new Edges();
        }

        [XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "nodes", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Nodes Nodes { get; set; }
		[XmlElement(ElementName = "edges", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public Edges Edges { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

        internal void Append(SpaceLayer spaceLayer)
        {
			if (spaceLayer == null)
				return; 

            if(Nodes == null)
            {
				Nodes = new Nodes();
            }

			Nodes.Append(spaceLayer.Nodes);

			if(Edges == null)
            {
				Edges = new Edges();
            }

			Edges.Append(spaceLayer.Edges);
        }
    }

	[XmlRoot(ElementName = "spaceLayerMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class SpaceLayerMember
	{
		[XmlElement(ElementName = "SpaceLayer", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public SpaceLayer SpaceLayer { get; set; }

		public SpaceLayerMember()
        {
			SpaceLayer = new SpaceLayer();
		}

        internal void Append(SpaceLayerMember spaceLayerMember)
        {
			if (spaceLayerMember == null)
				return;

			if (SpaceLayer == null)
            {
				SpaceLayer = new SpaceLayer();
            }

			SpaceLayer.Append(spaceLayerMember.SpaceLayer);


		}
    }

	[XmlRoot(ElementName = "spaceLayers", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class SpaceLayers
	{
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "spaceLayerMember", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public SpaceLayerMember SpaceLayerMember { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

        internal void Append(SpaceLayers spaceLayers)
        {
			if (spaceLayers == null)
				return;

			if(SpaceLayerMember == null)
            {
				SpaceLayerMember = new SpaceLayerMember();
			}

			SpaceLayerMember.Append(spaceLayers.SpaceLayerMember);


		}

		public SpaceLayers()
        {
			Id = "ID_" + IndoorFeatures.GlobalID;
			BoundedBy = new BoundedBy();
			SpaceLayerMember = new SpaceLayerMember();

		}
    }

	[XmlRoot(ElementName = "MultiLayeredGraph", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class MultiLayeredGraph
	{
		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "spaceLayers", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public SpaceLayers SpaceLayers { get; set; }
		[XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Id { get; set; }

        internal void Append(MultiLayeredGraph multiLayeredGraph)
        {
			if (multiLayeredGraph == null)
				return;
            if(SpaceLayers == null)
            {
				SpaceLayers = new SpaceLayers();
            }

			SpaceLayers.Append(multiLayeredGraph.SpaceLayers);
        }

		public MultiLayeredGraph()
        {
			BoundedBy = new BoundedBy();
			Id = "ID_" + IndoorFeatures.GlobalID;
			SpaceLayers = new SpaceLayers();

		}
	}

	[XmlRoot(ElementName = "multiLayeredGraph", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class MultiLayeredGraphOnwer
	{
		[XmlElement(ElementName = "MultiLayeredGraph", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public MultiLayeredGraph MultiLayeredGraph { get; set; }

		public MultiLayeredGraphOnwer()
        {
			MultiLayeredGraph = new MultiLayeredGraph();

		}

        internal void Append(MultiLayeredGraphOnwer multiLayeredGraph)
        {
			if (multiLayeredGraph == null)
				return;
			MultiLayeredGraph.Append(multiLayeredGraph.MultiLayeredGraph);

		}
    }

	[XmlRoot(ElementName = "IndoorFeatures", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
	public class IndoorFeatures
	{
		private static int id = 0;
		public static int GlobalID
		{
			get
			{
				return ++id;
			}
			set
			{
				id = 0;
			}
		}
		[XmlElement(ElementName = "description", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://www.opengis.net/gml/3.2")]
		public string Name { get; set; }


		[XmlElement(ElementName = "boundedBy", Namespace = "http://www.opengis.net/gml/3.2")]
		public BoundedBy BoundedBy { get; set; }
		[XmlElement(ElementName = "primalSpaceFeatures", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public PrimalSpaceFeaturesOwner PrimalSpaceFeatures { get; set; }
		[XmlElement(ElementName = "multiLayeredGraph", Namespace = "http://www.opengis.net/indoorgml/1.0/core")]
		public MultiLayeredGraphOnwer MultiLayeredGraph { get; set; }

		[XmlAttributeAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
		public string Id { get; set; }
		//[XmlAttribute(AttributeName = "gml", Namespace = "http://www.w3.org/2000/xmlns/")]
		//public string Gml { get; set; }
		//[XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
		//public string Xlink { get; set; }
		//[XmlAttribute(AttributeName = "core", Namespace = "http://www.w3.org/2000/xmlns/")]
		//public string Core { get; set; }
		//[XmlAttribute(AttributeName = "navi", Namespace = "http://www.w3.org/2000/xmlns/")]
		//public string Navi { get; set; }
		//[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
		//public string Xsi { get; set; }
		[XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
		public string SchemaLocation { get; set; }

		public IndoorFeatures()
        {
			IndoorFeatures.GlobalID = 0;

			//Xsi = "http://www.w3.org/2001/XMLSchema-instance";
			//Gml = "http://www.opengis.net/gml/3.2";
			Id = "ID_" + IndoorFeatures.GlobalID;
			//Core = "http://www.opengis.net/indoorgml/1.0/core";
			//Navi = "http://www.opengis.net/indoorgml/1.0/navigation";
			//SchemaLocation = "http://www.opengis.net/indoorgml/1.0/core http://schemas.opengis.net/indoorgml/1.0/indoorgmlcore.xsd http://www.opengis.net/indoorgml/1.0/navigation http://schemas.opengis.net/indoorgml/1.0/indoorgmlnavi.xsd";
			//Xlink = "http://www.w3.org/1999/xlink";

			BoundedBy = new BoundedBy();
			BoundedBy.Nil = "true";

			PrimalSpaceFeatures = new PrimalSpaceFeaturesOwner();

			MultiLayeredGraph = new MultiLayeredGraphOnwer();

		}

		public CellSpaceMember AddCellSpace(CellSpaceMember cellSpace)
        {
			PrimalSpaceFeatures.PrimalSpaceFeatures.CellSpaceMember.Add(cellSpace);
			return cellSpace;

		}

		public CellSpaceBoundaryMember AddCellSpaceBoundarMember(CellSpaceBoundaryMember cellSpaceMember)
		{
			PrimalSpaceFeatures.PrimalSpaceFeatures.CellSpaceBoundaryMember.Add(cellSpaceMember);
			return cellSpaceMember;
		}
		internal void Append(IndoorFeatures indoor)
        {
            if(this.PrimalSpaceFeatures == null)
            {
				this.PrimalSpaceFeatures = new PrimalSpaceFeaturesOwner();				
			}

			if(this.MultiLayeredGraph == null)
            {
				this.MultiLayeredGraph = new MultiLayeredGraphOnwer();
			}

			if(indoor.PrimalSpaceFeatures != null && indoor.PrimalSpaceFeatures.PrimalSpaceFeatures != null)
            {
				this.PrimalSpaceFeatures.PrimalSpaceFeatures.Append(indoor.PrimalSpaceFeatures.PrimalSpaceFeatures);
            }

			if(indoor.MultiLayeredGraph != null)
            {
				this.MultiLayeredGraph.Append(indoor.MultiLayeredGraph);

			}
        }

		public void Save(string output)
        {
			XmlSerializer xmlReader = new XmlSerializer(typeof(IndoorFeatures));
			SchemaLocation= "http://www.opengis.net/indoorgml/1.0/core http://www.indoorgml.net/extensions/indoorgmlcore.xsd";
			using (StreamWriter os = new StreamWriter(output, false, Encoding.UTF8))
			{
				xmlReader.Serialize(os, this, IndoorGMLUtility.GetNamespaces());
			}
		}

        public StateMember AddStateMember(StateMember stateMember)
        {
			MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Nodes.StateMember.Add(stateMember);
			return stateMember;

		}

        public void AddConnectionSpaceToDoor(int fromRoom, int toWindow)
        {
			var cellSpace = PrimalSpaceFeatures.PrimalSpaceFeatures.CellSpaceMember[fromRoom];
			var toDoor = PrimalSpaceFeatures.PrimalSpaceFeatures.CellSpaceBoundaryMember[toWindow];
			cellSpace.GeneralSpace.PartialboundedBy.Add(new PartialboundedBy() { Href = $"#{toDoor.ConnectionBoundary.Id}" });

        }

        public TransitionMember AddTransition(TransitionMember transitionMember)
        {
			var transitions = MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Edges.TransitionMember;

			
			var added = transitions.Where(x => x.Transition.Id == transitionMember.Transition.Id).FirstOrDefault();
			if(added != null)
            {
				added.Merge(transitionMember);
            }
			else
            {
				transitions.Add(transitionMember);
			}
			return transitionMember;
        }

       

		public StateMember GetState(int fromRoom)
		{
			return MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Nodes.StateMember[fromRoom];


		}

        public StateMember GetState(string stateID)
        {
			var states= MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Nodes;
			return states.StateMember.Where(x => x.State.Id == stateID).FirstOrDefault();
		}

        public void AddTransition(Transition transation)
        {
			TransitionMember member = new TransitionMember();
			member.Transition = transation;
			AddTransition(member);
        }

        internal void ConnectFloors(string className, string function, string usage,double tolerance)
        {
			var listSpaces = PrimalSpaceFeatures.PrimalSpaceFeatures.CellSpaceMember.Where(x => x.GeneralSpace!= null&& x.GeneralSpace.Class.IndexOf( className,StringComparison.OrdinalIgnoreCase)>=0).ToList();
			var nodes = MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Nodes.StateMember;

			List<StateMember> members = new List<StateMember>();
			listSpaces.ForEach(x =>
			{
				if (!string.IsNullOrEmpty(x.GeneralSpace.Duality.Href))
				{
					string duality = x.GeneralSpace.Duality.Href.Replace("#", "");
					foreach (var state in nodes.Where(y => y.State.Id == duality))
					{
						if (state != null)
							members.Add(state);
					}
				}
			});

			members.OrderBy(x => x.Elevation);
			for(int i=0;i<members.Count;i++)
            {
				var current = members[i];
				for(int j=i+1;j<members.Count;j++)
                {
					var next = members[j];
					if(current.Elevation != next.Elevation&& current.SamePlace(next,tolerance))
                    {
						CreateTransition(current, next);
						break;
                    }
                }
            }

		}

        private void CreateTransition(StateMember current, StateMember next)
        {
			var edges = MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Edges;
			TransitionMember transition = new TransitionMember();
			transition.Transition.Id = $"T_{current.State.Id}_{next.State.Id}";
			transition.Transition.AddConnect($"#{current.State.Id}");
			transition.Transition.AddConnect($"#{next.State.Id}");
			current.State.Connects.Add(new Connects() { Href = $"#{transition.Transition.Id}" });
			next.State.Connects.Add(new Connects() { Href = $"#{transition.Transition.Id}" });

			transition.Transition.Geometry = new Geometry();
			transition.Transition.Geometry.LineString = new LineString();
			transition.Transition.Geometry.LineString.Id = $"TG-{transition.Transition.Id}";
			transition.Transition.Geometry.LineString.Pos.Add(current.State.Geometry.Point.Pos);
			transition.Transition.Geometry.LineString.Pos.Add(next.State.Geometry.Point.Pos);

			edges.TransitionMember.Add(transition);

		}

		public IEnumerable<CellSpaceMember> GetConnectedSpace(string transitionID)
        {
			var edges = MultiLayeredGraph?.MultiLayeredGraph?.SpaceLayers?.SpaceLayerMember?.SpaceLayer?.Edges;
			if (edges != null)
			{
				var transition = edges.TransitionMember.Where(x => x.Transition.Id == transitionID).FirstOrDefault();
				if (transition == null)
					yield break;
				if (transition.Transition?.Connects == null)
					yield break;

				foreach (var connect in transition.Transition.Connects)
				{
					CellSpaceMember room=null;
					try
					{
						var state = GetState(connect.Href.Replace("#", ""));
						 room = GetGeneralSpace(state.State.Duality.Href.Replace("#", ""));
						
					}
					catch { }
					if(room != null)
                    {
						yield return room;
                    }
				}
			}
		}
        public CellSpaceMember GetGeneralSpace(string id)
        {
			return PrimalSpaceFeatures.PrimalSpaceFeatures.CellSpaceMember
					.Where(x => x.GeneralSpace != null && x.GeneralSpace.Id == id).FirstOrDefault();
		}

        public static IndoorFeatures OpenFile(string file)
        {
			XmlSerializer xmlTool = new XmlSerializer(typeof(IndoorFeatures));

			if (File.Exists(file))
			{
				using (FileStream fs = new FileStream(file, FileMode.Open))
				{
					return xmlTool.Deserialize(fs) as IndoorFeatures;				
				}
			}

			return null;
		}

        public TransitionMember GetTransitionById(string transitionID)
        {
			var edges = MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Edges;
			return edges.TransitionMember.Where(x => x.Transition.Id == transitionID).FirstOrDefault();
        }

        public StateMember GetNode(string id)
        {
			var nodes = MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Nodes;
			return nodes.StateMember.Where(x => x.State.Id == id).FirstOrDefault();
		}

        public List<TransitionMember> GetAllEdges()
        {
			var edges = MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Edges.TransitionMember;
			return edges;
		}

        public void AddEdge(TransitionMember member)
        {
			var edges = MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Edges.TransitionMember;

			edges.Add(member);
		}

		public void SaveSpaceDescription(CustomizeProperty _customizeProperty)
        {
			foreach (Space space in _customizeProperty.Space)
			{
				CellSpaceMember cellSpaceMember = GetGeneralSpace(space.Id);
				if (cellSpaceMember != null)
				{
					string descriptionData = "";
					//We have to keep function and class in cellspace in current data(Original model always have)
					//EX : function ="cellSpace Function" : class="cell Space Class." 
					string[] arraySubData = cellSpaceMember.GeneralSpace.Description.Split(':');
					string key;
					foreach(string subData in arraySubData)
                    {
						key = subData.Split('=')[0];

						if (key.Contains("function") || key.Contains("class"))
                        {
							descriptionData += subData + ":";
						}
                    }

					//Save new data
					foreach (Property property in space.Property)
					{
						descriptionData += property.Name + "=\"" + property.Text + "\":";
					}
					cellSpaceMember.GeneralSpace.Description = descriptionData;
				}
			}
		}
		
		public CustomizeProperty LoadSpaceDescription()
		{
			CustomizeProperty customizeProperty = new CustomizeProperty();
			string descriptionData;
			foreach (CellSpaceMember cellSpaceMember in PrimalSpaceFeatures.PrimalSpaceFeatures.CellSpaceMember)
            {
				List<Property> listProperty = new List<Property>();
				if(cellSpaceMember.GeneralSpace != null)
				{
					descriptionData = cellSpaceMember.GeneralSpace.Description;
					//Split data by character ":"
					if (descriptionData != null)
					{
						string[] arraySubData = descriptionData.Split(':');
						foreach (string subData in arraySubData)
						{
							if (subData != "" && subData.Contains("="))
							{
								Property property = new Property();
								string[] keyAndValue = subData.Split('=');
								property.Name = keyAndValue[0];
								property.Text = keyAndValue[1].Replace("\"", "");
								listProperty.Add(property);
							}
						}
					}

					//Add new space data
					Space space = new Space();
					space.Id = cellSpaceMember.GeneralSpace.Id;
					space.Property = listProperty;
					customizeProperty.Space.Add(space);
				}
			}

			return customizeProperty;
		}

		public void SaveTransitionDescription(CustomizeProperty _customizeProperty)
		{
			foreach (Space space in _customizeProperty.Space)
			{
				TransitionMember transitionMember = GetTransitionById(space.Id);
				if (transitionMember != null)
				{
					string descriptionData = "";
					foreach (Property property in space.Property)
					{
						descriptionData += property.Name + "=\"" + property.Text + "\":";
					}
					transitionMember.Transition.Description = descriptionData;
				}
			}
		}

		public CustomizeProperty LoadTransitionDescription()
		{
			CustomizeProperty customizeProperty = new CustomizeProperty();
			string descriptionData;
			foreach (TransitionMember transitionMember in MultiLayeredGraph.MultiLayeredGraph.SpaceLayers.SpaceLayerMember.SpaceLayer.Edges.TransitionMember)
			{
				List<Property> listProperty = new List<Property>();
				descriptionData = transitionMember.Transition.Description;
				if(descriptionData != null)
				{
					//Split data by character ":"
					string[] arraySubData = descriptionData.Split(':');
					foreach (string subData in arraySubData)
					{
						if (subData != "" && subData.Contains("="))
						{
							Property property = new Property();
							string[] keyAndValue = subData.Split('=');
							property.Name = keyAndValue[0];
							property.Text = keyAndValue[1].Replace("\"", "");
							listProperty.Add(property);
						}
					}
				}

				//Add new space data
				Space space = new Space();
				space.Id = transitionMember.Transition.Id;
				space.Property = listProperty;
				customizeProperty.Space.Add(space);
			}

			return customizeProperty;
		}
	}

}
