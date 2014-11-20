using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ThemeQuizWPF
{
    /// <summary>
    /// Interaktionslogik für Videoplayer.xaml
    /// </summary>
    public partial class Videoplayer : Window
    {
        public Videoplayer()
        {
            InitializeComponent();
        }
        public Videoplayer(int vidnumber,string vidURL)
        {
            InitializeComponent();

            Uri videoUrl = new Uri(vidURL);

            videoplayer.Source = videoUrl;

            BuildUI(vidnumber);
        }

        private void BuildUI(int vidnummer)
        {

        }
    }
}
