using Autodesk.Revit.DB;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace RevTools.Objects
{
    internal static class ExcellGenerator
    {
        internal static void CreateExcell(Document doc)
        {
            FilteredElementCollector CollectionOfRevisionsAsElement = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_RevisionClouds).OfClass(typeof(RevisionCloud));
            List<RevisionCloud> revisionClouds = new List<RevisionCloud>();

            foreach (Element el in CollectionOfRevisionsAsElement)
            {
                RevisionCloud rc = el as RevisionCloud;
                revisionClouds.Add(rc);
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true) // Show the dialog and check if the result is OK.
            {
                Excel.Application xlApp = new Excel.Application();
                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }

                Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // Add headers
                xlWorkSheet.Cells[1, 1] = "Sequence";
                xlWorkSheet.Cells[1, 2] = "RevisionNumber";
                xlWorkSheet.Cells[1, 3] = "Date";
                xlWorkSheet.Cells[1, 4] = "Description";
                xlWorkSheet.Cells[1, 5] = "IssuedTo";
                xlWorkSheet.Cells[1, 6] = "IssuedBy";
                xlWorkSheet.Cells[1, 7] = "Issued";
                xlWorkSheet.Cells[1, 8] = "Sheet";
                xlWorkSheet.Cells[1, 9] = "View";
                xlWorkSheet.Cells[1, 10] = "ElementId";

                int row = 2;

                foreach (RevisionCloud revisionCloud in revisionClouds)
                {
                    Revision revision = doc.GetElement(revisionCloud.RevisionId) as Revision;

                    string sequenceNumber = revision.SequenceNumber.ToString();
                    string revisionNumber = revision.RevisionNumber.ToString();
                    string date = revision.RevisionDate.ToString();
                    string description = revision.Description.ToString();
                    string issuedTo = revision.IssuedTo.ToString();
                    string issuedBy = revision.IssuedBy.ToString();
                    bool issued = revision.Issued;
                    string sheet = "";
                    string ownerView = "";

                    List<ElementId> ids = revisionCloud.GetSheetIds().ToList();
                    foreach (ElementId id in ids)
                    {
                        ViewSheet vs = doc.GetElement(id) as ViewSheet;
                        sheet = sheet + vs.SheetNumber + ",";
                    }
                    if (sheet.Length > 0)
                    {
                        sheet = sheet.Substring(0, sheet.Length - 1);
                    }
                    View view = doc.GetElement(revisionCloud.OwnerViewId) as View;
                    ownerView = view.Name;

                    string elementId = revisionCloud.Id.ToString();

                    // Write data to Excel cells
                    xlWorkSheet.Cells[row, 1] = sequenceNumber;
                    xlWorkSheet.Cells[row, 2] = revisionNumber;
                    xlWorkSheet.Cells[row, 3] = date;
                    xlWorkSheet.Cells[row, 4] = description;
                    xlWorkSheet.Cells[row, 5] = issuedTo;
                    xlWorkSheet.Cells[row, 6] = issuedBy;
                    xlWorkSheet.Cells[row, 7] = issued.ToString();
                    xlWorkSheet.Cells[row, 8] = sheet;
                    xlWorkSheet.Cells[row, 9] = ownerView;
                    xlWorkSheet.Cells[row, 10] = elementId;

                    row++;
                }

                int lastColumn = 10; // Assuming you have 10 columns
                Excel.Range headerRange = xlWorkSheet.Range[xlWorkSheet.Cells[1, 1], xlWorkSheet.Cells[1, lastColumn]];
                headerRange.AutoFilter(1);

                xlWorkBook.SaveAs(saveFileDialog.FileName, Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook.Close(true);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                Process.Start(saveFileDialog.FileName);
            }
        }
    }
}
