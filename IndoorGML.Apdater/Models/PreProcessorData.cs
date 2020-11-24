using CityGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessor.InDoorGML.Models
{
    public class PreProcessorData
    {
        public List<CellSpaceMember> CellSpaceMembers { get; set; }
        public List<CellSpaceBoundaryMember> CellSpaceBoundaryMembers { get; set; }
        public List<StateMember> StateMembers { get; set; }
        public List<TransitionMember> TransitionMembers { get; set; }
    }
}
