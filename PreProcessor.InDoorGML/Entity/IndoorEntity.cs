using IndoorGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CellSpaceGeometry = IndoorGML.Core.CellSpaceGeometry;
using Pos = IndoorGML.Core.Pos;
using SurfaceMember = IndoorGML.Core.SurfaceMember;

namespace IndoorGML.Adapter.Entity
{
    public class IndoorEntity
    {
        public static float Scale=1;

        public string Name { get; set; }
        public InDoorPolyline Boundary { get; set; }

        public float Elevation { get; set; }
        public float Height { get; set; }

        public string DisplayName { get; internal set; }
        public string Type { get; internal set; }
        public string Level { get; set; }
        public string DualityToState
        {
            get => $"#{StateID}";
        }

        public string StateID
        {
            get => $"S{Name}";
        }
        public string Id
        {
            get => $"C{Name}";
        }
        public string DualityToCellSpace
        {
            get => $"#{Id}";
        }
        public string StateGeoId
        {
            get => $"SG-{StateID}";
        }

        public virtual Pos GetCenterPos()
        {
            var center = Boundary.GetCenter();
            return new Pos(center.X*Scale, center.Y * Scale, center.Z+Elevation);
        }

        public virtual CellSpaceGeometry ConvertGeo(InDoorPolyline roomBoundary,string id,float Elevation,float floorHeight)
        {
            roomBoundary.Standard();

            CellSpaceGeometry geo = new CellSpaceGeometry();
            geo.Geometry3D = new Geometry3D();
            geo.Geometry3D.Solid = new IndoorGML.Core.Solid();
            geo.Geometry3D.Solid.Id = "CG-"+id;
            geo.Geometry3D.Solid.Exterior = new Exterior();
            geo.Geometry3D.Solid.Exterior.Shell = new Shell();
           
            geo.Geometry3D.Solid.Exterior.Shell.SurfaceMember = ConvertPolylineToSurfaces(roomBoundary, Elevation, floorHeight).ToList();
            return geo;
        }

        public CellSpaceGeometry ConvertGeo(List<Vector3> roomBoundary, string id, float Elevation, float DefaultHeight)
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

        public CellSpaceBoundaryGeometry ConvertBoundaryGeometry(IndoorLine line, string id, float Elevation, float DefaultHeight)
        {
            CellSpaceBoundaryGeometry geo = new CellSpaceBoundaryGeometry();
            geo.Geometry3D = new Geometry3DCellSpace();
            geo.Geometry3D.Polygon = new Polygon();
            geo.Geometry3D.Polygon.Id = "CBG-" + id;
            geo.Geometry3D.Polygon.Exterior = new Exterior();
            geo.Geometry3D.Polygon.Exterior.LinearRing = ConvertLineToLinearing(line, Elevation, DefaultHeight);
            return geo;
        }

        private LinearRing ConvertLineToLinearing(IndoorLine line, float elevation, float floorHeight)
        {
            LinearRing linear = new LinearRing();
            linear.Pos = GetPolygonSide(line.StartPos, line.EndPos, elevation, floorHeight);
            return linear;
        }

        public virtual IEnumerable<SurfaceMember> ConvertPolylineToSurfaces(InDoorPolyline polyline,float Elevation,float floorHeight)
        {
            SurfaceMember bottom = new SurfaceMember();
            bottom.Polygon.Exterior = new Exterior();
            bottom.Polygon.Exterior.LinearRing = new LinearRing();
            polyline.Points.Reverse();
            bottom.Polygon.Exterior.LinearRing.Pos = GetPos(polyline,Elevation).ToList();
            polyline.Points.Reverse();

            yield return bottom;

            SurfaceMember top = new SurfaceMember();
            top.Polygon.Exterior = new Exterior();
            top.Polygon.Exterior.LinearRing = new LinearRing();
            top.Polygon.Exterior.LinearRing.Pos = GetPos(polyline, Elevation+floorHeight).ToList();

            yield return top;

            for (int i=0;i< polyline.NumberOfVertices;i++)
            {
                SurfaceMember surface = new SurfaceMember();
                surface.Polygon.Exterior = new Exterior();
                surface.Polygon.Exterior.LinearRing = new LinearRing();
                surface.Polygon.Exterior.LinearRing.Pos = GetPolygonSide(polyline.GetPoint3dAt(i), polyline.GetPoint3dAt((i + 1)% polyline.NumberOfVertices),Elevation, floorHeight);
                yield return surface;
            }
        }

        public IEnumerable<SurfaceMember> ConvertPolylineToSurfaces(List<Vector3> polyline, float Elevation, float DefaultHeight)
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
        public IEnumerable<Pos> GetPos(InDoorPolyline polyline, float elevation)
        {
            for(int i=0;i<polyline.NumberOfVertices;i++)
            {
                var p = polyline.GetPoint3dAt(i);
                yield return new Pos(p.X * Scale, p.Y * Scale, p.Z * Scale + elevation);
            }

            var last = polyline.GetPoint3dAt(0);
            yield return new Pos(last.X * Scale, last.Y * Scale, last.Z * Scale + elevation);
        }

        public IEnumerable<Pos> GetPos(List<Vector3> polyline, float elevation)
        {
            for (int i = 0; i < polyline.Count; i++)
            {
                var p = polyline[i];
                yield return new Pos(p.X * Scale, p.Y * Scale, p.Z * Scale + elevation);
            }

            var last = polyline[0];
            yield return new Pos(last.X * Scale, last.Y * Scale, last.Z * Scale + elevation);
        }

        public List<Pos> GetPolygonSide(Vector3 start, Vector3 end,float Elevation,float Height)
        {
            return new List<Pos>()
            {
                new Pos(start.X* Scale,start.Y* Scale,start.Z* Scale+Elevation),
                new Pos(end.X* Scale, end.Y* Scale, end.Z* Scale + Elevation),
                new Pos(end.X* Scale, end.Y* Scale, end.Z* Scale + Elevation+Height),
                new Pos(start.X* Scale, start.Y* Scale, start.Z* Scale + Elevation + Height),
                new Pos(start.X* Scale,start.Y* Scale,start.Z* Scale+Elevation)
            };
        }

        public virtual CellSpaceMember ToCellSpaceMember()
        {
            CellSpaceMember member = new CellSpaceMember();
            member.GeneralSpace = new CellSpace();
            member.GeneralSpace.Id = Id;
            member.GeneralSpace.Name = Name;
            
            member.GeneralSpace.Duality = new Duality() { Href = DualityToState };
            member.GeneralSpace.CellSpaceGeometry = ConvertGeo(Boundary, Id, Elevation, Height);
            return member;
        }

        public StateMember ToStateMember()
        {
            StateMember state = new StateMember();
            state.State.Id = StateID;
            state.State.Duality = new Duality() { Href = DualityToCellSpace };
            state.State.Geometry = ConvertCenterPos();
            return state;
        }

        public Geometry ConvertCenterPos()
        {
            Geometry geo = new Geometry();
            geo.Point = new Point();
            geo.Point.Id = StateGeoId;
            geo.Point.Pos = GetCenterPos();
            return geo;
        }
    }
    
}