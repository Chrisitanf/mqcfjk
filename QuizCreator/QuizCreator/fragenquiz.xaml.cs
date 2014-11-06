using System;
using System.Collections.Generic;
using System.IO;
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

namespace QuizCreator
{
    /// <summary>
    /// Interaktionslogik für fragenquiz.xaml
    /// </summary>
    public partial class fragenquiz : Window
    {
        string xml_name, anzeigename;
        List<string> list_alternative = new List<string>();
        bool bearbeiten = false;

        public fragenquiz(string xmlname)
        {
            InitializeComponent();
            xml_name = xmlname;
            txt_frage.Focus();
        }

        public fragenquiz(string xmlname, string selectednode)
        {
            InitializeComponent();
            xml_name = xmlname;
            anzeigename = selectednode;
            txt_frage.Focus();

            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + xml_name + ".xml");

            XmlNode node;
            node = myXmlDocument.DocumentElement;

            foreach (XmlNode node1 in node.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    if (node2.Name == "frage")
                    {
                        if (node2.InnerText == anzeigename)
                        {
                            txt_frage.Text = node2.InnerText;
                            XmlNode node3;
                            node3 = node2.NextSibling;

                            if (node3.Name == "Anzeigename")
                            {
                                txt_loeseung.Text = node3.InnerText;
                                XmlNode node4;
                                node4 = node3.NextSibling;

                                while (node4 != null)
                                {
                                    if (node4.Name == "Alternativ")
                                    {
                                        list_alternative.Add(node4.InnerText);
                                        node4 = node4.NextSibling;
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
            }
            lb_loesungen.ItemsSource = list_alternative;
            bearbeiten = true;
        }

        private void txt_loeseung_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_loeseung.Text.Length > 0 && txt_frage.Text.Length > 0)
            {
                btn_save.IsEnabled = true;
            }
            else
            {
                btn_save.IsEnabled = false;
            }
        }
        //hinzufügen
        private void btn_hin_Click(object sender, RoutedEventArgs e)
        {
            if (!(txt_alternative.Text == ""))
            {
                list_alternative.Add(txt_alternative.Text);
                lb_loesungen.ItemsSource = null;
                lb_loesungen.ItemsSource = list_alternative;
                txt_alternative.Text = "";
                txt_alternative.Focus();
            }
        }
        //löschen
        private void btn_del_Click(object sender, RoutedEventArgs e)
        {
            list_alternative.Remove(lb_loesungen.SelectedItem.ToString());
            lb_loesungen.ItemsSource = null;
            lb_loesungen.ItemsSource = list_alternative;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!(txt_frage.Text == "" && txt_loeseung.Text == ""))
            {
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + xml_name + ".xml");

                XmlNode node, newnode, node_frage, node_alternativ;
                newnode = myXmlDocument.CreateNode(XmlNodeType.Element, "Anzeigename", null);
                node_frage = myXmlDocument.CreateNode(XmlNodeType.Element, "frage", null);

                node = myXmlDocument.DocumentElement;

                if (bearbeiten)
                {
                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {                           
                            if (node2.Name == "frage")
                            {
                                if (node2.InnerText == anzeigename)
                                {
                                    node2.InnerText = txt_frage.Text;

                                    XmlNode node3;
                                    node3 = node2.NextSibling;

                                    if (node3 != null)
                                    {
                                        if (node3.Name == "Anzeigename")
                                        {
                                            node3.InnerText = txt_loeseung.Text;

                                            XmlNode node4;
                                            node4 = node3.NextSibling;

                                            while (node4 != null)
                                            {
                                                if (node4.Name == "Alternativ")
                                                {
                                                    node1.RemoveChild(node4);
                                                    node4 = node3.NextSibling;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }                                   

                                    for (int i = lb_loesungen.Items.Count - 1; i > -1; i--)
                                    {
                                        node_alternativ = myXmlDocument.CreateNode(XmlNodeType.Element, "Alternativ", null);
                                        node_alternativ.InnerText = lb_loesungen.Items[i].ToString();
                                        node1.InsertAfter(node_alternativ, node3);
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
                            node_frage.InnerText = txt_frage.Text;
                            newnode.InnerText = txt_loeseung.Text;
                            node1.AppendChild(node_frage);
                            node1.AppendChild(newnode);
                            for (int i = 1; i < lb_loesungen.Items.Count + 1; i++)
                            {
                                node_alternativ = myXmlDocument.CreateNode(XmlNodeType.Element, "Alternativ", null);
                                node_alternativ.InnerText = lb_loesungen.Items[i - 1].ToString();
                                node1.AppendChild(node_alternativ);
                            }
                        }
                    }
                }
                myXmlDocument.Save(Directory.GetCurrentDirectory() + "/quizxml/" + xml_name + ".xml");

                fragenquiz NeuesFenster = new fragenquiz(xml_name);
                NeuesFenster.Show();
                this.Close();
            }           
        }

        private void txt_frage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_loeseung.Text.Length > 0 && txt_frage.Text.Length > 0)
            {
                btn_save.IsEnabled = true;
            }
            else
            {
                btn_save.IsEnabled = false;
            }
        }

        private void txt_alternative_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_hin_Click(null, null);
            }
        }
        private void txt_anzeigename_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_save_Click(null, null);
            }
        }

        private void lb_loesungen_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                btn_del_Click(null, null);
            }
        }

        private void lb_loesungen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_del.IsEnabled = true;
        }
    }
}
