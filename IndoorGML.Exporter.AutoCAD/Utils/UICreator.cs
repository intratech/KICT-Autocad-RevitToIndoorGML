using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

namespace CityGML.Exporter.AutoCAD.Utils
{
    public class UICreator
    {
        //https://forums.autodesk.com/t5/net/net-c-ribbon-tab/td-p/3920030
        public static RibbonTab CreateRibbonTab(string idTab, string title)
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon != null)
            {
                RibbonTab rtab = ribbon.FindTab(idTab);
                if (rtab != null)
                {
                    return rtab;
                }
                rtab = new RibbonTab();
                rtab.Title = title;
                rtab.Id = idTab;
                //Add the Tab
                ribbon.Tabs.Add(rtab);
                return rtab;
            }
            return null;
        }

        public static RibbonPanel CreateRibbonPanel(RibbonTab ribbonTab,string idPanel, string title)
        {
            if (ribbonTab.Panels.Count > 0)
            {
                var ribbonPanel = ribbonTab.Panels.Where(x => x.Id == idPanel).FirstOrDefault();
                if (ribbonPanel != null)
                    return ribbonPanel;
            }

            RibbonPanelSource rps = new RibbonPanelSource();
            rps.Title = title;
            RibbonPanel rp = new RibbonPanel();
            rp.Id = idPanel;
            rp.Source = rps;
            ribbonTab.Panels.Add(rp);
            return rp;
        }

        public static RibbonButton CreateRibbonButton(RibbonPanel panel,string idButton,string title,ImageSource image, ImageSource largeImage)
        {
            //RibbonButton rci = new RibbonButton();
            //rci.Name = idButton;
            //rci.Text = title;

            ////assign the Command Item to the DialgLauncher which auto-enables
            //// the little button at the lower right of a Panel
            //panel.Source.DialogLauncher = rci;
            var rb = panel.Source.Items.Where(x => x.Id == idButton).FirstOrDefault();
            if(rb != null)
            {
                return rb as RibbonButton;
            }

            rb = new RibbonButton();
            rb.Name = idButton;
            rb.ShowText = true;
            rb.Text = title;
            rb.Image = image;
            rb.LargeImage = largeImage;
            rb.ShowImage = true;
            
            //Add the Button to the Tab
            panel.Source.Items.Add(rb);
            
            return rb as RibbonButton;
        }
    }
}
