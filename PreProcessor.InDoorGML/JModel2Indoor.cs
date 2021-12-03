﻿using GML.Core;
using GML.Core.DTO.Json;
using IndoorGML.Adapter.Entity;
using IndoorGML.Core;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndoorGML.Adapter
{
    public class JModel2Indoor
    {
        public IndoorFeatures Convert(JModel model)
        {
            IndoorModel indoorModel = new IndoorModel(model.model_name);
            indoorModel.Name = model.model_name;
            indoorModel.Description = model.description;

            model.rooms.ForEach(x =>
            {
                //if (x.id == -227024213649462447)
                {
                    if (x.geometry != null && x.geometry.boundary.Count > 0)
                    {
                        if(string.IsNullOrEmpty(x.level) && model.levels != null )
                        {
                            x.level = model.levels.Where(level => level.id == x.level_id).FirstOrDefault()?.name;
                        }
                        x.JID = indoorModel.AddRoom(ConvertRoom(x));
                    }
                }
            });

          model.doors.ForEach(x =>
            {
                if (x.geometry != null && x.geometry.positions.Count >= 1)
                {
                    if (string.IsNullOrEmpty(x.level) && model.levels != null)
                    {
                        x.level = model.levels.Where(level => level.id == x.level_id).FirstOrDefault()?.name;
                    }
                    x.JID = indoorModel.AddDoor(ConvertDoor(x));
                }
            });

            //generate connection
            model.rooms.ForEach(x =>
            {
                if (x.JID != -1)
                {
                    foreach (var door in model.doors.Where(d => d.JID != -1 && x.HasConnect(d)))
                    {
                        indoorModel.AddConnection(x.JID, door.JID, ConnectionType.RoomToDoor);
                    }
                }
            });

            CreateVerticalConnection(model.rooms, indoorModel,Config.ToleranceSameArea);
            var component = model.rooms.Where(x => ( x.is_stair == true ) && x.JID != -1).OrderBy(x => x.geometry.bouding_box.min.z).ToList();

            for(int i=0;i<component.Count-1;i++)
            {
                var stair = component[i];
                for(int j=i+1;j<component.Count;j++)
                {
                    var nextStair = component[j];
                    if(stair.SameXY(nextStair,Config.ToleranceSameArea) && stair.IsNextElevation(nextStair))
                    {
                        indoorModel.AddConnection(stair.JID, nextStair.JID, ConnectionType.RoomToRoom);
                        break;
                    }
                }
            }

            component = model.rooms.Where(x => (x.is_elevator == true) && x.JID != -1).OrderBy(x => x.geometry.bouding_box.min.z).ToList();

            for (int i = 0; i < component.Count - 1; i++)
            {
                var stair = component[i];
                for (int j = i + 1; j < component.Count; j++)
                {
                    var nextStair = component[j];
                    if (stair.SameXY(nextStair, Config.ToleranceEqual) && stair.IsNextElevation(nextStair))
                    {
                        indoorModel.AddConnection(stair.JID, nextStair.JID, ConnectionType.RoomToRoom);
                        break;
                    }
                }
            }

            try
            {
                IndoorFeatures features = indoorModel.Convert(1);
                return features;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw ex;
            }
        }

        private void CreateVerticalConnection(List<JRoom> rooms, IndoorModel indoorModel,double tolerance)
        {
            List<string> functions = new List<string>();
            foreach(var room in rooms)
            {
                if(room.type != null && room.type == "Vertical")
                {
                    if(!functions.Contains(room.function))
                    {
                        functions.Add(room.function);
                    }
                }
            }

            foreach(var function in functions)
            {
               var spacecs = rooms.Where(x => x.type == "Vertical" && x.function == function).OrderBy(x => x.geometry.bouding_box.min.z).ToList();
                for (int i = 0; i < spacecs.Count - 1; i++)
                {
                    var stair = spacecs[i];
                    for (int j = i + 1; j < spacecs.Count; j++)
                    {
                        var nextStair = spacecs[j];
                        if (stair.SameXY(nextStair, tolerance) && stair.IsNextElevation(nextStair))
                        {
                            indoorModel.AddConnection(stair.JID, nextStair.JID, ConnectionType.RoomToRoom);
                            break;
                        }
                    }
                }
            }

        }

        public IndoorRoom ConvertRoom(JRoom x)
        {
            IndoorRoom room = new IndoorRoom();
            //room.Id = x.id;
            //room.Id = x.id.ToString();

            room.Name = x.id.ToString();
            room.DisplayName = x.name;            
            room.Type = x.type;
            room.Function = x.function;
            room.Code = x.code;
            room.Usage = "";// x.usage;
            room.Height = x.RoomHeight;
            room.Boundary = ConvertBoundary(x.geometry.boundary);
            room.Level = x.level;
            return room;
        }

        public InDoorPolyline ConvertBoundary(List<JLineSegment> boundary)
        {
            InDoorPolyline polyline = new InDoorPolyline();
            boundary.ForEach(
                x =>
                {
                    polyline.AddRange(x.points);

                });
            polyline.Points = polyline.Points.Distinct(new Vector3Comparer(0.01f)).ToList();
            return polyline;
        }

        public IndoorConnectionSpace ConvertDoor(JDoor x)
        {
            IndoorConnectionSpace connectionSpace = new IndoorConnectionSpace();
            connectionSpace.Height = x.DoorHeight;
            connectionSpace.Boundary = ConvertDoorLine(x);
            connectionSpace.DisplayName = x.name;
            connectionSpace.Type = x.type;
            connectionSpace.Name = x.id.ToString();
            connectionSpace.Usage = "";// x.usage;
            connectionSpace.Function = x.function;
            connectionSpace.Level = x.level;
            return connectionSpace;
        }

        public InDoorPolyline ConvertDoorLine(JDoor door)
        {
            InDoorPolyline polyline = new InDoorPolyline();
            foreach(var p in door.GetBoundaryPolyline())
            {
                polyline.Add(new System.Numerics.Vector3((float)p.x, (float)p.y, (float)p.z));
            }

            return polyline;
        }
        

    }
}