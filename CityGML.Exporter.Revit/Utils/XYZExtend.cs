using Autodesk.Revit.DB;

public static class XYZExtend
{
    public static Point3DIntra.Point3D ToPoint3D(this XYZ v)
    {
        return new Point3DIntra.Point3D(v.X, v.Y, v.Z);
    }
}