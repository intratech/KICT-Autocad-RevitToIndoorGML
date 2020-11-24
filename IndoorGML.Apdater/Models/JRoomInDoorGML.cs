using CityGML.Core;
using CityGML.Core.DTO.Json;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PreProcessor.InDoorGML.Models
{
    [Serializable]
    public class JRoomInDoorGML : JRoom
    {
        public JRoomInDoorGML(JRoom p)
        {
            foreach (FieldInfo prop in p.GetType().GetFields())
                GetType().GetField(prop.Name).SetValue(this, prop.GetValue(p));

            foreach (PropertyInfo prop in p.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(p, null), null);
        }

        /// <summary>
        /// This method returns all surfaces of the room.
        /// </summary>
        /// <summary>
        /// Gets doors belong this room.
        /// </summary>
        /// <param name="doors"></param>
        /// <returns></returns>
        public List<JDoorInDoorGML> GetDoors(List<JDoorInDoorGML> doors)
        {
            if (doors != null)
            {
                //Create list of door inside Room
                var doorList = new List<JDoorInDoorGML>();

                //Check id of room is exist in door and get door's list
                var segmentsRoom = geometry.boundary;

                //Loop in each segment of room
                for (var i = 0; i < segmentsRoom.Count; i++)
                {
                    //Loop all doors
                    foreach (var door in doors)
                    {
                        //Loop in door position of each segment
                        foreach (var doorPos in door.geometry.positions)
                        {
                            if (doorPos.segment_id == segmentsRoom[i].id)
                            {
                                doorList.Add(door);
                                break;
                            }
                        }
                    }
                }
                return doorList;
            }
            return null;
        }
        /// <summary>
        /// Gets windows belong this room.
        /// </summary>
        /// <param name="windows"></param>
        /// <returns></returns>
        public List<JWindowInDoorGML> GetWindows(List<JWindowInDoorGML> windows)
        {
            if (windows != null)
            {
                //Create list of door inside Room
                var windowList = new List<JWindowInDoorGML>();

                //Check id of room is exist in door and get door's list
                var segmentsRoom = geometry.boundary;

                //Loop in each segment of room
                for (var i = 0; i < segmentsRoom.Count; i++)
                {
                    //Loop all windows
                    foreach (var window in windows)
                    {
                        //Loop in door position of each segment
                        foreach (var doorPos in window.geometry.positions)
                        {
                            if (doorPos.segment_id == segmentsRoom[i].id)
                            {
                                windowList.Add(window);
                            }
                        }
                    }
                }
                return windowList;
            }
            return null;
        }

        public List<JLineSegmentInDoorGML> GetClosureSurfaces()
        {
            if (this.geometry == null)
                return new List<JLineSegmentInDoorGML>();
            var boundaries = geometry.boundary;
            if (boundaries == null)
                return new List<JLineSegmentInDoorGML>();
            var closureSurfaceSegments = new List<JLineSegmentInDoorGML>();
            for (int i = 0; i < boundaries.Count; i++)
            {
                if (boundaries[i].isClosureSurface == true)
                {
                    JLineSegmentInDoorGML cs = new JLineSegmentInDoorGML(boundaries[i]);
                    closureSurfaceSegments.Add(cs);
                }
            }
            return closureSurfaceSegments;
        }
    }
}
