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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace QuizCreator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string xml_name, anzeigename;
        List<string> list_alternative = new List<string>();
        bool bearbeiten = false;

        public MainWindow(string xmlname)
        {
            InitializeComponent();
            xml_name = xmlname;
            txt_anzeigename.Focus();
        }
        public MainWindow(string xmlname, string selectednode)
        {
            InitializeComponent();
            xml_name = xmlname;
            anzeigename = selectednode;
            txt_anzeigename.Focus();

            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + xml_name + ".xml");

            XmlNode node;
            node = myXmlDocument.DocumentElement;

            foreach (XmlNode node1 in node.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    if (node2.Name == "Anzeigename")
                    {
                        if (node2.InnerText == anzeigename)
                        {
                            XmlNode node3;
                            node3 = node2.NextSibling;

                            while (node3 != null)
                            {
                                if (node3.Name == "Alternativ")
                                {
                                    list_alternative.Add(node3.InnerText);
                                    node3 = node3.NextSibling;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            txt_anzeigename.Text = anzeigename;
            list_alternativ.ItemsSource = list_alternative;
            bearbeiten = true;

        }

        private void txt_anzeigename_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_anzeigename.Text.Length > 0)
            {
                button3.IsEnabled = true;
            }
            else
            {
                button3.IsEnabled = false;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (!(txt_anzeigename.Text == ""))
            {
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + xml_name + ".xml");

                XmlNode node, newnode, node_alternativ;
                newnode = myXmlDocument.CreateNode(XmlNodeType.Element, "Anzeigename", null);

                node = myXmlDocument.DocumentElement;

                if (bearbeiten)
                {
                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name == "Anzeigename")
                            {
                                if (node2.InnerText == anzeigename)
                                {
                                    node2.InnerText = txt_anzeigename.Text;

                                    XmlNode node3;
                                    node3 = node2.NextSibling;

                                    while (node3 != null)
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

                                    for (int i = list_alternativ.Items.Count - 1; i > -1; i--)
                                    {
                                        node_alternativ = myXmlDocument.CreateNode(XmlNodeType.Element, "Alternativ", null);
                                        node_alternativ.InnerText = list_alternativ.Items[i].ToString();
                                        node1.InsertAfter(node_alternativ, node2);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        if (node1.Name == "Items")
                        {
                            newnode.InnerText = txt_anzeigename.Text;
                            node1.AppendChild(newnode);
                            for (int i = 1; i < list_alternativ.Items.Count + 1; i++)
                            {
                                node_alternativ = myXmlDocument.CreateNode(XmlNodeType.Element, "Alternativ", null);
                                node_alternativ.InnerText = list_alternativ.Items[i - 1].ToString();
                                node1.AppendChild(node_alternativ);
                            }
                        }
                    }
                }
                myXmlDocument.Save(Directory.GetCurrentDirectory() + "/quizxml/" + xml_name + ".xml");

                MainWindow NeuesFenster = new MainWindow(xml_name);
                NeuesFenster.Show();
                this.Close();
            }           
        }

        private void txt_alternativ_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_alternativ.Text.Length > 0)
            {
                button1.IsEnabled = true;
            }
            else
            {
                button1.IsEnabled = false;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!(txt_alternativ.Text == ""))
            {
                list_alternative.Add(txt_alternativ.Text);
                list_alternativ.ItemsSource = null;
                list_alternativ.ItemsSource = list_alternative;
                txt_alternativ.Text = "";
                txt_alternativ.Focus();  
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            list_alternative.Remove(list_alternativ.SelectedItem.ToString());
            list_alternativ.ItemsSource = null;
            list_alternativ.ItemsSource = list_alternative;
        }

        private void list_alternativ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            button2.IsEnabled = true;
        }

        private void txt_alternativ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button1_Click(null, null);
            }
        }

        private void txt_anzeigename_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button3_Click(null, null);
            }
        }

        private void list_alternativ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                button2_Click(null, null);
            }
        }
    }
}
