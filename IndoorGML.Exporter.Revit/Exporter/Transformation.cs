using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit
{
    public class Transformation
    {
        private Stack<Transform> m_transforms = new Stack<Transform>();
        private Transform m_currentTransform = Transform.Identity;

        public void PushTransform(Transform t)
        {
            m_transforms.Push(m_currentTransform);
            m_currentTransform = m_currentTransform.Multiply(t);
        }

        public void PopTransform()
        {
            if (m_transforms.Count > 0)
            {
                m_currentTransform = m_transforms.Pop();
            }
            else
            {
                //warning
            }
        }

        public void ClearTransforms()
        {
            m_transforms.Clear();
            m_currentTransform = Transform.Identity;
        }

        public IList<XYZ> ApplyTransform(IList<XYZ> points)
        {
            IList<XYZ> newPoints = new List<XYZ>(points.Count);

            foreach (XYZ xyz in points)
            {
                newPoints.Add(m_currentTransform.OfPoint(xyz));
            }

            return newPoints;
        }
    }
}
