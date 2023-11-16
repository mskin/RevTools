using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodesk.Revit.DB.SpecTypeId;

namespace RevTools.Objects
{
    public class ThisRevisionCloud
    {
        public string prefix { get; set; }
        public int RevisionId { get; set; }
        public int elementId { get; set; }
        public string SheetNumber { get; set; }
        public string Comments { get; set; }
      
        public ThisRevisionCloud(RevisionCloud rc)
        {
            RevisionId = rc.RevisionId.IntegerValue;
            elementId = rc.Id.IntegerValue;
            SetSheetNumbers(rc);
            Comments = rc.LookupParameter("Comments").AsValueString();
            
        }

    

        private void SetSheetNumbers(RevisionCloud rc)
        {
            List<ElementId> ids = rc.GetSheetIds().ToList();
            Document doc = rc.Document;

            foreach (ElementId id in ids)
            {
               ViewSheet vs = doc.GetElement(id) as ViewSheet;
               if (vs != null)
                {
                    SheetNumber = vs.SheetNumber + ", ";
                    prefix = vs.LookupParameter("JCJ-Drawing List Prefix").AsString();
                }
            }
           
            SheetNumber = SheetNumber.TrimEnd(',', ' ');
        }
    }
}
