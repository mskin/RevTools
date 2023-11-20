using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using RevTools.Objects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RevTools
{
    public partial class WPF_Window : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool isolateClouds = false;
        public bool unHide = false;

        private UIApplication uiapp;
        private UIDocument uidoc;
        private Autodesk.Revit.ApplicationServices.Application app;
        private Document doc;

        public ObservableCollection<RevFilterRule> RevFilterRules { get; set; } = new ObservableCollection<RevFilterRule>();

        private ObservableCollection<ThisRevisionCloud> _filteredRevisionClouds = new ObservableCollection<ThisRevisionCloud>();
        public ObservableCollection<ThisRevisionCloud> FilteredRevisionClouds
        {
            get => _filteredRevisionClouds;
            set
            {
                _filteredRevisionClouds = value;
                OnPropertyChanged(nameof(FilteredRevisionClouds));
            }
        }

        private bool _isFilteringApplied = false;
        public bool IsFilteringApplied
        {
            get => _isFilteringApplied;
            set
            {
                _isFilteringApplied = value;
                OnPropertyChanged(nameof(IsFilteringApplied));
            }
        }

        private string _revFilterCriteria;
        public string RevFilterCriteria
        {
            get => _revFilterCriteria;
            set
            {
                _revFilterCriteria = value;
                OnPropertyChanged(nameof(RevFilterCriteria));
            }
        }

        private ThisRevisionClouds rcs;
        public WPF_Window(ExternalCommandData commandData)
        {
            InitializeComponent();
            DataContext = this;

            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;

            FilteredElementCollector CollectionOfRevisionsAsElement = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_RevisionClouds).OfClass(typeof(RevisionCloud));

            List<Revision> revisions = Utiilities.GetRevisions(doc);
            List<RevisionCloud> revisionsClouds = Utiilities.GetRevisionsClouds(doc);
            List<ViewSheet> sheetsWithRevClouds = Utiilities.GetSheetsWithRevisionClouds(CollectionOfRevisionsAsElement, doc);

            revisionDataGrid.ItemsSource = revisions;
        }
        /// <summary>
        /// this is called from the main list of revisions. It gets a list of revisionclouds that reside in that revision and sets it as the itemsource
        /// of  the RevisionCloudDatagrid.  It also establishes an observeable collection that will later be applied to 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DG_GetRevisionData(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Revision selectedRevision)
            {
                rcs = new ThisRevisionClouds(selectedRevision);
                IsFilteringApplied = true;
                FilteredRevisionClouds = FilterCloudsBasedOnCriteria(rcs);
                revisionCloudDataGrid.ItemsSource = rcs._All;
            }
        }


        private ObservableCollection<ThisRevisionCloud> FilterCloudsBasedOnCriteria(ThisRevisionClouds rcs)
        {
            ObservableCollection<ThisRevisionCloud> filteredClouds = new ObservableCollection<ThisRevisionCloud>();

            foreach (var cloud in rcs._All)
            {
                bool matchesAllRules = true;

                foreach (var rule in RevFilterRules)
                {
                    if (!MatchesFilterCriteria(cloud, rule.RevFilterOption, rule.RevFilterCriteria))
                    {
                        matchesAllRules = false;
                        break;
                    }
                }

                if (matchesAllRules)
                {
                    filteredClouds.Add(cloud);
                }
            }

            return filteredClouds;
        }


        private bool MatchesFilterCriteria(ThisRevisionCloud cloud, FilterOptions selectedCriteria, string filterCriteria)
        {
            if (filterCriteria == null || cloud.Comments == null)
            {
                return false;
            }

            switch (selectedCriteria)
            {
                case FilterOptions.Contains:
                    return cloud.Comments.Contains(filterCriteria);
                case FilterOptions.DoesNotContain:
                    return !(cloud.Comments.Contains(filterCriteria));
                case FilterOptions.BeginsWith:
                    return cloud.Comments.StartsWith(filterCriteria);
                case FilterOptions.DoesNotBeginWith:
                    return !(cloud.Comments.StartsWith(filterCriteria));
                case FilterOptions.EndsWith:
                    return cloud.Comments.EndsWith(filterCriteria);
                case FilterOptions.DoesNotEndWith:
                    return !(cloud.Comments.EndsWith(filterCriteria));
                default:
                    return true; // Default to true if no specific filtering logic is implemented
            }
        }

        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            RevFilterRule newFilterRule = new RevFilterRule();
            RevFilterRules.Add(newFilterRule);
            if (RevFilterRules.Count() > 0) {
                Border br = RulesBorder;
                br.Visibility = System.Windows.Visibility.Visible;
            }
            FilteredRevisionClouds = FilterCloudsBasedOnCriteria(rcs);
        }

        private void RemoveRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Control_Rule ruleControl && ruleControl.DataContext is RevFilterRule filterRule)
            {
                RevFilterRules.Remove(filterRule);

                if (RevFilterRules.Count() == 0)
                {
                    Border br = RulesBorder;
                    br.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        private void ControlRule_FilterMethodChanged(object sender, RoutedEventArgs e)
        {
            if (sender is Control_Rule ruleControl)
                if (sender is Control_Rule rc && rc.DataContext is RevFilterRule rule)
                {
                    rule.RevFilterOption = rc.SelectedFilterOption;
                    rule.RevFilterCriteria = rc.CriteriaTextValue;

                    ApplyFilter(rc);
                }
        }
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is Control_Rule rc && rc.DataContext is RevFilterRule rule)
            {
                rule.RevFilterOption = rc.SelectedFilterOption;
                rule.RevFilterCriteria = rc.CriteriaTextValue;

                ApplyFilter(rc);
            }
            
        }

        private void ApplyFilter(object sender)
        {
            if (sender is Control_Rule)
            {
                FilteredRevisionClouds = FilterCloudsBasedOnCriteria(rcs);
                FilteredrevisionCloudDataGrid.ItemsSource = FilteredRevisionClouds;
            }
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;
            T parentT = parent as T;
            return parentT ?? FindVisualParent<T>(parent);
        }
        private ComboBox FindVisualChild<ComboBox>(DependencyObject parent) where ComboBox : FrameworkElement
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is ComboBox comboBox)
                {
                    return comboBox; // Return the first ComboBox found
                }
                else
                {
                    var result = FindVisualChild<ComboBox>(child);
                    if (result != null)
                    {
                        return result; // Return the result if found
                    }
                }
            }
            return null; // Return null if no ComboBox is found in the tree
        }

        private void btn_SavreWordNarrative_Click(object sender, RoutedEventArgs e)
        {
            WordNarrativeGenerator.CreateWordDocument(FilteredRevisionClouds.ToList());
        }

        private void btn_Isolate_Click(object sender, RoutedEventArgs e)
        {
            isolateClouds = true;
            Close();
        }

        private void btn_UnhideRevisinons_Click(object sender, RoutedEventArgs e)
        {
            unHide = true;
            Close();
        }

        private void btn_SavreExcelNarrative_Click(object sender, RoutedEventArgs e)
        {
            ExcellGenerator.CreateExcell(doc);
        }
    }
}
