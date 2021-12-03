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
        private List<XElement> CreateTransitionMembers(List<TransitionMember> transitions)
        {
            var transitionMembers = new List<XElement>();
            if(transitions != null)
            {
                foreach (var transition in transitions)
                {
                    if (transition.LineStringGeo == null || transition.LineStringGeo.Positions == null || transition.LineStringGeo.Positions.Count == 0)
                        continue;

                    var transitionMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.transitionMember.ToString());
                    var Transition = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.Transition.ToString());
                    var description = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.description.ToString());
                    var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                    var weight = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.weight.ToString());
                    var connects = CreateConnects(transition.Connects);
                    var duality = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.duality.ToString());
                    var geometry = CreateGeometry(transition);

                    Transition.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), transition.Id));
                    transitionMember.Add(Transition);
                    description.Add(transition.Description);
                    Transition.Add(description);
                    name.Add(transition.Name);
                    Transition.Add(name);
                    Transition.Add(CreateBoundeBy());
                    weight.Add(transition.Weight);
                    Transition.Add(weight);
                    foreach (var connect in connects)
                    {
                        Transition.Add(connect);
                    }
                    duality.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), transition.Duality));
                    Transition.Add(duality);
                    Transition.Add(geometry);

                    transitionMembers.Add(transitionMember);
                }
            }

            return transitionMembers;
        }
        private XElement CreateGeometry(TransitionMember transition)
        {
            var geometry = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.geometry.ToString());
            var LineString = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.LineString.ToString());
            var posList = CreatePosList(transition.LineStringGeo.Positions);

            LineString.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), transition.LineStringGeo.Id));
            geometry.Add(LineString);
            foreach (var pos in posList)
            {
                geometry.Add(pos);
            }
            return geometry;

        }
    }
}
