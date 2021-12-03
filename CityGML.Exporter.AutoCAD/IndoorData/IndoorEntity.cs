using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using GML.Core;
using GML.Core.DTO.Json;
using IndoorGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CellSpaceGeometry = IndoorGML.Core.CellSpaceGeometry;
using Pos = IndoorGML.Core.Pos;
using SurfaceMember = IndoorGML.Core.SurfaceMember;

namespace CityGML.Exporter.AutoCAD.IndoorData
{
    public class IndoorEntity
    {
        public CellSpaceGeometry ConvertGeo(Polyline roomBoundary,string id,float Elevation,float DefaultHeight)
        {
            CellSpaceGeometry geo = new CellSpaceGeometry();
            geo.Geometry3D = new Geometry3D();
            geo.Geometry3D.Solid = new IndoorGML.Core.Solid();
            geo.Geometry3D.Solid.Id = "CG-"+id;
            geo.Geometry3D.Solid.Exterior = new Exterior();
            geo.Geometry3D.Solid.Exterior.Shell = new Shell();
           
            geo.Geometry3D.Solid.Exterior.Shell.SurfaceMember = ConvertPolylineToSurfaces(roomBoundary, Elevation, DefaultHeight).ToList();
            return geo;
        }

        public CellSpaceGeometry ConvertGeo(List<Point3d> roomBoundary, string id, float Elevation, float DefaultHeight)
        {
            CellSpaceGeometry geo = new CellSpaceGeometry();
            geo.Geometry3D = new Geometry3D();
            geo.Geometry3D.Solid = new IndoorGML.Core.Solid();
            geo.Geometry3D.Solid.Id = "CG-" + id;
            geo.Geometry3D.Solid.Exterior = new Exterior();
            geo.Geometry3D.Solid.Exterior.Shell = new Shell();

            geo.Geometry3D.Solid.Exterior.Shell.SurfaceMember = ConvertPolylineToSurfaces(roomBoundary, Elevation, DefaultHeight).ToList();
            return geo;
        }

        public CellSpaceBoundaryGeometry ConvertBoundaryGeometry(Line line, string id, float Elevation, float DefaultHeight)
        {
            CellSpaceBoundaryGeometry geo = new CellSpaceBoundaryGeometry();
            geo.Geometry3D = new Geometry3DCellSpace();
            geo.Geometry3D.Polygon = new Polygon();
            geo.Geometry3D.Polygon.Id = "CBG-" + id;
            geo.Geometry3D.Polygon.Exterior = new Exterior();
            geo.Geometry3D.Polygon.Exterior.LinearRing = ConvertLineToLinearing(line, Elevation, DefaultHeight);
            return geo;
        }

        private LinearRing ConvertLineToLinearing(Line line, float elevation, float floorHeight)
        {
            LinearRing linear = new LinearRing();
            linear.Pos = GetPolygonSide(line.StartPoint,line.EndPoint, elevation, floorHeight);
            return linear;
        }

        public IEnumerable<SurfaceMember> ConvertPolylineToSurfaces(Polyline polyline,float Elevation,float DefaultHeight)
        {
            SurfaceMember bottom = new SurfaceMember();
            bottom.Polygon.Exterior = new Exterior();
            bottom.Polygon.Exterior.LinearRing = new LinearRing();
            bottom.Polygon.Exterior.LinearRing.Pos = GetPos(polyline,Elevation).ToList();

            yield return bottom;

            SurfaceMember top = new SurfaceMember();
            top.Polygon.Exterior = new Exterior();
            top.Polygon.Exterior.LinearRing = new LinearRing();
            top.Polygon.Exterior.LinearRing.Pos = GetPos(polyline, Elevation+DefaultHeight).ToList();

            yield return top;

            for (int i=0;i< polyline.NumberOfVertices;i++)
            {
                SurfaceMember surface = new SurfaceMember();
                surface.Polygon.Exterior = new Exterior();
                surface.Polygon.Exterior.LinearRing = new LinearRing();
                surface.Polygon.Exterior.LinearRing.Pos = GetPolygonSide(polyline.GetPoint3dAt(i), polyline.GetPoint3dAt((i + 1)% polyline.NumberOfVertices),Elevation, DefaultHeight);
                yield return surface;
            }
        }

        public IEnumerable<SurfaceMember> ConvertPolylineToSurfaces(List<Point3d> polyline, float Elevation, float DefaultHeight)
        {
            SurfaceMember bottom = new SurfaceMember();
            bottom.Polygon.Exterior = new Exterior();
            bottom.Polygon.Exterior.LinearRing = new LinearRing();
            bottom.Polygon.Exterior.LinearRing.Pos = GetPos(polyline, Elevation).ToList();

            yield return bottom;

            SurfaceMember top = new SurfaceMember();
            top.Polygon.Exterior = new Exterior();
            top.Polygon.Exterior.LinearRing = new LinearRing();
            top.Polygon.Exterior.LinearRing.Pos = GetPos(polyline, Elevation + DefaultHeight).ToList();

            yield return top;

            for (int i = 0; i < polyline.Count; i++)
            {
                SurfaceMember surface = new SurfaceMember();
                surface.Polygon.Exterior = new Exterior();
                surface.Polygon.Exterior.LinearRing = new LinearRing();
                surface.Polygon.Exterior.LinearRing.Pos = GetPolygonSide(polyline[i], polyline[(i + 1) % polyline.Count], Elevation, DefaultHeight);
                yield return surface;
            }
        }
        public IEnumerable<Pos> GetPos(Polyline polyline, float elevation)
        {
            for(int i=0;i<polyline.NumberOfVertices;i++)
            {
                var p = polyline.GetPoint3dAt(i);
                yield return new Pos(p.X * API.Scale, p.Y * API.Scale, p.Z * API.Scale + elevation);
            }

            var last = polyline.GetPoint3dAt(0);
            yield return new Pos(last.X * API.Scale, last.Y * API.Scale, last.Z * API.Scale + elevation);
        }

        public IEnumerable<Pos> GetPos(List<Point3d> polyline, float elevation)
        {
            for (int i = 0; i < polyline.Count; i++)
            {
                var p = polyline[i];
                yield return new Pos(p.X * API.Scale, p.Y * API.Scale, p.Z * API.Scale + elevation);
            }

            var last = polyline[0];
            yield return new Pos(last.X * API.Scale, last.Y * API.Scale, last.Z * API.Scale + elevation);
        }

        public List<Pos> GetPolygonSide(Point3d start, Point3d end,float Elevation,float Height)
        {
            return new List<Pos>()
            {
                new Pos(start.X* API.Scale,start.Y* API.Scale,start.Z* API.Scale+Elevation),
                new Pos(end.X* API.Scale, end.Y* API.Scale, end.Z* API.Scale + Elevation),
                new Pos(end.X* API.Scale, end.Y* API.Scale, end.Z* API.Scale + Elevation+Height),
                new Pos(start.X* API.Scale, start.Y* API.Scale, start.Z* API.Scale + Elevation + Height),
                new Pos(start.X* API.Scale,start.Y* API.Scale,start.Z* API.Scale+Elevation)
            };
        }
    }
}