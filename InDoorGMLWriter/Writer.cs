using CityGML.Core;
using CityGML.Core.DTO.Json;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InDoorGMLWriter
{
    public partial class Writer
    {
        //Declare name space for CityGML
        XNamespace xmlns = "";
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        Dictionary<XName, XNamespace> xmlnamespace = new Dictionary<XName, XNamespace>
            {
                { XNamespace.Xmlns + ObjName.gml.ToString(), "http://www.opengis.net/gml/3.2" },
                { XNamespace.Xmlns + ObjName.xlink.ToString(), "http://www.w3.org/1999/xlink" },
                { XNamespace.Xmlns + ObjName.core.ToString(), "http://www.opengis.net/indoorgml/1.0/core" },
                { XNamespace.Xmlns + ObjName.navi.ToString(), "http://www.opengis.net/indoorgml/1.0/navigation" },
                { XNamespace.Xmlns + ObjName.xsi.ToString(), "http://www.w3.org/2001/XMLSchema-instance" }
            };
        private string path;
        List<CellSpaceMember> cellSpaces;
        List<CellSpaceBoundaryMember> cellSpaceBoundaries;
        List<TransitionMember> transitions;
        List<StateMember> states;
        public Writer(string path, List<CellSpaceMember> cellSpaces, List<CellSpaceBoundaryMember> cellSpaceBoundaries, List<TransitionMember> transitions, List<StateMember> states) 
        {
            this.path = path;
            this.cellSpaces = cellSpaces;
            this.cellSpaceBoundaries = cellSpaceBoundaries;
            this.transitions = transitions;
            this.states = states;
        }
        public void Save()
        {
            var Root = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.IndoorFeatures.ToString());
            var boundeBy = CreateBoundeBy();
            var primalSpaceFeatures = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.primalSpaceFeatures.ToString());
            var PrimalSpaceFeatures = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.PrimalSpaceFeatures.ToString());

            //multiLayeredGraph
            var multiLayeredGraph = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.multiLayeredGraph.ToString());
            var MultiLayeredGraph = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.MultiLayeredGraph.ToString());
            var spaceLayers = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.spaceLayers.ToString());
            var spaceLayerMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.spaceLayerMember.ToString());
            var SpaceLayer = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.SpaceLayer.ToString());
            var nodes = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.nodes.ToString());
            var edges = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.edges.ToString());

            Root.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), Guid.NewGuid()));

            //Start writing name space attribute
            foreach (var ns in xmlnamespace)
            {
                Root.Add(new XAttribute(ns.Key, ns.Value));
            }
            string schemaLocation = "http://www.opengis.net/indoorgml/1.0/core http://schemas.opengis.net/indoorgml/1.0/indoorgmlcore.xsd http://www.opengis.net/indoorgml/1.0/navigation http://schemas.opengis.net/indoorgml/1.0/indoorgmlnavi.xsd";
            Root.Add(new XAttribute(xsi + ObjName.schemaLocation.ToString(), schemaLocation));

            primalSpaceFeatures.Add(PrimalSpaceFeatures);
            PrimalSpaceFeatures.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), Guid.NewGuid()));
            PrimalSpaceFeatures.Add(CreateBoundeBy());

            //Add cell space members
            foreach (var cellSpace in CreateCellSpaceMembers(cellSpaces))
            {
                PrimalSpaceFeatures.Add(cellSpace);
            }

            //Add cell Space Boundary Members
            foreach (var cellSpaceB in CreateCellSpaceBoundaryMembers(cellSpaceBoundaries))
            {
                PrimalSpaceFeatures.Add(cellSpaceB);
            }
            
            nodes.Add(CreateBoundeBy());
            //Add state members
            foreach (var state in CreateStateMembers(states))
            {
                nodes.Add(state);
            }

            edges.Add(CreateBoundeBy());
            //Add transition members
            foreach (var transition in CreateTransitionMembers(transitions))
            {
                edges.Add(transition);
            }

            SpaceLayer.Add(CreateBoundeBy());
            nodes.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), Guid.NewGuid()));
            SpaceLayer.Add(nodes);
            edges.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), Guid.NewGuid()));
            SpaceLayer.Add(edges);
            SpaceLayer.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), "base"));
            spaceLayerMember.Add(SpaceLayer);
            spaceLayers.Add(CreateBoundeBy());
            spaceLayers.Add(spaceLayerMember);
            MultiLayeredGraph.Add(CreateBoundeBy());
            spaceLayers.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), Guid.NewGuid()));
            MultiLayeredGraph.Add(spaceLayers);
            MultiLayeredGraph.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), Guid.NewGuid()));
            multiLayeredGraph.Add(MultiLayeredGraph);
            Root.Add(boundeBy);
            Root.Add(primalSpaceFeatures);
            Root.Add(multiLayeredGraph);

            Root.Save(path);
        }

        private XElement CreateBoundeBy()
        {
            var boundeBy = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.boundedBy.ToString());
            boundeBy.Add(new XAttribute(xsi + ObjName.nil.ToString(), "true"));
            return boundeBy;
        }
    }
}
