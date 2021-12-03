using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using Autodesk.AutoCAD.Windows;
using CityGML.Exporter.AutoCAD.UI;
using System;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CityGML.Exporter.AutoCAD
{
    public class IndoorGMLPalette
    {
        // We cannot derive from PaletteSet
        // so we contain it
        static PaletteSet ps;
        // We need to make the textbox available
        // via a static member
        static IndoorGMLControl tvc;
        public IndoorGMLPalette()
        {
            tvc = new IndoorGMLControl();
        }
        public void Show()
        {
            if (ps == null)
            {
                ps = new PaletteSet("IndoorGML Exporter",new Guid("71e27d47-6fcd-447c-b20f-d6a58bfbe54b"));
                
                ps.Style =
                  PaletteSetStyles.NameEditable |
                  PaletteSetStyles.ShowPropertiesMenu |
                  PaletteSetStyles.ShowAutoHideButton |
                  PaletteSetStyles.ShowCloseButton;
                ps.MinimumSize =
                  new System.Drawing.Size(350, 300);
                ps.KeepFocus = false;
                ps.Dock = DockSides.Left;
                
                ps.Add("IndoorGML Exporter", tvc);
            }
            try
            {
                ps.Visible = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SetObjectText(string text)
        {
            //tvc.typeTextBox.Text = text;
        }
        public void SetObjectIds(ObjectIdCollection ids)
        {
            if (ids.Count < 0)
            {
                SetObjectText("");
            }
            else
            {
                Document doc =
                  Autodesk.AutoCAD.ApplicationServices.
                    Application.DocumentManager.MdiActiveDocument;
                DocumentLock loc =
                  doc.LockDocument();
                using (loc)
                {
                    string info =
                      "Number of objects: " +
                      ids.Count.ToString() + "\r\n";
                    Transaction tr =
                      doc.TransactionManager.StartTransaction();
                    using (tr)
                    {
                        foreach (ObjectId id in ids)
                        {
                            Entity ent =
                              (Entity)tr.GetObject(id, OpenMode.ForRead);
                            Solid3d sol = ent as Solid3d;
                            if (sol != null)
                            {
                                Acad3DSolid oSol =
                                  (Acad3DSolid)sol.AcadObject;
                                // Put in a try-catch block, as it's possible
                                // for solids to not support this property,
                                // it seems (better safe than sorry)
                                try
                                {
                                    string solidType = oSol.SolidType;
                                    info +=
                                      ent.GetType().ToString() +
                                      " (" + solidType + ") : " +
                                      ent.ColorIndex.ToString() + "\r\n";
                                }
                                catch (System.Exception)
                                {
                                    info +=
                                      ent.GetType().ToString() +
                                      " : " +
                                      ent.ColorIndex.ToString() + "\r\n";
                                }
                            }
                            else
                            {
                                info +=
                                  ent.GetType().ToString() +
                                  " : " +
                                  ent.ColorIndex.ToString() + "\r\n";
                            }
                        }
                        tr.Commit();
                    }
                    SetObjectText(info);
                }
            }
        }
        public void SetObjectId(ObjectId id)
        {
            if (id == ObjectId.Null)
            {
                SetObjectText("");
            }
            else
            {
                Document doc =
                  Autodesk.AutoCAD.ApplicationServices.
                    Application.DocumentManager.MdiActiveDocument;
                DocumentLock loc =
                  doc.LockDocument();
                using (loc)
                {
                    Transaction tr =
                      doc.TransactionManager.StartTransaction();
                    using (tr)
                    {
                        DBObject obj =
                          tr.GetObject(id, OpenMode.ForRead);
                        SetObjectText(obj.GetType().ToString());
                        tr.Commit();
                    }
                }
            }
        }
    }
}