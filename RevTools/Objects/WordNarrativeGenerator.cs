using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevTools.Objects
{
    internal static class WordNarrativeGenerator
    {
        public static void CreateWordDocument(List<ThisRevisionCloud> revisionClouds)
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

                // Create a new Word document
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
                {
                    // Add a main document part
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();

                    // Define styles
                    StyleDefinitionsPart stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
                    Styles styles = new Styles();
                    stylePart.Styles = styles;

                    // Style 1 (Normal)
                    styles.AppendChild(CreateStyle("Style1", "Normal", "Calibri", "20", "auto", "auto", "auto"));

                    // Style 2 (Bold, Underlined, Red)
                    styles.AppendChild(CreateStyle("Style2", "BoldUnderlineRed", "Calibri", "20", "auto", "auto", "auto", true, true, "FF0000"));

                    // Style 3 (Black, Bold)
                    styles.AppendChild(CreateStyle("Style3", "BlackBold", "Calibri", "20", "auto", "auto", "auto", true));

                    // Create a body for the document
                    Body body = new Body();

                    // Add "Description:" in Style 3
                    Paragraph descriptionParagraph = new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "Style3" }
                        ),
                        new Run(
                            new Text("Description:")
                        )
                    );
                    body.Append(descriptionParagraph);

                    // Add "General Drawings:" in Style 2
                    Paragraph generalDrawingsParagraph = new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "Style2" }
                        ),
                        new Run(
                            new Text("General Drawings:")
                        )
                    );
                    body.Append(generalDrawingsParagraph);

                    // Add General Drawings (Prefix starting with 01) in Style 1
                    foreach (ThisRevisionCloud cloud in sortedClouds)
                    {
                        if (cloud.prefix.StartsWith("01"))
                        {
                            Paragraph generalDrawingParagraph = new Paragraph(
                                new ParagraphProperties(
                                    new ParagraphStyleId() { Val = "Style1" },
                                    new Tabs(
                                        new TabStop()
                                        {
                                            Val = TabStopValues.Left,
                                            Position = 1440 // 1 inch = 1440 twentieths of a point
                                        })
                                ),
                                new Run(
                                    new Text(cloud.SheetNumber),
                                    new TabChar(), // Add a TabChar element to represent a tab
                                    new Text(cloud.Comments)
                                )
                            );

                            // Add the paragraph to the body
                            body.Append(generalDrawingParagraph);
                        }
                    }

                    // Add "Architectural Drawings:" in Style 2
                    Paragraph architecturalDrawingsParagraph = new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "Style2" }
                        ),
                        new Run(
                            new Text("Architectural Drawings:")
                        )
                    );
                    body.Append(architecturalDrawingsParagraph);

                    // Add Architectural Drawings (All) in Style 1
                    foreach (ThisRevisionCloud cloud in sortedClouds)
                    {
                        if (!cloud.prefix.StartsWith("01"))
                        {

                       
                            Paragraph architecturalDrawingParagraph = new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId() { Val = "Style1" },
                                new Tabs(
                                    new TabStop()
                                    {
                                        Val = TabStopValues.Left,
                                        Position = 1440 // 1 inch = 1440 twentieths of a point
                                    })
                            ),
                            new Run(
                                new Text(cloud.SheetNumber),
                                new TabChar(), // Add a TabChar element to represent a tab
                                new Text(cloud.Comments)
                            )
                        );

                        // Add the paragraph to the body
                        body.Append(architecturalDrawingParagraph);
                        }
                    }

                    // Add the body to the main document part
                    mainPart.Document.Append(body);
                }

                // Open the saved document
                Process.Start(outputPath);
            }
        }

        private static Run CreateRun(string text, string styleId = null)
        {
            Run run = new Run();
            Text runText = new Text(text);
            run.Append(runText);
            if (!string.IsNullOrEmpty(styleId))
            {
                RunProperties runProperties = new RunProperties();
                runProperties.Append(new RunStyle() { Val = styleId });
                run.RunProperties = runProperties;
            }
            return run;
        }

        private static Style CreateStyle(string styleId, string styleName, string fontFamily, string fontSize, string marginLeft, string marginRight, string marginTop, bool isBold = false, bool isUnderlined = false, string fontColor = null)
        {
            Style style = new Style();
            style.StyleId = styleId;
            style.StyleName = new StyleName() { Val = styleName };

            StyleRunProperties styleRunProperties = new StyleRunProperties();
            styleRunProperties.Append(new RunFonts() { Ascii = "Arial" });
            styleRunProperties.Append(new FontSize() { Val = "22" });
            styleRunProperties.Append(new SpacingBetweenLines() { Before = "0", After = "0" });

            if (!string.IsNullOrEmpty(marginLeft))
                styleRunProperties.Append(new Indentation() { Left = marginLeft });
            if (!string.IsNullOrEmpty(marginRight))
                styleRunProperties.Append(new Indentation() { Right = marginRight });
            if (!string.IsNullOrEmpty(marginTop))
                styleRunProperties.Append(new SpacingBetweenLines() { LineRule = LineSpacingRuleValues.Auto, Before = marginTop });

            if (isBold)
                styleRunProperties.Append(new Bold());
            if (isUnderlined)
                styleRunProperties.Append(new Underline() { Val = UnderlineValues.Single });

            if (!string.IsNullOrEmpty(fontColor))
                styleRunProperties.Append(new Color() { Val = fontColor });

            style.Append(styleRunProperties);

            return style;
        }
    }
}
