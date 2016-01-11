﻿using System;
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

namespace AoEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        File fileWAV = new File();

        public MainWindow()
        {
            InitializeComponent();

            var buffer = fileWAV.OpenFile("./Ressources/WAV/good_bad_ugly.wav");
            txtBoxLog.Text = Encoding.UTF8.GetString(buffer);
        }
    }
}
