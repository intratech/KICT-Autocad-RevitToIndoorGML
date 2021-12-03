using GML.Core.DTO.Json;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GML.Core
{
    public class GMLUtil
    {
        /// <summary>
        /// This method calculates the surface from base line and moving vector.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="moveVec"></param>
        /// <returns></returns>
        public SurfaceMember GetSurface(Point3D startPoint, Point3D endPoint, Point3D moveVec)
        {
            if (startPoint != null && endPoint != null && moveVec != null)
            {
                Point3D point1 = new Point3D(startPoint.x, startPoint.y, startPoint.z);
                Point3D point2 = point1 + moveVec;

                Point3D point4 = new Point3D(endPoint.x, endPoint.y, endPoint.z);
                Point3D point3 = point4 + moveVec;

                SurfaceMember surface = new SurfaceMember();
                surface.points.Add(point1);
                surface.points.Add(point2);
                surface.points.Add(point3);
                surface.points.Add(point4);
                surface.points.Add(point1);
                return surface;
            }

            return null;
        }
     
    }
}
