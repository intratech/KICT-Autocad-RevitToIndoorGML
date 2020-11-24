using CityGML.Core;
using CityGML.Core.DTO.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PreProcessor.InDoorGML.Models
{
    [Serializable]
    public class InDoorGMLModel
    {
        public string model_name { get; set; }
        public JBoundingBox bounding_box { get; set; }
        public double scale { get; set; }
        public List<JLevel> levels { get; set; }
        public List<JRoomInDoorGML> rooms { get; set; }
        public List<JDoorInDoorGML> doors { get; set; }
        public List<JWindowInDoorGML> windows { get; set; }

        public InDoorGMLModel(JModel model)
        {
            this.model_name = model.model_name;
            this.bounding_box = model.bounding_box;
            
            this.levels = model.levels;
            
            if(model.rooms != null)
                this.rooms = model.rooms.Select(x => new JRoomInDoorGML(x)).ToList();
            
            if(model.doors!=null)
                this.doors = model.doors.Select(x => new JDoorInDoorGML(x)).ToList() ;

            if(model.windows !=null)
                this.windows = model.windows.Select(x=>new JWindowInDoorGML(x)).ToList();
        }

        public List<JLineSegmentInDoorGML> GetBoundaryClosureSurfaces()
        {
            if (rooms == null)
                return new List<JLineSegmentInDoorGML>();

            var util = new GMLUtil();
            var closureSurfaces = new List<JLineSegmentInDoorGML>();
            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                var css = room.GetClosureSurfaces();
                foreach (var cs in css)
                {
                    if (closureSurfaces.Where(k => k.id == cs.id).FirstOrDefault() != null)
                        continue;
                    closureSurfaces.Add(cs);
                }
            }
            return closureSurfaces;
        }
        public List<CeilingInDoorGML> GetCeilingAndFloorClosureSurfaces()
        {
            if (rooms == null)
                return new List<CeilingInDoorGML>();

            var util = new GMLUtil();
            var ceilingAndFloors = new List<CeilingInDoorGML>();
            foreach (var room in rooms)
            {
                if (((room.is_elevator == true) || (room.is_escalator == true) || (room.is_stair == true)))
                {
                    if (!util.IsHighestRoom(room, GetOriginRooms(), GetOriginLevels()))
                    {
                        ceilingAndFloors.Add(new CeilingInDoorGML(room.GetCeiling(), room));
                    }

                    if (!util.IsLowestRoom(room, GetOriginRooms(), GetOriginLevels()))
                    {
                        ceilingAndFloors.Add(new CeilingInDoorGML(room.GetFloor(), room));
                    }
                }
            }
            return ceilingAndFloors;
        }
        public List<JRoom> GetOriginRooms()
        {
            if (rooms != null)
            {
                var rms = new List<JRoom>();
                foreach (var r in rooms)
                {
                    rms.Add(r as JRoom);
                }
                return rms;
            }
            return new List<JRoom>();
        }
        public List<JLevel> GetOriginLevels()
        {
            if (levels != null)
            {
                var levels = new List<JLevel>();
                foreach (var l in this.levels)
                {
                    levels.Add(l as JLevel);
                }
                return levels;
            }
            return new List<JLevel>();
        }
    }
}
