using CityGML.Core.DTO.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessor.InDoorGML.Models
{
    public class JLineSegmentInDoorGML : JLineSegment
    {
        public JLineSegmentInDoorGML(JLineSegment segment) : base(segment)
        {

        }
        public List<JRoomInDoorGML> GetRooms(List<JRoomInDoorGML> rooms)
        {
            return new InDoorUtil().GetRooms(rooms, this);
        }
    }
}
