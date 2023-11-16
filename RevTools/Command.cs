#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevTools.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace RevTools
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public List<ThisRevisionCloud> FilteredrevClouds;
        UIApplication uiapp;
        UIDocument uidoc;
        Application app;
        Document doc;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            
            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;

            WPF_Window window = new WPF_Window(commandData);
            window.ShowDialog();

            if (window.isolateClouds == true)
            {
                FilteredrevClouds = window.FilteredRevisionClouds.ToList();
                HideUnFilteredRevisions();
            }

            if (window.unHide == true)
            {
                UnHideAll();
            }
            return Result.Succeeded;
        }

        private void UnHideAll()
        {
            List<ElementId> idsToShow = Utiilities.GetRevisionCloudIds(doc).ToList();
            List<ViewSheet> sheets = Utiilities.GetSheetsWithRevisionClouds(idsToShow, doc);

            using (Transaction transaction = new Transaction(doc, "Show Hidden Clouds"))
            {
                transaction.Start();

                foreach (ViewSheet sheet in sheets)
                {
                    ElementId sheetId = sheet.Id;
                    ICollection<ElementId> ids = new HashSet<ElementId>();

                    foreach (ElementId id in idsToShow)
                    {
                        Element el = doc.GetElement(id);
                        if (el.IsHidden(sheet))
                        {
                            ids.Add(id);
                        }
                    }
                    if (ids.Count > 0)
                    {
                        sheet.UnhideElements(ids);
                    }

                }
                    transaction.Commit();
            }
        }

        private void HideUnFilteredRevisions()
        {
            List<ElementId> idsToHids = Utiilities.GetRevisionIdsNotInThisSet(doc, FilteredrevClouds).ToList();
            List<ViewSheet> sheets = Utiilities.GetSheetsWithRevisionClouds(idsToHids, doc);

            UnHideAll();

            using (Transaction transaction = new Transaction(doc, "Hide Un Filtered Revisions"))
            {
                transaction.Start();

                foreach (ViewSheet sheet in sheets)
                {
                    ElementId sheetId = sheet.Id;
                    ICollection<ElementId> ids = new HashSet<ElementId>();

                    foreach (ElementId id in idsToHids)
                    {
                        Element el = doc.GetElement(id);
                        if (el.CanBeHidden(sheet) && !el.IsHidden(sheet))
                        {
                            ids.Add(id);
                        }
                    }
                    if (ids.Count > 0)
                    {
                        sheet.HideElements(ids);
                    }

                }

                transaction.Commit();
            }
        }
    }
}
