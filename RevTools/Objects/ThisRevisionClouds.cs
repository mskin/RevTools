using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevTools.Objects
{
    internal class ThisRevisionClouds
    {
        public ObservableCollection<ThisRevisionCloud> _All = new ObservableCollection<ThisRevisionCloud>();

        public ThisRevisionClouds(Revision revision)
        {
            ElementId revId = revision.Id;
            Document doc = revision.Document;

            foreach (RevisionCloud revCloud in Utiilities.GetRevisionsClouds(doc))
            {
                if (revCloud.RevisionId == revId)
                {
                    _All.Add(new ThisRevisionCloud(revCloud));
                }
            }
        }
    }
}
