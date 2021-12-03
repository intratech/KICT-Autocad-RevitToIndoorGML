﻿using CityGML.Core.DTO.Json;
using CityGMLExporter.Utils;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CityGMLExporter
{
    public static class CityGMLWriter
    {
        public static List<string> _door_window_id = new List<string>();
        public static void WriteCityGMLInteriorRoom(JRoom room, Dictionary<XName, XNamespace> xmlnamespace, XElement interiorRoom, Dictionary<string, DoorSurfaces> doorSurfacesList)
        {
            //write Room to CityGML
            var Room = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + ObjName.Room.ToString());
            Room.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), room.id.ToString()));

            //Add name of Room
            var RoomName = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
            RoomName.Add(room.name);
            Room.Add(RoomName);

            //Write lod4Solid to define walls, doors, windows of room
            List<string> nameSpace = new List<string> { ObjName.lod4Solid.ToString(), ObjName.Solid.ToString(), ObjName.exterior.ToString(), ObjName.CompositeSurface.ToString() };
            Writelod4Solid(room, xmlnamespace, Room, nameSpace, doorSurfacesList);

            interiorRoom.Add(Room);

            if (room.geometry != null && room.geometry.boundary != null)
            {
                var Jbox = room.geometry.bouding_box;
                Point3D moveVec = new Point3D(0, 0, Jbox.max.z - Jbox.min.z);

                var boundary = room.geometry.boundary;
                if (boundary != null)
                {
                    //Write wall surfaces
                    for (var i = 0; i < boundary.Count; i++)
                    {
                        Point3D startPoint = new Point3D((float)boundary[i].points[0].x, (float)boundary[i].points[0].y, (float)boundary[i].points[0].z);
                        Point3D endPoint = new Point3D((float)boundary[i].points[1].x, (float)boundary[i].points[1].y, (float)boundary[i].points[1].z);

                        //Create surfaces of walls from points
                        SurfaceMember surface = GeometryUtils.GetSurface(startPoint, endPoint, moveVec);

                        if (surface != null)
                        {
                            var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                            //string id = boundary[i].gmlID;
                            string id = boundary[i].id.ToString();
                            if (boundary[i].isClosureSurface == true)
                            {
                                //Write Closure surfaces
                                name.Add(ObjName.ClosureSurface.ToString() + " " + boundary[i].id.ToString());
                                WriteInteriorWallSurface(ObjName.ClosureSurface.ToString(), room, Room, xmlnamespace, doorSurfacesList, surface, name, id);
                            }
                            else
                            {
                                //Write interior wall surfaces
                                name.Add("Wall " + boundary[i].id.ToString());
                                WriteInteriorWallSurface(ObjName.InteriorWallSurface.ToString(), room, Room, xmlnamespace, doorSurfacesList, surface, name, id);
                            }
                        }
                    }

                    //Checking is elevator -> Floor and Ceiling are closure surfaces
                    string typeFloorWall = "";
                    string typeCeilingWall = "";

                    typeFloorWall = ObjName.FloorSurface.ToString();
                    typeCeilingWall = ObjName.CeilingSurface.ToString();

                    //Note: in CityGML.Core project had GMLUtil. So if have modify, please use GMLUtil class instead GeometryUtils class.
                    if (((room.is_elevator == true) || (room.is_escalator == true) || (room.is_stair == true)) && !GeometryUtils.IsHighestRoom(room))
                    {
                        typeCeilingWall = ObjName.ClosureSurface.ToString();
                    }
                    if (((room.is_elevator == true) || (room.is_escalator == true) || (room.is_stair == true)) && !GeometryUtils.IsLowestRoom(room))
                    {
                        typeFloorWall = ObjName.ClosureSurface.ToString();
                    }

                    //Write Floor surface
                    SurfaceMember floorSurface = room.GetFloor().surface;
                    SurfaceMember ceilingSurface = room.GetCeiling().surface;

                    if (floorSurface != null)
                    {
                        string id = ObjName.Floor.ToString() + room.id.ToString();
                        //string id = room.floorGMLID;
                        var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                        name.Add(ObjName.Floor.ToString() + " " + room.id.ToString());
                        WriteInteriorWallSurface(typeFloorWall, room, Room, xmlnamespace, doorSurfacesList, floorSurface, name, id);
                    }

                    //Write Ceiling surface
                    if (ceilingSurface != null)
                    {
                        string id = ObjName.Ceiling.ToString() + room.id.ToString();
                        //string id = room.ceilingGMLID;
                        var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                        name.Add(ObjName.Ceiling.ToString() + " " + room.id.ToString());
                        WriteInteriorWallSurface(typeCeilingWall, room, Room, xmlnamespace, doorSurfacesList, ceilingSurface, name, id);
                    }
                }
            }
        }

        //***********************************
        //Function content: Start write each surface of room
        //Developer: Donny
        //Last modifier:
        //Modification content:
        //Updated on: 9-Sep-19
        //***********************************
        public static void WriteInteriorWallSurface(string typeWall, JRoom room, XElement Room, Dictionary<XName, XNamespace> xmlnamespace, Dictionary<string, DoorSurfaces> doorSurfacesList, SurfaceMember Surface, XElement name, string id)
        {
            if (Surface != null)
            {
                var boundedBy = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + ObjName.boundedBy.ToString());
                Room.Add(boundedBy);

                //InteriorWallSurface or ClosureSurface
                var TypeWallSurface = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + typeWall);
                boundedBy.Add(TypeWallSurface);

                TypeWallSurface.Add(name);

                //Write lod4MultiSurface
                List<string> lod4MultiSurfaceNameSpace = new List<string> { ObjName.lod4MultiSurface.ToString(), ObjName.MultiSurface.ToString(), ObjName.surfaceMember.ToString(), ObjName.CompositeSurface.ToString() };
                Writelod4MultiSurface(id, typeWall, xmlnamespace, TypeWallSurface, Surface, lod4MultiSurfaceNameSpace, doorSurfacesList);


                //Write opening Door/Window
                foreach (var doorSurface in doorSurfacesList)
                {
                    if (doorSurface.Value.idWall == id)
                    {
                        if (doorSurface.Value.item.door.geometry.positions[0].segment_id.ToString() == id)
                        {
                            List<string> nameSpaceOpenning = new List<string> { ObjName.opening.ToString(), null, ObjName.name.ToString(), ObjName.lod4MultiSurface.ToString(),
                            ObjName.MultiSurface.ToString()};
                            nameSpaceOpenning[1] = doorSurface.Value.item.door.type;//Write door or window
                            WriteOpenningPolygon(doorSurface, xmlnamespace, TypeWallSurface, nameSpaceOpenning);
                        }
                        else
                        {
                            List<string> nameSpaceOpenning = new List<string> { ObjName.opening.ToString(), null, ObjName.name.ToString(), ObjName.lod4MultiSurface.ToString(),
                            ObjName.MultiSurface.ToString(), ObjName.surfaceMember.ToString(), ObjName.OrientableSurface.ToString(), ObjName.baseSurface.ToString() };
                            nameSpaceOpenning[1] = doorSurface.Value.item.door.type;//Write door or window
                            WriteOpenningRef(doorSurface, xmlnamespace, TypeWallSurface, nameSpaceOpenning);
                        }
                    }
                }
            }
        }

        public static void Writelod4MultiSurface(string id, string typeWall, Dictionary<XName, XNamespace> xmlnamespace, XElement Root, SurfaceMember surface, List<string> nameSpace, Dictionary<string, DoorSurfaces> doorSurfacesList)
        {
            var parent = Root;
            for (var i = 0; i < nameSpace.Count; i++)
            {
                var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + nameSpace[i]);
                if (nameSpace[i] == ObjName.CompositeSurface.ToString())
                {
                    child.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), id));
                }
                parent.Add(child);
                parent = child;
            }

            string idSurfaceMember = id + ObjName.interior.ToString() + ObjName.Door.ToString();
            if (typeWall == ObjName.ClosureSurface.ToString())
            {
                //Write Closure surface is unappearance
                List<SurfaceMember> newClosureSurface = new List<SurfaceMember>();
                newClosureSurface.Add(surface);
                WriteSurfaceMembers(idSurfaceMember, xmlnamespace, parent, null, newClosureSurface);
            }
            else
            {
                //Write SurfaceMembers attached interior surfaces
                List<SurfaceMember> interiorSurfaceList = new List<SurfaceMember>();
                foreach (var doorSurface in doorSurfacesList)
                {
                    if (doorSurface.Value.idWall == id)
                    {
                        interiorSurfaceList.Add(doorSurface.Value.doorSurfaceList[0]);
                    }
                }
                WriteSurfaceMembers(idSurfaceMember, xmlnamespace, parent, surface, interiorSurfaceList);
            }

            //Create openning name
            List<string> nameSpaceOpenning = new List<string> { ObjName.opening.ToString(), null, ObjName.name.ToString(), ObjName.lod4MultiSurface.ToString(),
                    ObjName.MultiSurface.ToString(), ObjName.surfaceMember.ToString(), ObjName.OrientableSurface.ToString(), ObjName.baseSurface.ToString() };

            //Write SurfaceMembers from door list
            foreach (var doorSurface in doorSurfacesList)
            {
                if (doorSurface.Value.idWall == id)
                {
                    for (var i = 0; i < doorSurface.Value.doorSurfaceList.Count; i++)
                    {
                        string doorMemberId = ObjName.Door.ToString() + doorSurface.Key + ObjName.Room.ToString() + doorSurface.Value.item.room.id + ObjName.id.ToString() + i.ToString();
                        if (i != 0)//ignore interior surface of door 
                            WriteSurfaceMembers(doorMemberId, xmlnamespace, parent, doorSurface.Value.doorSurfaceList[i], null);
                    }

                    ////Write openning with door
                    //nameSpaceOpenning[1] = doorSurface.Value.item.door.type;//Write door or window
                    //WriteOpenning(doorSurface, xmlnamespace, Root, nameSpaceOpenning);
                }
            }
        }

        public static void WriteSurfaceMembers(string id, Dictionary<XName, XNamespace> xmlnamespace, XElement Root, SurfaceMember surface, List<SurfaceMember> interSurfaceList)
        {
            var surfaceMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.surfaceMember.ToString());
            Root.Add(surfaceMember);
            {
                var Polygon = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Polygon.ToString());
                Polygon.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), id));
                surfaceMember.Add(Polygon);
                {
                    List<string> nameSpaces = new List<string> { ObjName.exterior.ToString(), ObjName.LinearRing.ToString(), ObjName.posList.ToString() };

                    //Write exterior -> appearance surface
                    if (surface != null)
                    {
                        WriteSurface(xmlnamespace, Polygon, surface, nameSpaces);
                    }

                    //Write interior -> unappearance surface
                    if (interSurfaceList != null && interSurfaceList.Count != 0)
                    {
                        nameSpaces[0] = ObjName.interior.ToString();
                        for (var i = 0; i < interSurfaceList.Count; i++)
                        {
                            WriteSurface(xmlnamespace, Polygon, interSurfaceList[i], nameSpaces);
                        }
                    }
                }
            }
        }

        public static void WriteSurface(Dictionary<XName, XNamespace> xmlnamespace, XElement Root, SurfaceMember surface, List<string> nameSpace)
        {
            var parent = Root;
            for (var i = 0; i < nameSpace.Count; i++)
            {
                var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                if (nameSpace[i] == ObjName.posList.ToString())
                {
                    string points = "";
                    for (var j = 0; j < surface.points.Count; j++)
                    {
                        points += $" {surface.points[j].x.ToString()} {surface.points[j].y.ToString()} {surface.points[j].z.ToString()}";
                    }
                    child.Add(points);
                }
                parent.Add(child);
                parent = child;
            }
        }

        public static void WriteOpenningRef(KeyValuePair<string, DoorSurfaces> doorSurface, Dictionary<XName, XNamespace> xmlnamespace, XElement Root, List<string> nameSpace)
        {
            var parent = Root;
            for (var i = 0; i < nameSpace.Count; i++)
            {
                //Write opening/Door/lod4MultiSurface
                if (nameSpace[i] == ObjName.opening.ToString() || nameSpace[i] == ObjName.Door.ToString() || nameSpace[i] == ObjName.Window.ToString() || nameSpace[i] == ObjName.lod4MultiSurface.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + nameSpace[i]);
                    parent.Add(child);
                    parent = child;
                }

                //Write name of door/window
                if (nameSpace[i] == ObjName.name.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    string name = doorSurface.Value.item.door.type + "_" + doorSurface.Key;
                    child.Add(name);
                    parent.Add(child);
                }

                //Write MultiSurface and surfaceMember
                if (nameSpace[i] == ObjName.MultiSurface.ToString() || nameSpace[i] == ObjName.surfaceMember.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    parent.Add(child);
                    parent = child;
                }

                //Write OrientableSurface
                if (nameSpace[i] == ObjName.OrientableSurface.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    child.Add(new XAttribute(ObjName.orientation.ToString(), "-"));
                    parent.Add(child);
                    parent = child;
                }

                //Write baseSurface
                if (nameSpace[i] == ObjName.baseSurface.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    child.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), "#"+doorSurface.Key));
                    parent.Add(child);
                    parent = child;
                }
            }
        }

        public static XElement WriteMultiSurface(Dictionary<XName, XNamespace> xmlnamespace, List<string> nameSpace, XElement Building)
        {
            XElement currentNode = Building;
            for (var i = 0; i < nameSpace.Count; i++)
            {
                //Write boundedBy/WallSurface
                if (nameSpace[i] == ObjName.MultiSurface.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    currentNode.Add(child);
                    currentNode = child;
                }
                else
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + nameSpace[i]);
                    currentNode.Add(child);
                    currentNode = child;
                }
            }

            return currentNode;
        }

        //Write Doors-Windows exterior room for reference
        public static void WriteOpenningPolygon(KeyValuePair<string, DoorSurfaces> doorSurface, Dictionary<XName, XNamespace> xmlnamespace, XElement parent, List<string> nameSpace)
        {
            SurfaceMember surface = doorSurface.Value.item.door.GetSurfaceGeo();
            if(surface != null)
            {
                XElement currentNode = parent;
                for (var i = 0; i < nameSpace.Count; i++)
                {
                    //Write name of door/window
                    if (nameSpace[i] == ObjName.name.ToString())
                    {
                        var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                        string name = doorSurface.Value.item.door.type + "_" + doorSurface.Key;
                        child.Add(name);
                        currentNode.Add(child);
                    }

                    //Write MultiSurface
                    else if (nameSpace[i] == ObjName.MultiSurface.ToString())
                    {
                        var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                        currentNode.Add(child);
                        currentNode = child;
                    }
                    else
                    {
                        var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + nameSpace[i]);
                        currentNode.Add(child);
                        currentNode = child;
                    }
                }
                WriteSurfaceMembers(doorSurface.Value.item.door.id.ToString(), xmlnamespace, currentNode, surface, null);
            }
        }


        //Write lod4Solid to define walls,doors,windows
        public static void Writelod4Solid(JRoom room, Dictionary<XName, XNamespace> xmlnamespace, XElement RoomElement, List<string> nameSpace, Dictionary<string, DoorSurfaces> doorSurfacesList)
        {
            //Get Wall's IDs exclude closure surface
            List<string> wallListID = room.GetWallIDListExcludeCS();
            if(!(((room.is_elevator == true) || (room.is_escalator == true) || (room.is_stair == true)) && !GeometryUtils.IsHighestRoom(room)))
                wallListID.Add(room.GetCeiling().id);
            if (!(((room.is_elevator == true) || (room.is_escalator == true) || (room.is_stair == true)) && !GeometryUtils.IsLowestRoom(room)))
                wallListID.Add(room.GetFloor().id);

            //Get door list id
            List<string> doorListID = new List<string>();
            foreach (var doorWall in doorSurfacesList)
            {
                doorListID.Add(doorWall.Key);
            }

            if (wallListID == null && doorListID.Count == 0)
                return;

            var parent = RoomElement;
            for (var i = 0; i < nameSpace.Count; i++)
            {
                //Write lod4Solid
                if (nameSpace[i] == ObjName.lod4Solid.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + nameSpace[i]);
                    parent.Add(child);
                    parent = child;
                }

                if (nameSpace[i] == ObjName.Solid.ToString()|| nameSpace[i] == ObjName.exterior.ToString()||
                    nameSpace[i] == ObjName.CompositeSurface.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    parent.Add(child);
                    parent = child;
                }
            }

            //Write wall's IDs of Room
            List<string> nameSpaceWallLinkID = new List<string> { ObjName.surfaceMember.ToString(), ObjName.OrientableSurface.ToString(), ObjName.baseSurface.ToString() };
            if(wallListID != null)
            {
                foreach (var wallID in wallListID)
                {
                    string orientation = "-";
                    WriteIDLinkSurfaceMember(xmlnamespace, parent, nameSpaceWallLinkID, wallID, orientation);
                }
            }
            //Write door's IDs of Room
            if (doorListID.Count!=0)
            {
                foreach (var doorID in doorListID)
                {
                    string orientation = "+";
                    WriteIDLinkSurfaceMember(xmlnamespace, parent, nameSpaceWallLinkID, doorID, orientation);
                }
            }
        }

        public static void WriteIDLinkSurfaceMember(Dictionary<XName, XNamespace> xmlnamespace, XElement RoomElement, List<string> nameSpace, string wallID, string orientation)
        {
            var parent = RoomElement;
            for (var i = 0; i < nameSpace.Count; i++)
            {
                //Write lod4Solid
                if (nameSpace[i] == ObjName.surfaceMember.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    parent.Add(child);
                    parent = child;
                }

                //Write OrientableSurface
                if (nameSpace[i] == ObjName.OrientableSurface.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    child.Add(new XAttribute(ObjName.orientation.ToString(), orientation));
                    parent.Add(child);
                    parent = child;
                }

                //Write baseSurface
                if (nameSpace[i] == ObjName.baseSurface.ToString())
                {
                    var child = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + nameSpace[i]);
                    child.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), "#" + wallID));
                    parent.Add(child);
                    parent = child;
                }
            }
        }
    }
}