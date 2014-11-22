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
using System.Xml;
using System.IO;

namespace QuizCreator
{
    /// <summary>
    /// Interaktionslogik für uebersicht.xaml
    /// </summary>
    public partial class uebersicht : Window
    {
        public uebersicht()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            XMLUebersicht new_window = new XMLUebersicht();
            new_window.Owner = this;
            new_window.Show();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            string quizname;
            List<string> stringlist = new List<string>();
            if (Directory.Exists(Directory.GetCurrentDirectory() + "/quizxml/"))
            {
                listBox1.ItemsSource = Directory.GetFiles(Directory.GetCurrentDirectory() + "/quizxml/");
                foreach (string path in listBox1.Items)
                {
                    quizname = path.Replace(Directory.GetCurrentDirectory() + "/quizxml/", "");
                    quizname = quizname.Replace(".xml", "");
                    stringlist.Add(quizname);
                }
            }
            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/quizxml/"); 
            }
           listBox1.ItemsSource = stringlist;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                XMLUebersicht new_window = new XMLUebersicht(listBox1.SelectedItem.ToString(), true);
                new_window.Owner = this;
                new_window.Show();
            }
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string selected = "";
            if (listBox1.SelectedItem != null)
            {
                selected = listBox1.SelectedItem.ToString();
                MessageBoxResult result = MessageBox.Show("Sind Sie sicher, dass Sie dieses Quiz löschen möchten?", "Achtung!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    File.Delete(Directory.GetCurrentDirectory() + "/quizxml/" + selected + ".xml");
                    Window_Activated(null, null);
                }
                
            }
        }
    }
}
