using CityGML.Core;
using CityGML.Core.DTO.Json;
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
        private List<XElement> CreateStateMembers(List<StateMember> states)
        {
            var stateMembers = new List<XElement>();
            if(states != null)
            {
                foreach (var state in states)
                {
                    if (state.PointGeo == null || state.PointGeo.Position == null)
                        continue;

                    var stateMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.stateMember.ToString());
                    var State = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.State.ToString());
                    var description = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.description.ToString());
                    var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                    var duality = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.duality.ToString());
                    var connects = CreateConnects(state.Connects);
                    var geometry = CreateGeometry(state);

                    State.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), state.Id));
                    stateMember.Add(State);
                    description.Add(state.Description);
                    State.Add(description);
                    name.Add(state.Name);
                    State.Add(name);
                    State.Add(CreateBoundeBy());
                    duality.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), state.Duality));
                    State.Add(duality);
                    foreach (var connect in connects)
                    {
                        State.Add(connect);
                    }
                    State.Add(geometry);

                    stateMembers.Add(stateMember);
                }
            }

            return stateMembers;
        }

        private XElement CreateGeometry(StateMember state)
        {
            var geometry = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.geometry.ToString());
            var Point = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Point.ToString());
            var pos = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.pos.ToString());

            Point.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), state.PointGeo.Id));
            geometry.Add(Point);
            pos.Add(new XAttribute(ObjName.srsDimension.ToString(), "3"));
            pos.Add($"{state.PointGeo.Position.x} {state.PointGeo.Position.y} {state.PointGeo.Position.z}");
            Point.Add(pos);

            return geometry;
        }

        private List<XElement> CreateConnects(List<string> Connects)
        {
            var connects = new List<XElement>();
            foreach (var item in Connects)
            {
                var connect = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.connects.ToString());
                connect.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), item));
                connects.Add(connect);
            }
            return connects;
        }
    }
}
