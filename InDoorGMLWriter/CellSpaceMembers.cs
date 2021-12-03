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
        private List<XElement> CreateCellSpaceMembers(List<CellSpaceMember> cellSpaces)
        {
            var cellSpaceMembers = new List<XElement>();

            if(cellSpaces != null)
            {
                foreach (var cell in cellSpaces)
                {
                    if (cell.Id == "C454365")
                        Console.WriteLine();
                    if (cell.CellSpaceGeometry == null || cell.CellSpaceGeometry.Surfaces == null || cell.CellSpaceGeometry.Surfaces.Count == 0)
                        continue;

                    var cellSpaceMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.cellSpaceMember.ToString());
                    var cellSpace = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.CellSpace.ToString());
                    var description = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.description.ToString());
                    var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                    var cellSpaceGeometry = CreateCellSpaceGeometry(cell);
                    var duality = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.duality.ToString());
                    var partialboundedBy = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.partialboundedBy.ToString());
                    var Class = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.navi.ToString()] + "class");
                    var function = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.navi.ToString()] + ObjName.function.ToString());
                    var usage = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.navi.ToString()] + ObjName.usage.ToString());

                    cellSpaceMember.Add(cellSpace);
                    cellSpace.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), cell.Id));
                    description.Add(cell.Description);
                    cellSpace.Add(description);
                    name.Add(cell.Name);
                    cellSpace.Add(name);
                    cellSpace.Add(CreateBoundeBy());
                    cellSpace.Add(cellSpaceGeometry);
                    duality.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), cell.Duality));
                    cellSpace.Add(duality);
                    cellSpace.Add(partialboundedBy);
                    Class.Add("1020");
                    cellSpace.Add(Class);
                    function.Add("2550");
                    cellSpace.Add(function);
                    usage.Add("2550");
                    cellSpace.Add(usage);

                    cellSpaceMembers.Add(cellSpaceMember);
                }
            }

            return cellSpaceMembers;
        }
        //private List<XElement> CreateCellSpaceMembers(List<CellSpaceMember> cellSpaces)
        //{
        //    var cellSpaceMembers = new List<XElement>();
        //    foreach (var cell in cellSpaces)
        //    {
        //        var cellSpaceMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.cellSpaceMember.ToString());
        //        var CellSpace = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.CellSpace.ToString());
        //        var description = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.description.ToString());
        //        var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
        //        var cellSpaceGeometry = CreateCellSpaceGeometry(cell);
        //        var duality = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.duality.ToString());
        //        var partialboundedBy = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.partialboundedBy.ToString());
        //        var Class = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.navi.ToString()] + "class");
        //        var function = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.navi.ToString()] + ObjName.function.ToString());
        //        var usage = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.navi.ToString()] + ObjName.usage.ToString());

        //        cellSpaceMember.Add(CellSpace);
        //        CellSpace.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), cell.Id));
        //        description.Add(cell.Description);
        //        CellSpace.Add(description);
        //        name.Add(cell.Name);
        //        CellSpace.Add(name);
        //        CellSpace.Add(CreateBoundeBy());
        //        CellSpace.Add(cellSpaceGeometry);
        //        duality.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), cell.Duality));
        //        CellSpace.Add(duality);
        //        CellSpace.Add(partialboundedBy);
        //        CellSpace.Add(Class);
        //        CellSpace.Add(function);
        //        CellSpace.Add(usage);

        //        cellSpaceMembers.Add(cellSpaceMember);
        //    }


        //    return cellSpaceMembers;
        //}

        private XElement CreateCellSpaceGeometry(CellSpaceMember cell)
        {
            var cellSpaceGeometry = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.cellSpaceGeometry.ToString());
            var Geometry3D = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.Geometry3D.ToString());
            var Solid = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Solid.ToString());
            var exterior = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.exterior.ToString());
            var Shell = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Shell.ToString());
            var surfaceMembers = CreateSurfaceMenbers(cell);

            cellSpaceGeometry.Add(Geometry3D);
            Geometry3D.Add(Solid);
            Solid.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), cell.CellSpaceGeometry.Id));
            Solid.Add(exterior);
            exterior.Add(Shell);
            foreach (var surface in surfaceMembers)
            {
                Shell.Add(surface);
            }
            return cellSpaceGeometry;
        }

        private List<XElement> CreateSurfaceMenbers(CellSpaceMember cell)
        {
            var surfaceMembers = new List<XElement>();
            foreach (var surface in cell.CellSpaceGeometry.Surfaces)
            {
                var surfaceMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.surfaceMember.ToString());
                var Polygon = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Polygon.ToString());
                var exterior = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.exterior.ToString());
                var LinearRing = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.LinearRing.ToString());
                var poses = CreatePosList(surface.points);

                surfaceMember.Add(Polygon);
                Polygon.Add(exterior);
                exterior.Add(LinearRing);
                foreach (var pos in poses)
                {
                    LinearRing.Add(pos);
                }
                surfaceMembers.Add(surfaceMember);
            }
            return surfaceMembers;
        }

        private List<XElement> CreatePosList(List<Point3D> points)
        {
            var posList = new List<XElement>();

            foreach (Point3D p in points)
            {
                var pos = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.pos.ToString());
                pos.Add(new XAttribute(ObjName.srsDimension.ToString(), "3"));
                string posContent = p.x.ToString() + " " + p.y.ToString() + " " + p.z.ToString();
                pos.Add(posContent.Trim());
                posList.Add(pos);
            }

            return posList;
        }
    }
}
