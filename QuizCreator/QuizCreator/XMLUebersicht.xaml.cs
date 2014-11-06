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
using System.Xml;
using System.IO;

namespace QuizCreator
{
    /// <summary>
    /// Interaktionslogik für XMLUebersicht.xaml
    /// </summary>
    public partial class XMLUebersicht : Window
    {
        bool bearbeiten = false;

        public XMLUebersicht()
        {
            InitializeComponent();
            lb_kato.Items.Add("Soundquiz");
            lb_kato.Items.Add("Textquiz");            
        }

        public XMLUebersicht(string filename, bool bearbeiten2)
        {
            InitializeComponent();
            txt_name.Text = filename;
            bearbeiten = bearbeiten2;
            //in der xml ausgewählte kategorie wählen
            lb_kato.Items.Add("Soundquiz");
            lb_kato.Items.Add("Textquiz");           
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            int dauer = 10;

            try
            {
                dauer = Convert.ToInt32(txt_dauer.Text);
            }
            catch (Exception)
            {
                
               
            }
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/quizxml/"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/quizxml/"); 
            }

            if (File.Exists(Directory.GetCurrentDirectory() + "/quizxml/" + txt_name.Text + ".xml"))
            {
                if (!bearbeiten)
                {
                    MessageBox.Show("Ein Quiz mit diesem Namen existiert bereits! Bitte wählen Sie einen anderen Namen.");
                    return;  
                }                
            }
            else
            {
                XmlTextWriter myXmlTextWriter = new XmlTextWriter(Directory.GetCurrentDirectory() + "/quizxml/" + txt_name.Text + ".xml", System.Text.Encoding.UTF8);
                myXmlTextWriter.Formatting = Formatting.Indented;
                myXmlTextWriter.WriteStartDocument(false);

                myXmlTextWriter.WriteStartElement("Quiz");
                myXmlTextWriter.WriteElementString("quizname", txt_name.Text);
                myXmlTextWriter.WriteElementString("sounddatei", txt_name.Text + ".mp3");
                myXmlTextWriter.WriteElementString("clipdauer", dauer.ToString());                
                myXmlTextWriter.WriteElementString("quiztyp", lb_kato.SelectedValue.ToString());
                myXmlTextWriter.WriteStartElement("Items");
                myXmlTextWriter.WriteEndElement();
                myXmlTextWriter.WriteEndElement();
                myXmlTextWriter.Close();
                bearbeiten = true;
            }


            // index = 0 für sound
            if (lb_kato.SelectedIndex == 0)
            {
                MainWindow begriffe_window = new MainWindow(txt_name.Text);
                begriffe_window.Owner = this;
                begriffe_window.Show();
            }
            // index = 1 für text
            if (lb_kato.SelectedIndex == 1)
            {
                fragenquiz begriffe_window = new fragenquiz(txt_name.Text);
                begriffe_window.Owner = this;
                begriffe_window.Show();
            }
        }

        private void txt_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_name.Text.Length > 0)
            {
                button1.IsEnabled = true;
            }
            else
            {
                button1.IsEnabled = false;
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            button2.IsEnabled = true;
            button3.IsEnabled = true;
        }


        private void Window_Activated(object sender, EventArgs e)
        {
            int dauer = 10;
           
            if (bearbeiten && File.Exists(Directory.GetCurrentDirectory() + "/quizxml/" + txt_name.Text + ".xml"))
            {
                listBox1.Items.Clear();
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + txt_name.Text + ".xml");
                XmlNode node;
                node = myXmlDocument.DocumentElement;
                if (listBox1.SelectedItem == null)
                {
                    button2.IsEnabled = false;
                    button3.IsEnabled = false;
                }
                foreach (XmlNode node1 in node.ChildNodes)
                {
                    if (node1.Name == "clipdauer")
                    {
                        dauer = Convert.ToInt32(node1.InnerText);
                    }
                    if (node1.Name == "quiztyp")
                    {
                        lb_kato.SelectedItem = node1.InnerText;
                    }                  

                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        // index = 0 für sound
                        if (lb_kato.SelectedIndex == 0)
                        {
                            if (node2.Name == "Anzeigename")
                            {
                                listBox1.Items.Add(node2.InnerText);
                            }
                        }
                        // index = 1 für text
                        if (lb_kato.SelectedIndex == 1)
                        {
                            if (node2.Name == "frage")
                            {
                                listBox1.Items.Add(node2.InnerText);
                            }
                        }
                    }
              

                }                
            }
            lbl_anzahl.Content = listBox1.Items.Count.ToString();
            txt_dauer.Text = dauer.ToString();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + txt_name.Text + ".xml");

            XmlNode node;
            node = myXmlDocument.DocumentElement;


            foreach (XmlNode node1 in node.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    if (node2.Name == "Anzeigename")
                    {
                        if (node2.InnerText == listBox1.SelectedItem.ToString())
                        {
                            XmlNode node3;
                            node3 = node2.NextSibling;
                          
                            while (node3 != null )
                            {
                                if (node3.Name == "Alternativ")
                                {                                    
                                    node1.RemoveChild(node3);
                                    node3 = node2.NextSibling;
                                }
                                else
                                {
                                    break;
                                }                               
                            }                            
                            node1.RemoveChild(node2);
                        }                                                
                    }
                }
            }
            myXmlDocument.Save(Directory.GetCurrentDirectory() + "/quizxml/" + txt_name.Text + ".xml");

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // index = 0 für sound
            if (lb_kato.SelectedIndex == 0 && listBox1.SelectedItem != null)
            {
                MainWindow begriffe_window = new MainWindow(txt_name.Text, listBox1.SelectedItem.ToString());
                begriffe_window.Owner = this;
                begriffe_window.Show();
            }
            // index = 1 für text
            if (lb_kato.SelectedIndex == 1 && listBox1.SelectedItem != null)
            {
                fragenquiz begriffe_window = new fragenquiz(txt_name.Text, listBox1.SelectedItem.ToString());
                begriffe_window.Owner = this;
                begriffe_window.Show();
            }
        }
    }
}
