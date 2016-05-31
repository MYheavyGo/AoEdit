using LearnDATAGRID.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LearnDATAGRID.IntelliSense
{
    public class FilterIntelliSense : TextBox
    {
        [DllImport("user32")]
        private extern static int GetCaretPos(out Point p);

        public string Regex { get; set; }
        public List<string> Properties { get; set; }
        public Filter Filter { get; set; }
        public Popup Popup { get; set; }
        public ListBox ListBox { get; set; }

        public FilterIntelliSense(string nameClass)
        {
            ListBox = new ListBox();
            Properties = new List<string>();
            Filter = new Filter(nameClass);

            Initialize();
        }

        private void Initialize()
        {
            VerticalContentAlignment = VerticalAlignment.Center;

            ListBox.Visibility = Visibility.Hidden;
            ListBox.KeyUp += ListBox_KeyUp;

            AddProperties();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter || e.Key == Key.Down)
            {
                if (ListBox.Visibility == Visibility.Visible)
                {
                    ListBox.Focus();
                }
                e.Handled = true;
            } else if (e.Key == Key.Escape)
            {

                ListBox.Visibility = Visibility.Hidden;
                e.Handled = true;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            Point cp;
            GetCaretPos(out cp);

            Console.WriteLine(cp.X + cp.Y);
        }

        private void ListBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddProperties()
        {
            PropertyInfo[] infos = Filter.Instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo item in infos)
            {
                Properties.Add(item.Name);
            }
        }
    }
}
