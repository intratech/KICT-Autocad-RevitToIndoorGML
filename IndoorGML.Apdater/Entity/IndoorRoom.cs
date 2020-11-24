using IndoorGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TransitionMember = IndoorGML.Core.TransitionMember;

namespace IndoorGML.Adapter.Entity
{
    public class IndoorRoom : IndoorEntity
    {        
    

        internal TransitionMember ToTransition(IndoorConnectionSpace door)
        {
            TransitionMember transition = new TransitionMember();
            //transition.Transition.Duality = new Duality() { Href = $"#{door.Id}" };
            transition.Transition.Id = door.TransationId; 
            transition.Transition.Connects = new List<Connects>();
            transition.Transition.Connects.Add(new Connects() { Href = $"#{StateID}" });
            transition.Transition.Connects.Add(new Connects() { Href = $"#{door.StateID}" });
            //transition.Transition.Connects.Add(new Connects() { Href = $"#{door.Id}" });
            transition.Transition.Geometry = GetTransationGeoToDoor(door);
            return transition;
        }

        private Geometry GetTransationGeoToDoor(IndoorConnectionSpace door)
        {
            Geometry geo = new Geometry();
            geo.LineString = new LineString();
            geo.LineString.Id = $"TG-{door.TransationId}";
            geo.LineString.Pos = new List<Pos>();
            geo.LineString.Pos.Add(GetCenterPos());
            geo.LineString.Pos.Add(door.GetCenterPos());
            return geo;
        }
    }
}
