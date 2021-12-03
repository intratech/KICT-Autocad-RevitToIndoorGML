using CityGML.Core.DTO.Json;
using CityGMLExporter.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace CityGMLExporter
{
    public class Exporter
    {
        //<add key = "width" value="1300"/>
        //<add key = "height" value="3000"/>
        //<add key = "thickness" value="150"/>
        public void ExportCityGML(JModel model, string saveCityGMLFilePath,int width=1300,int height=3000,int thickness= 150)
        {
            try
            {
                JModel JData = model;
                if (JData != null)
                {
                    Config.Scale = JData.scale;
                    CityGML.Core.Config.Scale = JData.scale;

                    CityGML.Core.Config.DefaultWidth = width / Config.Scale;
                    CityGML.Core.Config.DefaultHeight = height / Config.Scale;
                    CityGML.Core.Config.DefaultThickness = thickness / Config.Scale;

                    Config.Rooms = JData.rooms.ToArray();
                    Config.Levels = JData.levels.ToArray();
                }

                //Declare name space for CityGML
                XNamespace xmlns = "http://www.opengis.net/citygml/2.0";
                var xmlnamespace = new Dictionary<XName, XNamespace>
            {
                { XNamespace.Xmlns + ObjName.xsi.ToString(), "http://www.w3.org/2001/XMLSchema-instance" },
                { XNamespace.Xmlns + ObjName.xAL.ToString(), "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0"},
                { XNamespace.Xmlns + ObjName.xlink.ToString(), "http://www.w3.org/1999/xlink" },
                { XNamespace.Xmlns + ObjName.gml.ToString(), "http://www.opengis.net/gml" },
                { XNamespace.Xmlns + ObjName.dem.ToString(), "http://www.opengis.net/citygml/relief/2.0" },
                { XNamespace.Xmlns + ObjName.bldg.ToString(), "http://www.opengis.net/citygml/building/2.0" },
            };

                //Declare Root of CityGML
                var Root = new XElement(xmlns + ObjName.CityModel.ToString());

                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                Root.Add(new XAttribute(xsi + ObjName.schemaLocation.ToString(), "http://www.opengis.net/citygml/building/2.0 http://schemas.opengis.net/citygml/building/2.0/building.xsd http://www.opengis.net/citygml/relief/2.0 http://schemas.opengis.net/citygml/relief/2.0/relief.xsd"));

                //Start writing name space attribute
                foreach (var ns in xmlnamespace)
                {
                    Root.Add(new XAttribute(ns.Key, ns.Value));
                }

                //Write name of project
                var projectName = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                projectName.Add(JData.model_name);
                Root.Add(projectName);

                //Write reference coordinate System
                var coordinateSystem = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.boundedBy.ToString());
                Root.Add(coordinateSystem);
                {
                    var Envelope = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Envelope.ToString());
                    coordinateSystem.Add(Envelope);
                    {
                        var lowerCorner = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.lowerCorner.ToString());
                        Envelope.Add(lowerCorner);
                        {
                            string min = $"{ JData.bounding_box.min.x.ToString() } { JData.bounding_box.min.y.ToString() } { JData.bounding_box.min.z.ToString() }";
                            lowerCorner.Add(min);//Write coordinate box
                        }
                        var upperCorner = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.upperCorner.ToString());
                        Envelope.Add(upperCorner);
                        {
                            string max = $"{ JData.bounding_box.max.x.ToString() } { JData.bounding_box.max.y.ToString() } { JData.bounding_box.max.z.ToString() }";
                            upperCorner.Add(max);//Write coordinate box
                        }
                        Envelope.Add(new XAttribute(ObjName.srsDimension.ToString(), "3"));
                        //Envelope.Add(new XAttribute(ObjName.srsName.ToString(), "urn:ogc:def:crs,crs:EPSG::25832,crs:EPSG::5783"));
                        Envelope.Add(new XAttribute(ObjName.srsName.ToString(), "Unknown"));
                    }
                }

                //Write cityObjectMember
                var cityObjectMember = new XElement(xmlns + ObjName.cityObjectMember.ToString());
                Root.Add(cityObjectMember);
                {
                    var Building = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + ObjName.Building.ToString());
                    Building.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), "GML_7b1a5a6f-ddad-4c3d-a507-3eb9ee0a8e68"));
                    cityObjectMember.Add(Building);

                    List<JRoom> rooms = JData.rooms;
                    List<JDoor> doors = JData.doors;
                    List<JWindow> windows = JData.windows;

                    ////Write door-window for reference
                    //List<string> nameSpaceForDoor = new List<string> { ObjName.boundedBy.ToString(), ObjName.WallSurface.ToString() , ObjName.opening.ToString(), ObjName.Door.ToString(), ObjName.lod4MultiSurface.ToString(),
                    //ObjName.MultiSurface.ToString()};
                    //List<string> nameSpaceForWindow = new List<string> { ObjName.boundedBy.ToString(), ObjName.WallSurface.ToString() , ObjName.opening.ToString(), ObjName.Window.ToString(), ObjName.lod4MultiSurface.ToString(),
                    //ObjName.MultiSurface.ToString()};
                    //var parentDoor = CityGMLWriter.WriteMultiSurface(xmlnamespace, nameSpaceForDoor, Building);
                    //foreach (var door in doors)
                    //{
                    //    //if (door.id == 353736)
                    //    //{
                    //    //    var a = door.GetSurfaceGeo();
                    //    //}
                    //    CityGMLWriter.WriteOpenningPolygon(door, xmlnamespace, parentDoor);
                    //}
                    //var parentWindow = CityGMLWriter.WriteMultiSurface(xmlnamespace, nameSpaceForWindow, Building);
                    //foreach (var window in windows)
                    //{
                    //    CityGMLWriter.WriteOpenningPolygon(window, xmlnamespace, parentWindow);
                    //}

                    //Loop each room
                    for (var item = 0; item < rooms.Count; item++)
                    {
                        if (rooms[item].geometry.boundary.Count == 0)
                            continue;
                        var interiorRoom = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + ObjName.interiorRoom.ToString());
                        Building.Add(interiorRoom);

                        //find Door point to attach to Room
                        Dictionary<string, Door> doorsInRoom = GeometryUtils.FindDoorsWindowsInRoom(rooms[item], doors, windows);

                        //Create doorSurfaces by door points
                        Dictionary<string, DoorSurfaces> doorSurfacesList = new Dictionary<string, DoorSurfaces>();
                        doorSurfacesList = GeometryUtils.GetDoorSurfacesInRoom(doorsInRoom);

                        //Write interior Room
                        CityGMLWriter.WriteCityGMLInteriorRoom(rooms[item], xmlnamespace, interiorRoom, doorSurfacesList);
                    }
                }

                Root.Save(saveCityGMLFilePath);
            }
            catch (Exception err)
            {
                throw err;
            }

        }

        public void exportCityGMLFromJson(string jsonFilePath, string saveCityGMLFilePath)
        {
            try
            {
                string jsonData = File.ReadAllText(jsonFilePath);

                JModel JData = new JModel();
                if (!string.IsNullOrEmpty(jsonData))
                {
                    JData = JsonConvert.DeserializeObject<JModel>(jsonData);
                    Config.Scale = JData.scale;
                    CityGML.Core.Config.Scale = JData.scale;

                    //get width
                    int value;
                    if (Int32.TryParse(ConfigurationManager.AppSettings["width"],out value))
                    {
                        CityGML.Core.Config.DefaultWidth = value / Config.Scale;
                        
                    }
                    if (Int32.TryParse(ConfigurationManager.AppSettings["height"], out value))
                    {
                        CityGML.Core.Config.DefaultHeight = value/Config.Scale;
                    }
                    if (Int32.TryParse(ConfigurationManager.AppSettings["thickness"], out value))
                    {
                        CityGML.Core.Config.DefaultThickness = value / Config.Scale;
                    }

                    Config.Rooms = JData.rooms.ToArray();
                    Config.Levels = JData.levels.ToArray();
                }

                //Declare name space for CityGML
                XNamespace xmlns = "http://www.opengis.net/citygml/2.0";
                var xmlnamespace = new Dictionary<XName, XNamespace>
            {
                { XNamespace.Xmlns + ObjName.xsi.ToString(), "http://www.w3.org/2001/XMLSchema-instance" },
                { XNamespace.Xmlns + ObjName.xAL.ToString(), "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0"},
                { XNamespace.Xmlns + ObjName.xlink.ToString(), "http://www.w3.org/1999/xlink" },
                { XNamespace.Xmlns + ObjName.gml.ToString(), "http://www.opengis.net/gml" },
                { XNamespace.Xmlns + ObjName.dem.ToString(), "http://www.opengis.net/citygml/relief/2.0" },
                { XNamespace.Xmlns + ObjName.bldg.ToString(), "http://www.opengis.net/citygml/building/2.0" },
            };

                //Declare Root of CityGML
                var Root = new XElement(xmlns + ObjName.CityModel.ToString());

                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                Root.Add(new XAttribute(xsi + ObjName.schemaLocation.ToString(), "http://www.opengis.net/citygml/building/2.0 http://schemas.opengis.net/citygml/building/2.0/building.xsd http://www.opengis.net/citygml/relief/2.0 http://schemas.opengis.net/citygml/relief/2.0/relief.xsd"));

                //Start writing name space attribute
                foreach (var ns in xmlnamespace)
                {
                    Root.Add(new XAttribute(ns.Key, ns.Value));
                }

                //Write name of project
                var projectName = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                projectName.Add(JData.model_name);
                Root.Add(projectName);

                //Write reference coordinate System
                var coordinateSystem = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.boundedBy.ToString());
                Root.Add(coordinateSystem);
                {
                    var Envelope = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Envelope.ToString());
                    coordinateSystem.Add(Envelope);
                    {
                        var lowerCorner = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.lowerCorner.ToString());
                        Envelope.Add(lowerCorner);
                        {
                            string min = $"{ JData.bounding_box.min.x.ToString() } { JData.bounding_box.min.y.ToString() } { JData.bounding_box.min.z.ToString() }";
                            lowerCorner.Add(min);//Write coordinate box
                        }
                        var upperCorner = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.upperCorner.ToString());
                        Envelope.Add(upperCorner);
                        {
                            string max = $"{ JData.bounding_box.max.x.ToString() } { JData.bounding_box.max.y.ToString() } { JData.bounding_box.max.z.ToString() }";
                            upperCorner.Add(max);//Write coordinate box
                        }
                        Envelope.Add(new XAttribute(ObjName.srsDimension.ToString(), "3"));
                        //Envelope.Add(new XAttribute(ObjName.srsName.ToString(), "urn:ogc:def:crs,crs:EPSG::25832,crs:EPSG::5783"));
                        Envelope.Add(new XAttribute(ObjName.srsName.ToString(), "Unknown"));
                    }
                }

                //Write cityObjectMember
                var cityObjectMember = new XElement(xmlns + ObjName.cityObjectMember.ToString());
                Root.Add(cityObjectMember);
                {
                    var Building = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + ObjName.Building.ToString());
                    Building.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), "GML_7b1a5a6f-ddad-4c3d-a507-3eb9ee0a8e68"));
                    cityObjectMember.Add(Building);

                    List<JRoom> rooms = JData.rooms;
                    List<JDoor> doors = JData.doors;
                    List<JWindow> windows = JData.windows;

                    ////Write door-window for reference
                    //List<string> nameSpaceForDoor = new List<string> { ObjName.boundedBy.ToString(), ObjName.WallSurface.ToString() , ObjName.opening.ToString(), ObjName.Door.ToString(), ObjName.lod4MultiSurface.ToString(),
                    //ObjName.MultiSurface.ToString()};
                    //List<string> nameSpaceForWindow = new List<string> { ObjName.boundedBy.ToString(), ObjName.WallSurface.ToString() , ObjName.opening.ToString(), ObjName.Window.ToString(), ObjName.lod4MultiSurface.ToString(),
                    //ObjName.MultiSurface.ToString()};
                    //var parentDoor = CityGMLWriter.WriteMultiSurface(xmlnamespace, nameSpaceForDoor, Building);
                    //foreach (var door in doors)
                    //{
                    //    //if (door.id == 353736)
                    //    //{
                    //    //    var a = door.GetSurfaceGeo();
                    //    //}
                    //    CityGMLWriter.WriteOpenningPolygon(door, xmlnamespace, parentDoor);
                    //}
                    //var parentWindow = CityGMLWriter.WriteMultiSurface(xmlnamespace, nameSpaceForWindow, Building);
                    //foreach (var window in windows)
                    //{
                    //    CityGMLWriter.WriteOpenningPolygon(window, xmlnamespace, parentWindow);
                    //}

                    //Loop each room
                    for (var item = 0; item < rooms.Count; item++)
                    {
                        if (rooms[item].geometry.boundary.Count == 0)
                            continue;
                        var interiorRoom = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.bldg.ToString()] + ObjName.interiorRoom.ToString());
                        Building.Add(interiorRoom);

                        //find Door point to attach to Room
                        Dictionary<string, Door> doorsInRoom = GeometryUtils.FindDoorsWindowsInRoom(rooms[item], doors, windows);

                        //Create doorSurfaces by door points
                        Dictionary<string, DoorSurfaces> doorSurfacesList = new Dictionary<string, DoorSurfaces>();
                        doorSurfacesList = GeometryUtils.GetDoorSurfacesInRoom(doorsInRoom);

                        //Write interior Room
                        CityGMLWriter.WriteCityGMLInteriorRoom(rooms[item], xmlnamespace, interiorRoom, doorSurfacesList);
                    }
                }

                Root.Save(saveCityGMLFilePath);
            }
            catch (Exception err)
            {
                throw err;
            }
            
        }
    }
}

