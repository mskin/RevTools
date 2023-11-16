using Autodesk.Revit.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevTools.Objects
{
    internal static class ExcellGenerator
    {
        internal static void CreateExcell(List<ThisRevisionCloud> revisionClouds)
        {
            // Sort the revisionClouds by Prefix property as numeric values
            List<ThisRevisionCloud> sortedClouds = revisionClouds.OrderBy(rc => int.Parse(rc.prefix)).ToList();

            // Create a SaveFileDialog to let the user choose the output path
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word Documents (*.docx)|*.docx";
            saveFileDialog.FileName = "Revisions.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string outputPath = saveFileDialog.FileName;
            }
        }
    }
}
