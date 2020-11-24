using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IndoorGML.Exporter.Revit
{
    class ExportContext : IExportContext
    {
        private Document m_doc = null;
        private XmlTextWriter xmlWt;
        private ElementId mCurrentElement;
        private Transform mCurrentTransform;
        private Material mCurrentMat;
        private Transformation m_transforms = new Transformation();
        private int instanceLv = 0;
        private int maxInstanceLv = 0;

        private GeometryInfo geoInfo = new GeometryInfo();


        public ExportContext(Document doc, string outputFile)
        {
            this.m_doc = doc;
            //ufxWt = new ufxWriter(outputFolder + "\\" + fileName + ".ufxx", false, 1000);
            var 
            xmlWt = new XmlTextWriter(Path.Combine(Path.GetDirectoryName(outputFile) , Path.GetFileNameWithoutExtension(outputFile) + ".xml"), Encoding.UTF8);
            //xmlWt.Formatting = Formatting.Indented;
            //xmlWt.Indentation = 2;
        }
        public bool Start()
        {
            xmlWt.WriteStartDocument(false);
            xmlWt.WriteStartElement("Document");
            return true;
        }
        public void Finish()
        {
            xmlWt.WriteStartElement("MaxInstance");
            xmlWt.WriteString(maxInstanceLv.ToString());
            xmlWt.WriteEndElement();

            xmlWt.WriteEndElement();
            xmlWt.WriteEndDocument();
            xmlWt.Flush();
            xmlWt.Close();
            xmlWt = null;
            //ufxWt.Save();
        }

        public bool IsCanceled()
        {
            return false;
        }

        

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            xmlWt.WriteStartElement("Element");
            mCurrentElement = elementId;
            instanceLv = 0;
            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            xmlWt.WriteEndElement();
            if (instanceLv > maxInstanceLv)
                maxInstanceLv = instanceLv;
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            xmlWt.WriteStartElement("Face");
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {
            xmlWt.WriteEndElement();
        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            xmlWt.WriteStartElement("Instance");
            m_transforms.PushTransform(node.GetTransform());
            instanceLv++;
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            xmlWt.WriteEndElement();
            m_transforms.PopTransform();
        }

        public void OnLight(LightNode node)
        {
            xmlWt.WriteStartElement("OnLight");
            xmlWt.WriteEndElement();
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            xmlWt.WriteStartElement("Link");
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
            xmlWt.WriteEndElement();
        }

        public void OnMaterial(MaterialNode node)
        {
            mCurrentMat = m_doc.GetElement(node.MaterialId) as Material;
            xmlWt.WriteStartElement("Material");
            xmlWt.WriteEndElement();
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            xmlWt.WriteStartElement("OnPolymesh");
            xmlWt.WriteEndElement();
            return;
            try
            {
                //Faces
                var f1 = node.GetFacets();
                var f2 = new int[node.NumberOfFacets * 3];
                var fPos = 0;
                foreach (var f in f1)
                {
                    f2[fPos + 0] = f.V1;
                    f2[fPos + 1] = f.V2;
                    f2[fPos + 2] = f.V3;
                    fPos += 3;
                }
                //xmlWt.WriteStartElement("Face");
                //xmlWt.WriteString(string.Join(" ", f2));
                //xmlWt.WriteEndElement();

                //Points
                var p1 = m_transforms.ApplyTransform(node.GetPoints());
                var p2 = new float[node.NumberOfPoints * 3];
                var pPos = 0;
                foreach (var p in p1)
                {
                    p2[pPos + 0] = (float)p.X * 1000;
                    p2[pPos + 1] = (float)p.Z * 1000;
                    p2[pPos + 2] = (float)p.Y * 1000;
                    pPos += 3;
                }
                //xmlWt.WriteStartElement("Point");
                //xmlWt.WriteString(string.Join(" ", p2));
                //xmlWt.WriteEndElement();

                //UV
                //node.GetUV(0).

                //Material
                var mat = new float[4];
                if (mCurrentMat != null && mCurrentMat.Color != null)
                {
                    mat[0] = (float)(mCurrentMat.Color.Red / 255.0);
                    mat[1] = (float)(mCurrentMat.Color.Green / 255.0);
                    mat[2] = (float)(mCurrentMat.Color.Blue / 255.0);
                    mat[3] = 1f;
                }
                //xmlWt.WriteStartElement("Material");
                //xmlWt.WriteString(string.Join(" ", mat));
                //xmlWt.WriteEndElement();

                //Bounding box
                var box = m_doc.GetElement(mCurrentElement).get_BoundingBox(null);
                var box2 = new float[] {
                    (float)box.Min.X*1000, (float)box.Min.Z*1000, (float)box.Min.Y*1000,
                    (float)box.Max.X*1000, (float)box.Max.Z*1000, (float)box.Max.Y*1000,
                };
                //xmlWt.WriteStartElement("BoundingBox");
                //xmlWt.WriteString(string.Join(" ", box2));
                //xmlWt.WriteEndElement();

                //ufxWt.AddMesh(p2, f2, box2, mat, null, mCurrentElement.IntegerValue);
                //xmlWt.WriteEndElement();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception when process mesh: " + ex.StackTrace);
            }
        }

        public void OnRPC(RPCNode node)
        {
            xmlWt.WriteStartElement("RPC");
            xmlWt.WriteEndElement();
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            xmlWt.WriteStartElement("View");
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
            xmlWt.WriteEndElement();
        }
    }
}
