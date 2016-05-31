using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using LearnDATAGRID.IntelliSense;
using System;

namespace LearnDATAGRID
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Countries Countries { get; set; }
        public Countries tmpCountries { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            InitializeDATA();
        }

        private void InitializeDATA()
        {
            Countries = new Countries();
            tmpCountries = Countries;
            DataContext = tmpCountries;

            FilterIntelliSense txtBoxFilter = new FilterIntelliSense("LearnDATAGRID.Country");
            gridFilter.Children.Add(txtBoxFilter);
        }

        private void Add_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Country c = new Country();
            if (c.ID < Countries.Count)
                Countries.Insert(c.ID, c);
            else
                Countries.Add(c);
        }

        private void Del_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Countries.Count < 1)
                return;

            if (DataGridContent.SelectedItems.Count > 0)
                for (int i = DataGridContent.SelectedItems.Count; i > 0; i--)
                    Countries.Remove((Country)DataGridContent.SelectedItems[0]);
            else
                Countries.Remove(Countries.Last());
        }
    }
}
