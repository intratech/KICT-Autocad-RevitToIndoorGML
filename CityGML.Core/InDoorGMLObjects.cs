using GML.Core.DTO.Json;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GML.Core
{
    public class CellSpaceMember
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public CellSpaceGeometry CellSpaceGeometry { get; set; }
        public string Duality { get; set; }
        public string PartialboundedBy { get; set; }
        public CellSpaceMember(string Id, string Description, string Name, CellSpaceGeometry CellSpaceGeometry, string Duality, string PartialboundedBy)
        {
            this.Id = Id;
            this.Description = Description;
            this.Name = Name;
            this.CellSpaceGeometry = CellSpaceGeometry;
            this.Duality = Duality;
            this.PartialboundedBy = PartialboundedBy;
        }
    }
    public class CellSpaceGeometry
    {
        public string Id { get; set; }
        public List<SurfaceMember> Surfaces { get; set; }
        public CellSpaceGeometry(string Id, List<SurfaceMember> Surfaces)
        {
            this.Id = Id;
            this.Surfaces = Surfaces;
        }
    }
    public class CellSpaceBoundaryMember
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Duality { get; set; }
        public SurfaceMember CellSpaceBoundaryGeometry { get; set; }
        public CellSpaceBoundaryMember(string Id, string Description, string Name, string Duality, SurfaceMember CellSpaceBoundaryGeometry)
        {
            this.Id = Id;
            this.Description = Description;
            this.Name = Name;
            this.Duality = Duality;
            this.CellSpaceBoundaryGeometry = CellSpaceBoundaryGeometry;
            if(this.CellSpaceBoundaryGeometry!=null)
                this.CellSpaceBoundaryGeometry.Id = $"CBG-{Id}";
        }
    }
    public class TransitionMember
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public List<string> Connects { get; set; }
        public string Duality { get; set; }
        public LineStringGeo LineStringGeo { get; set; }
        public TransitionMember(string Id, string Description, string Name, string Weight, List<string> Connects, string Duality, LineStringGeo LineStringGeo)
        {
            this.Id = Id;
            this.Description = Description;
            this.Name = Name;
            this.Weight = Weight;
            this.Connects = Connects;
            this.Duality = Duality;
            this.LineStringGeo = LineStringGeo;
        }
    }
    public class LineStringGeo
    {
        public string Id { get; set; }
        public List<Point3D> Positions { get; set; }
        public LineStringGeo(string Id, List<Point3D> Positions)
        {
            this.Id = Id;
            this.Positions = Positions;
        }
    }
    public class StateMember
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Duality { get; set; }
        public List<string> Connects { get; set; }
        public PointGeo PointGeo { get; set; }
        public StateMember(string Id, string Description, string Name, string Duality, List<string> Connects, PointGeo PointGeo)
        {
            this.Id = Id;
            this.Description = Description;
            this.Name = Name;
            this.Duality = Duality;
            this.Connects = Connects;
            this.PointGeo = PointGeo;
        }
    }
    public class PointGeo
    {
        public string Id { get; set; }
        public Point3D Position { get; set; }
        public PointGeo(string Id, Point3D Position)
        {
            this.Id = Id;
            this.Position = Position;
        }
    }
}
