using RevTools.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevTools
{
    /// <summary>
    /// Interaction logic for Control_Rule.xaml
    /// </summary>
    public partial class Control_Rule : UserControl
    {
        public event RoutedEventHandler RemoveClicked;
        public event KeyEventHandler CriteriaKeyUp;
        public event RoutedEventHandler FilterMethodChanged;

        public static readonly DependencyProperty SelectedFilterMethodProperty = DependencyProperty.Register("SelectedFilterMethod", typeof(string), typeof(Control_Rule), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty CriteriaTextProperty = DependencyProperty.Register("CriteriaText", typeof(string), typeof(Control_Rule), new PropertyMetadata(string.Empty));
        public string SelectedFilterMethod
        {
            get { return (string)GetValue(SelectedFilterMethodProperty); }
            set { SetValue(SelectedFilterMethodProperty, value); }
        }

        public string CriteriaText
        {
            get { return (string)GetValue(CriteriaTextProperty); }
            set { SetValue(CriteriaTextProperty, value); }
        }
        public Control_Rule()
        {
            InitializeComponent();
        }

        private void CB_FilterMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ComboBoxItem selectedItem)
            {
                SelectedFilterMethod = selectedItem.Content.ToString();
                FilterMethodChanged?.Invoke(this, new RoutedEventArgs());
            }
        }

        public FilterOptions SelectedFilterOption
        {
            get
            {
              
                switch (SelectedFilterMethod)
                {
                    case "Contains":
                        return FilterOptions.Contains;
                    case "Does Not Contain":
                        return FilterOptions.DoesNotContain;
                    case "Begins With":
                        return FilterOptions.BeginsWith;
                    case "Does Not Begin With":
                        return FilterOptions.DoesNotBeginWith;
                    case "Ends With":
                        return FilterOptions.EndsWith;
                    case "Does Not End With":
                        return FilterOptions.DoesNotEndWith;
                    default:
                        return FilterOptions.Contains;
                }
            }
        }

        public string CriteriaTextValue
        {
            get { return TB_Criteria.Text; }
        }

        private void TB_Criteria_KeyUp(object sender, KeyEventArgs e)
        {
            CriteriaKeyUp?.Invoke(this, e);
        }

        private void BTN_Remove_Click(object sender, RoutedEventArgs e)
        {
            RemoveClicked?.Invoke(this, e);
        }
    }
}
