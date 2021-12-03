using CityGML.Core;
using CityGML.Core.DTO.Json;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace InDoorGMLWriter
{
    public partial class Writer
    {
        private List<XElement> CreateCellSpaceBoundaryMembers(List<CellSpaceBoundaryMember> cellSpaceBoundaries)
        {
            var cellSpaceBoundaryMembers = new List<XElement>();
            if (cellSpaceBoundaries != null)
            {
                foreach (var cellBoundary in cellSpaceBoundaries)
                {
                    if (cellBoundary.CellSpaceBoundaryGeometry == null || cellBoundary.CellSpaceBoundaryGeometry.points == null || cellBoundary.CellSpaceBoundaryGeometry.points.Count == 0)
                        continue;

                    var cellSpaceBoundaryMember = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.cellSpaceBoundaryMember.ToString());
                    var CellSpaceBoundary = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.CellSpaceBoundary.ToString());
                    var description = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.description.ToString());
                    var name = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.name.ToString());
                    var duality = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.duality.ToString());
                    var cellSpaceBoundaryGeometry = CreateBoundeBycellSpaceBoundaryGeometry(cellBoundary);

                    CellSpaceBoundary.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), cellBoundary.Id));
                    cellSpaceBoundaryMember.Add(CellSpaceBoundary);
                    description.Add(cellBoundary.Description);
                    CellSpaceBoundary.Add(description);
                    name.Add(cellBoundary.Name);
                    CellSpaceBoundary.Add(name);
                    CellSpaceBoundary.Add(CreateBoundeBy());
                    duality.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.xlink.ToString()] + ObjName.href.ToString(), cellBoundary.Duality));
                    CellSpaceBoundary.Add(duality);
                    CellSpaceBoundary.Add(cellSpaceBoundaryGeometry);

                    cellSpaceBoundaryMembers.Add(cellSpaceBoundaryMember);
                }
            }
            return cellSpaceBoundaryMembers;
        }

        private XElement CreateBoundeBycellSpaceBoundaryGeometry(CellSpaceBoundaryMember cellBoundary)
        {            
            var cellSpaceBoundaryGeometry = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.cellSpaceBoundaryGeometry.ToString());
            var geometry3D = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.core.ToString()] + ObjName.geometry3D.ToString());
            var Polygon = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.Polygon.ToString());
            var exterior = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.exterior.ToString());
            var LinearRing = new XElement(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.LinearRing.ToString());        

            cellSpaceBoundaryGeometry.Add(geometry3D);
            Polygon.Add(new XAttribute(xmlnamespace[XNamespace.Xmlns + ObjName.gml.ToString()] + ObjName.id.ToString(), cellBoundary.CellSpaceBoundaryGeometry.Id));
            geometry3D.Add(Polygon);
            Polygon.Add(exterior);
            exterior.Add(LinearRing);

            var posList = CreatePosList(cellBoundary.CellSpaceBoundaryGeometry.points);
            foreach (var item in posList)
            {
                LinearRing.Add(item);
            }

            return cellSpaceBoundaryGeometry;
        }

    }
}
