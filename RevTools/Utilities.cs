using Autodesk.Revit.DB;
using RevTools.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevTools
{
    public static class Utiilities
    {
        /// <summary>
        /// Returns a list of ViewSheets the have revisions on them.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ViewSheet> GetSheetsWithRevisionClouds(FilteredElementCollector col, Document doc)
        {
            List<ViewSheet> sheets = new List<ViewSheet>();

            foreach (Element e in col)
            {
                RevisionCloud rc = e as RevisionCloud;

                foreach (ElementId elid in rc.GetSheetIds())
                {
                    ViewSheet vs = doc.GetElement(elid) as ViewSheet;
                    if (!sheets.Contains(vs))
                    {
                        sheets.Add(vs);
                    }

                }
            }
            return sheets;
        }
        /// <summary>
        /// Returns a list of ViewSheets the have revisions on them.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ViewSheet> GetSheetsWithRevisionClouds(List<ElementId> ids, Document doc)
        {
            List<ViewSheet> sheets = new List<ViewSheet>();

            foreach (ElementId e in ids)
            {
                RevisionCloud rc = doc.GetElement(e) as RevisionCloud;

                foreach (ElementId elid in rc.GetSheetIds())
                {
                    ViewSheet vs = doc.GetElement(elid) as ViewSheet;
                    if (!sheets.Contains(vs))
                    {
                        sheets.Add(vs);
                    }

                }
            }
            return sheets;
        }
        /// <summary>
        /// Returns a list of ViewSheets the have revisions on them.
        /// </summary>
        /// <param name="filteredrevClouds"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ViewSheet> GetSheetsWithRevisionClouds(List<ThisRevisionCloud> filteredrevClouds, Document doc)
        {
            
            List<ViewSheet> sheets = new List<ViewSheet>();
            
            foreach ( ThisRevisionCloud trc in filteredrevClouds)
            {
                RevisionCloud rc = doc.GetElement(new ElementId(trc.elementId)) as RevisionCloud;
                
                foreach (ElementId id in rc.GetSheetIds().ToList())
                {
                    ViewSheet vs = doc.GetElement(id) as ViewSheet;
                    if (!sheets.Contains(vs))
                    {
                        sheets.Add(vs);
                    }
                }
            }
            return sheets;
        }
        /// <summary>
        /// Returns a list of the revisions in the project.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal static List<Revision> GetRevisions(Document doc)
        {
           
            FilteredElementCollector CollectionOfRevisionsAsElement = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Revisions).OfClass(typeof(Revision));
            List<Revision> revisions = new List<Revision>();

            foreach (Element e in CollectionOfRevisionsAsElement)
            {
                Revision rc = e as Revision;
                revisions.Add(rc);
            }
            
            return revisions;
        }
        /// <summary>
        /// Returns a list of Revision Clouds in the entire project
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<RevisionCloud> GetRevisionsClouds(Document doc)
        {
            FilteredElementCollector CollectionOfRevisionsAsElement = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_RevisionClouds).OfClass(typeof(RevisionCloud));
            List<RevisionCloud> revisionClouds = new List<RevisionCloud>();

            foreach (Element e in CollectionOfRevisionsAsElement)
            {
                RevisionCloud rc = e as RevisionCloud;
                revisionClouds.Add(rc);
            }

            return revisionClouds;
        }
        /// <summary>
        /// Gets All the Element Ids of ALL the revision clouds in the model.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ICollection<ElementId> GetRevisionCloudIds(Document doc)
        {
            ICollection<ElementId> elementSet = new HashSet<ElementId>();

            List<RevisionCloud> rc = GetRevisionsClouds(doc);
            foreach (Element e in rc)
            {
                elementSet.Add(e.Id);
            }

            return elementSet;
        }
        /// <summary>
        /// Gets a list of all the element ids of revisionClouds NOT in the filtered set.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="FilteredrevClouds"></param>
        /// <returns></returns>
        public static ICollection<ElementId> GetRevisionIdsNotInThisSet(Document doc, List<ThisRevisionCloud> FilteredrevClouds)
        {
            ICollection<ElementId> elementSet = new HashSet<ElementId>();

            ICollection<ElementId> AllId = GetRevisionCloudIds(doc);
            ICollection<ElementId> FilteredId = new HashSet<ElementId>();

            foreach(ThisRevisionCloud rc in FilteredrevClouds)
            {
                FilteredId.Add(new ElementId(rc.elementId));
            }

            foreach (ElementId id in AllId)
            {
                if (!FilteredId.Contains(id))
                {
                    elementSet.Add(id);
                }
            }

            return elementSet;
        }

        
    }
}
