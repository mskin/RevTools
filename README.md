# RevTools

We are trying to track changes to the Revit Model as a result of RFI responses, PRs, ASIs, etc...  
We are exploring the use of revisions for this.  What we dont want is a new revision for every RFI as our model would contain 1000s of revisions.  Same for PRs, CCDs, etc...
Problem: there us no good, internal way to schedule and filter revisions within revit.

The idea is to have a Revision for RFIs and use the Comments to designate which RFI the cloud is for.  This can be filtered out and exported to a list, or hidden on sheets.

This tools lists the Revisions in the model. 
When a Revision is double Clicked, it populated the "Sheets with this revision" with all the Revision Clouds in that revision.
"Filter By Comments"... im modeling this after the "Filters" tool in revit, but this lets you filter the revisions and populates the "Filered Revisions" datagrid.

With that filtered list, you can...
1. Export the Revisions to (my companies standard format for narratives) a word file.
2. Isolate all those revisions in the project, by hiding all the others that arent in this list
3. Unhide all revisions that are currently hidden.

To test it out, you should be able to copy the .dll from the main directory and the .addin file from the RevTools directory into your Addins folder for Revit 2023.  This particular DLL was compiled in 2023.

![IM1](https://github.com/mskin/RevTools/assets/10675562/96867478-fc06-46f3-8825-bbd7b20bc25a)
