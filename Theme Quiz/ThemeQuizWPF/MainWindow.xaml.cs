﻿using System;
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
using System.Timers;
using System.IO;
using System.Xml;




namespace ThemeQuizWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBlock frage = new TextBlock();
        List<String> Stringlist = new List<string>();
        List<String> Fragen = new List<string>();        
        List<Label> Labellist = new List<Label>();
        List<TextBlock> textblocklist = new List<TextBlock>();
        List<string> geloeste = new List<string>();
        bool quizrunning = false, playing = false;
        int ClipTime = 0, fragenindex = 0;
        int countdowntimer = 0;
        string globalSound = "";
        string mode = "";

        public MainWindow()
        {
            InitializeComponent();
            //Hier neue Modi hinzufügen
            spielmodibox.Items.Add("Soundquiz");
            spielmodibox.Items.Add("Textquiz");
            //spielmodibox.Items.Add("Videoquiz");

            //Set Background
            DateTime Halloween1 = new DateTime(DateTime.Today.Year, 10, 31);
            DateTime Halloween2 = new DateTime(DateTime.Today.Year, 11, 1);

            if (DateTime.Today.Date == Halloween1 || DateTime.Today.Date == Halloween2)
            {
                TheGrid.Background.Opacity = 0.0;
                this.Background = new ImageBrush(new BitmapImage(new Uri("Images/halloween.jpg", UriKind.Relative)));
            }
            else
            {
                TheGrid.Background.Opacity = 0.0;
                this.Background = new ImageBrush(new BitmapImage(new Uri("Images/normal.jpg", UriKind.Relative)));
            }
            //Background Ende --- XML Directory Laden
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/quizxml/"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/quizxml/");
            }
        }

        void Countdown(int count, TimeSpan interval, Action<int> ts)
        {
            var dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = interval;
            dt.Tick += (_, a) =>
            {
                if (count-- == 0)
                    dt.Stop();
                else
                    ts(count);
            };
            ts(count);
            dt.Start();

        }

        int Countdown(int count, TimeSpan interval)
        {
            var dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = interval;
            dt.Tick += (_, a) =>
            {
                if (count-- == 0)
                    dt.Stop();
            };
            dt.Start();
            return count;
        }

        int erzeugespielfeld(List<string> anzahl)
        {
            string TextBlockName = "";
            int TopMargin = 20, LeftMargin = 12, counter = 0, columncounter = 0, setrow = 3, NameCounter = 0, durchlaeufe = 0;
            double maxwidth = 0;
            bool virgin = true;
            DateTime NeuZeit = new DateTime();
            DateTime AltZeit = new DateTime();
            if (ClipTime == 0)
            {
                ClipTime = 10;
            }

            vongeloest.Content = "/ " + anzahl.Count;

            //Für den Textquizmodus

            if (spielmodibox.SelectedItem.ToString() == "Textquiz")
            {
                Thickness frageMargin = new Thickness();
                frageMargin.Left = 6;
                GridLength length = new GridLength(26);
                Gridtextbutton.Height = length;
                Textpanel.Visibility = Visibility.Visible;
                frage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                frage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                frage.Foreground = Brushes.White;
                frage.FontSize = 30;
                frage.Name = "fragebox";
                frage.Text = "1. " + Fragen[0];
                frage.Margin = frageMargin;
                Grid.SetRow(frage, 2);
                TheGrid.Children.Add(frage);
            }

            //Erstellen der Label nach Anzahl der Quizangaben
            foreach (string quizelement in anzahl)
            {
                durchlaeufe++;
                NameCounter++;
                TextBlockName = "textBlock" + NameCounter;
                if (columncounter == 0 && !virgin)
                {
                    LeftMargin = 12;
                    virgin = true;
                }
                TopMargin = counter * 25;

                Label SetLabel = new Label();
                TextBlock SetTextBlock = new TextBlock();

                SetLabel.Foreground = Brushes.White;
                SetTextBlock.Foreground = Brushes.White;

                #region Zeit fuer die Label: Soundquiz
                if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
                {
                    if (AltZeit.Minute < 10)
                    {
                        SetLabel.Content = "0";
                    }
                    SetLabel.Content = SetLabel.Content + Convert.ToString(AltZeit.Minute);
                    if (AltZeit.Second < 10)
                    {
                        if (AltZeit.Second == 0)
                        {
                            SetLabel.Content = SetLabel.Content + ":" + Convert.ToString(AltZeit.Second) + "0";
                        }
                        else
                        {
                            SetLabel.Content = SetLabel.Content + ":0" + Convert.ToString(AltZeit.Second);
                        }
                    }
                    else
                    {
                        SetLabel.Content = SetLabel.Content + ":" + Convert.ToString(AltZeit.Second);
                    }

                    NeuZeit = NeuZeit.AddSeconds(ClipTime);
                    AltZeit = NeuZeit;
                    if (NeuZeit.Minute < 10)
                    {
                        SetLabel.Content = SetLabel.Content + " - 0" + Convert.ToString(NeuZeit.Minute);
                    }
                    else
                    {
                        SetLabel.Content = SetLabel.Content + " - " + Convert.ToString(NeuZeit.Minute);
                    }

                    if (NeuZeit.Second < 10)
                    {
                        if (NeuZeit.Second == 0)
                        {
                            SetLabel.Content = SetLabel.Content + ":" + Convert.ToString(NeuZeit.Second) + "0";
                        }
                        else
                        {
                            SetLabel.Content = SetLabel.Content + ":0" + Convert.ToString(NeuZeit.Second);
                        }

                    }
                    else
                    {
                        SetLabel.Content = SetLabel.Content + ":" + Convert.ToString(NeuZeit.Second);
                    }


                    if (anzahl.Count() == durchlaeufe)
                    {
                        if (SetLabel.Content.ToString().Contains("- "))
                        {
                            durchlaeufe = SetLabel.Content.ToString().LastIndexOf(" ");
                            audio_bis.Text = SetLabel.Content.ToString().Remove(0, durchlaeufe);
                        }
                    }
                }
                #endregion

                #region Zeit fuer die Label: Textquiz && Videos
                if (spielmodibox.SelectedItem.ToString() == "Textquiz" || spielmodibox.SelectedItem.ToString() == "Videoquiz")
                {
                    SetLabel.Content = durchlaeufe + ".";

                    if (anzahl.Count() == durchlaeufe)
                    {
                        if (SetLabel.Content.ToString().Contains("- "))
                        {
                            durchlaeufe = SetLabel.Content.ToString().LastIndexOf(" ");
                            audio_bis.Text = SetLabel.Content.ToString().Remove(0, durchlaeufe);
                        }
                    }
                }
                #endregion

                #region Fuer die Loesungen unten
                Thickness LabelMargin = new Thickness();
                LabelMargin.Top = TopMargin;
                LabelMargin.Left = LeftMargin;
                SetLabel.Margin = LabelMargin;
                SetLabel.Width = Double.NaN;
                SetLabel.Height = 24;
                SetLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                SetLabel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                SetLabel.MouseLeftButtonUp += HandleClick;
                SetLabel.MouseEnter += HandleMouseEnter;
                SetLabel.MouseLeave += HandleMouseLeave;
                Grid.SetRow(SetLabel, setrow);
                TheGrid.Children.Add(SetLabel);
                Labellist.Add(SetLabel);

                if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
                {
                    //Setzen von Werten beim TextBlock
                    LabelMargin.Top = TopMargin + 5;
                    LabelMargin.Left = LeftMargin + 90;
                    SetTextBlock.Name = TextBlockName;
                    SetTextBlock.Margin = LabelMargin;
                    SetTextBlock.Width = Double.NaN;
                    SetTextBlock.Height = 24;
                    SetTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    SetTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    SetTextBlock.Text = "";
                    Grid.SetRow(SetTextBlock, setrow);
                    TheGrid.Children.Add(SetTextBlock);

                    textblocklist.Add(SetTextBlock);
                }
                if (spielmodibox.SelectedItem.ToString() == "Textquiz")
                {
                    //Setzen von Werten beim TextBlock
                    LabelMargin.Top = TopMargin + 5;
                    LabelMargin.Left = LeftMargin + 30;
                    SetTextBlock.Name = TextBlockName;
                    SetTextBlock.Margin = LabelMargin;
                    SetTextBlock.Width = Double.NaN;
                    SetTextBlock.Height = 24;
                    SetTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    SetTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    SetTextBlock.Text = "";
                    Grid.SetRow(SetTextBlock, setrow);
                    TheGrid.Children.Add(SetTextBlock);

                    textblocklist.Add(SetTextBlock);
                }


                if (maxwidth < (quizelement.Length * 10))
                {
                    maxwidth = quizelement.Length * 10;
                }

                counter++;

                if (counter % 24 == 0)
                {
                    LeftMargin = Convert.ToInt32(Math.Round(LeftMargin + maxwidth + 20, 0));
                    counter = 0;
                    columncounter++;
                    maxwidth = 0;
                }
                if (columncounter == 5)
                {
                    setrow++;
                    columncounter = 0;
                    virgin = false;
                }
            }
                #endregion
            return ClipTime;
        }


        private void solution_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (quizrunning)
            {
                //Überprüfung der Eingabe auf verschiedene Fälle.
                string loesung = "";
                string solution_enter, changed_answer = "";
                int solutionpos = 0, geloest_intern, aktuellpos = 0;
                bool boolloesung = false;

                if (tb.Text == "0")
                {
                    solution.IsEnabled = false;
                    return;
                }
                geloest_intern = Convert.ToUInt16(geloest.Content);
                solution_enter = solution.Text;
                solution_enter = solution_enter.ToLower();
                if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
                {
                    aktuellpos = Convert.ToInt32(Math.Truncate(slider1.Value / ClipTime)) + 1;
                }
                if (spielmodibox.SelectedItem.ToString() == "Textquiz")
                {
                    aktuellpos = fragenindex + 1;
                }
               
                //XML nach alternativen überprüfen, Es wird keine Liste von Alternativen gebraucht, da es mit jeder Textänderung
                //durchlaufen wird.
                #region text
                if (!(QuizSelection.SelectedItem.GetType() == typeof(ListBoxItem)) && quizrunning && mode == "text")
                {
                    XmlDocument myXmlDocument = new XmlDocument();
                    myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + QuizSelection.SelectedItem.ToString() + ".xml");

                    XmlNode node;
                    node = myXmlDocument.DocumentElement;

                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name == "Alternativ")
                            {
                                if (node2.InnerText.ToLower() == solution_enter)
                                {
                                    XmlNode node3;
                                    node3 = node2.PreviousSibling;

                                    while (node3 != null)
                                    {
                                        if (node3.Name == "Alternativ")
                                        {
                                            node3 = node3.PreviousSibling;
                                        }
                                        else
                                        {
                                            loesung = node3.InnerText;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region sound
                if (!(QuizSelection.SelectedItem.GetType() == typeof(ListBoxItem)) && quizrunning && mode == "sound")
                {
                    XmlDocument myXmlDocument = new XmlDocument();
                    myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + QuizSelection.SelectedItem.ToString() + ".xml");

                    XmlNode node;
                    node = myXmlDocument.DocumentElement;

                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name == "Alternativ")
                            {
                                if (node2.InnerText.ToLower() == solution_enter)
                                {
                                    XmlNode node3;
                                    node3 = node2.PreviousSibling;

                                    while (node3 != null)
                                    {
                                        if (node3.Name == "Alternativ")
                                        {
                                            node3 = node3.PreviousSibling;
                                        }
                                        else
                                        {
                                            loesung = node3.InnerText;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                solutionpos = 0;
                // Der allgemeine Suchalgo. Hier Beispielsweise das Filtern von Der/Die/Das/The hinzufügen.
                // Hinzufügen nur das Richtige bis zum aktuellen Zeitpunkt einzugeben.
                foreach (string item in Stringlist)
                {
                    if (((item.ToLower() == loesung.ToLower() && !geloeste.Contains(item)) || (item.ToLower() == solution_enter && !geloeste.Contains(item))) && aktuellpos >= solutionpos + 1)
                    {
                        textblocklist[solutionpos].Text = Stringlist[solutionpos].ToString();
                        solution.Text = "";
                        geloest_intern++;                      
                        geloeste.Add(item);
                        if (aktuellpos == solutionpos + 1)
                        {
                            boolloesung = true;
                            fragenindex++;
                        }   
                        break;
                    }

                    if (item.ToLower().Contains("the "))
                    {
                        if (item.ToLower().IndexOf("the ") == 0)
                        {
                            changed_answer = item.ToLower().Remove(0, 4);
                        }
                    }

                    if (item.ToLower().Contains("der "))
                    {
                        if (item.ToLower().IndexOf("der ") == 0)
                        {
                            changed_answer = item.ToLower().Remove(0, 4);
                        }
                    }

                    if (item.ToLower().Contains("die "))
                    {
                        if (item.ToLower().IndexOf("die ") == 0)
                        {
                            changed_answer = item.ToLower().Remove(0, 4);
                        }
                    }

                    if (item.ToLower().Contains("das "))
                    {
                        if (item.ToLower().IndexOf("das ") == 0)
                        {
                            changed_answer = item.ToLower().Remove(0, 4);
                        }
                    }
                    if (changed_answer != "")
                    {
                        if (((changed_answer == loesung.ToLower() && !geloeste.Contains(item)) || (changed_answer == solution_enter && !geloeste.Contains(item))) && aktuellpos >= solutionpos + 1)
                        {
                            textblocklist[solutionpos].Text = Stringlist[solutionpos].ToString();
                            solution.Text = "";
                            geloest_intern++;                            
                            geloeste.Add(item);
                            if (aktuellpos == solutionpos + 1)
                            {
                                boolloesung = true;
                                fragenindex++;
                            }                            
                            break;
                        }
                    }

                   
                    solutionpos++;
                }
                if (mode == "text" && boolloesung)
                {
                    if (Fragen.Count > fragenindex)
                    {
                        frage.Text = (fragenindex + 1) + ". " + Fragen[fragenindex];
                    }
                }
                geloest.Content = geloest_intern.ToString();
            }
        }


        private void button4_Click(object sender, RoutedEventArgs e)
        {
            int solutionpos = 0;
            foreach (string item in Stringlist)
            {
                if (!geloeste.Contains(item))
                {
                    textblocklist[solutionpos].Foreground = Brushes.Yellow;
                    textblocklist[solutionpos].Text = Stringlist[solutionpos].ToString();
                }
                solutionpos++;
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        // Quizstart für unterschiedliche Modis
        // Jede Quizart braucht eine neue Methode
        private void QuizSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!quizrunning)
            {
                Stringlist.Clear();
                textblocklist.Clear();
                quizrunning = true;
                spielmodibox.IsEnabled = false;
                if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
                {
                    mode = "sound";
                    fromsoundxml(QuizSelection.SelectedItem.ToString());
                }

                if (spielmodibox.SelectedItem.ToString() == "Textquiz")
                {
                    mode = "text";
                    fromtextxml(QuizSelection.SelectedItem.ToString());
                }

                if (spielmodibox.SelectedItem.ToString() == "Videoquiz")
                {
                    mode = "video";
                    fromvideoxml(QuizSelection.SelectedItem.ToString());
                }
            }
        }
        // für videoquiz
        private void fromvideoxml(string xmlname)
        {
            string sounddatei = "";
            int countdowntimer = 0;            
            XmlDocument myXmlDocument = new XmlDocument();
            if (File.Exists(Directory.GetCurrentDirectory() + "/quizxml/" + xmlname + ".xml"))
            {
                myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + xmlname + ".xml");
                XmlNode node;
                node = myXmlDocument.DocumentElement;

                foreach (XmlNode node1 in node.ChildNodes)
                {                    
                    if (node1.Name == "clipdauer")
                    {
                        ClipTime = Convert.ToInt32(node1.InnerText);
                    }
                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        if (node2.Name == "frage")
                        {
                            Fragen.Add(node2.InnerText);
                        }
                        if (node2.Name == "Anzeigename")
                        {
                            Stringlist.Add(node2.InnerText);
                        }
                    }
                }

                countdowntimer = erzeugespielfeld(Stringlist);
                countdowntimer = ClipTime;
                countdowntimer = (Stringlist.Count * countdowntimer) * 2;
                solution.IsEnabled = true;
                if (sounddatei != "")
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + "/sound/" + sounddatei))
                    {
                        globalSound = sounddatei;
                        openSound();
                    }
                    else
                    {
                        if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
                        {
                            MessageBox.Show("Die Datei (" + sounddatei + ") konnte nicht gefunden werden.");
                        }
                    }
                }

                Countdown(countdowntimer, TimeSpan.FromSeconds(1), cur => tb.Text = cur.ToString());




                quizrunning = true;

            }
        }

        // für textquiz
        private void fromtextxml(string xmlname)
        {
            string sounddatei = "";
            int countdowntimer = 0;
            //int clipdauer = 10;
            XmlDocument myXmlDocument = new XmlDocument();
            if (File.Exists(Directory.GetCurrentDirectory() + "/quizxml/" + xmlname + ".xml"))
            {
                myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + xmlname + ".xml");
                XmlNode node;
                node = myXmlDocument.DocumentElement;

                foreach (XmlNode node1 in node.ChildNodes)
                {
                    if (node1.Name == "sounddatei")
                    {
                        sounddatei = node1.InnerText;
                    }
                    if (node1.Name == "clipdauer")
                    {
                        ClipTime = Convert.ToInt32(node1.InnerText);
                    }
                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        if (node2.Name == "frage")
                        {
                            Fragen.Add(node2.InnerText);
                        }
                        if (node2.Name == "Anzeigename")
                        {
                            Stringlist.Add(node2.InnerText);
                        }
                    }
                }

                countdowntimer = erzeugespielfeld(Stringlist);
                countdowntimer = ClipTime;
                countdowntimer = (Stringlist.Count * countdowntimer) * 2;
                solution.IsEnabled = true;
                if (sounddatei != "")
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + "/sound/" + sounddatei))
                    {
                        globalSound = sounddatei;
                        openSound();
                    }
                    else
                    {
                        if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
                        {
                            MessageBox.Show("Die Datei (" + sounddatei + ") konnte nicht gefunden werden.");
                        }
                    }
                }

                Countdown(countdowntimer, TimeSpan.FromSeconds(1), cur => tb.Text = cur.ToString());




                quizrunning = true;

            }
        }
        // für soundquiz
        private void fromsoundxml(string xmlname)
        {
            string sounddatei = "";
            XmlDocument myXmlDocument = new XmlDocument();
            if (File.Exists(Directory.GetCurrentDirectory() + "/quizxml/" + xmlname + ".xml"))
            {
                myXmlDocument.Load(Directory.GetCurrentDirectory() + "/quizxml/" + xmlname + ".xml");

                XmlNode node;
                node = myXmlDocument.DocumentElement;

                foreach (XmlNode node1 in node.ChildNodes)
                {
                    if (node1.Name == "sounddatei")
                    {
                        sounddatei = node1.InnerText;
                    }
                    if (node1.Name == "clipdauer")
                    {
                        ClipTime = Convert.ToInt32(node1.InnerText);
                    }
                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        if (node2.Name == "Anzeigename")
                        {
                            Stringlist.Add(node2.InnerText);
                        }
                    }
                }

                countdowntimer = erzeugespielfeld(Stringlist);
                countdowntimer = ClipTime;
                countdowntimer = (Stringlist.Count * countdowntimer) * 2;
                solution.IsEnabled = true;

                if (File.Exists(Directory.GetCurrentDirectory() + "/sound/" + sounddatei))
                {
                    globalSound = sounddatei;
                    openSound();
                }
                else
                {
                    if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
                    {
                        MessageBox.Show("Die Datei (" + sounddatei + ") konnte nicht gefunden werden.");
                    }
                }

                Countdown(countdowntimer, TimeSpan.FromSeconds(1), cur => tb.Text = cur.ToString());
                quizrunning = true;
            }
        }

        void openSound()
        {
            if (globalSound.Contains(".mp3"))
            {
                globalSound = globalSound.Replace(".mp3", "");
            }

            mediaElement1.Source = new Uri(Environment.CurrentDirectory + "/sound/" + globalSound + ".mp3", UriKind.RelativeOrAbsolute);
            MediaPlayer m = new MediaPlayer();

            mediaElement1.Volume = 1;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            btn_playppause_Click(null, null);
        }

        private void btn_playppause_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            if (quizrunning)
            {
                if (playing)
                {
                    mediaElement1.Pause();
                    bi3.BeginInit();
                    bi3.UriSource = new Uri("Images/player_play.png", UriKind.Relative);
                    bi3.EndInit();
                    button_image.Source = bi3;
                    playing = false;
                }
                else
                {
                    mediaElement1.Play();

                    bi3.BeginInit();
                    bi3.UriSource = new Uri("Images/player_pause.png", UriKind.Relative);
                    bi3.EndInit();
                    button_image.Source = bi3;
                    playing = true;
                }
            }

        }
        void timer_Tick(object sender, EventArgs e)
        {
            slider1.Value = mediaElement1.Position.TotalSeconds;
        }

        private void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement1.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(mediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds);
                slider1.Maximum = ts.TotalSeconds;
            }
            volume_slider.Value = mediaElement1.Volume;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int indexof = 0, seconds = 0;
            string labeltextanfang = "", labeltextende = "";


            TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);
            mediaElement1.Position = ts;

            if (mediaElement1.Position.Minutes < 10)
            {
                audio_von.Text = "0";
            }
            else
            {
                audio_von.Text = "";
            }
            if (mediaElement1.Position.Seconds < 10)
            {
                audio_von.Text = audio_von.Text + mediaElement1.Position.Minutes.ToString() + ":0" + mediaElement1.Position.Seconds.ToString();
            }
            else
            {
                audio_von.Text = audio_von.Text + mediaElement1.Position.Minutes.ToString() + ":" + mediaElement1.Position.Seconds.ToString();
            }


            //Label soll verdeutlichen, dass die Zeit sich gerade an dieser Stelle befindet
            //Funktioniert aber finde ich nicht performant, da es jede Sekunde durchlaufen wird.
            if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
            {
                foreach (Label item in Labellist)
                {
                    labeltextanfang = item.Content.ToString();
                    labeltextende = item.Content.ToString();

                    indexof = labeltextanfang.IndexOf("-");
                    labeltextanfang = labeltextanfang.Remove(indexof - 1, labeltextanfang.Length - indexof + 1);
                    labeltextende = labeltextende.Remove(0, labeltextende.Length - indexof + 1);
                    indexof = labeltextanfang.IndexOf(":");

                    seconds = Convert.ToInt32(labeltextanfang.Remove(indexof, 3)) * 60;
                    seconds = seconds + (Convert.ToInt32(labeltextanfang.Remove(0, indexof + 1)));
                    TimeSpan ts2 = TimeSpan.FromSeconds(seconds);

                    seconds = Convert.ToInt32(labeltextende.Remove(2, 3)) * 60;
                    seconds = seconds + (Convert.ToInt32(labeltextende.Remove(0, 3)));

                    TimeSpan ts3 = TimeSpan.FromSeconds(seconds);

                    if (mediaElement1.Position >= ts2 && mediaElement1.Position <= ts3)
                    {
                        item.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        item.FontWeight = FontWeights.Normal;
                    }
                }
            }
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();

            mediaElement1.Stop();

            bi3.BeginInit();
            bi3.UriSource = new Uri("Images/player_play.png", UriKind.Relative);
            bi3.EndInit();
            button_image.Source = bi3;
            playing = false;
        }

        private void HandleClick(object sender, EventArgs e)
        {
            // Abspielen des ausgewähltem Videoclips
            if (spielmodibox.SelectedItem.ToString() == "Videoquiz")
            {
                int indexof = 0;
                Label clicked_label = new Label();
                clicked_label = sender as Label;
                string label = clicked_label.Content.ToString();
                indexof = label.IndexOf(".");
                label = label.Remove(indexof);
                fragenindex = Convert.ToInt32(label) - 1;
                Videoplayer vid = new Videoplayer(fragenindex, Directory.GetCurrentDirectory() + "/videos/" + Stringlist[fragenindex] + ".avi");                             
            }

            if (spielmodibox.SelectedItem.ToString() == "Soundquiz")
            {
                //Springen zu der Zeit vom angeklicktem Label
                int indexof = 0, seconds = 0;
                Label clicked_label = new Label();
                clicked_label = sender as Label;
                string label = clicked_label.Content.ToString();
                indexof = label.IndexOf("-");
                label = label.Remove(indexof - 1, label.Length - indexof + 1);
                indexof = label.IndexOf(":");

                seconds = Convert.ToInt32(label.Remove(indexof, 3)) * 60;
                seconds = seconds + (Convert.ToInt32(label.Remove(0, indexof + 1)));
                TimeSpan ts = TimeSpan.FromSeconds(seconds);
                mediaElement1.Position = ts;
            }
            if (spielmodibox.SelectedItem.ToString() == "Textquiz")
            {
                //Springen zur angeklicktem Frage
                int indexof = 0;
                Label clicked_label = new Label();
                clicked_label = sender as Label;
                string label = clicked_label.Content.ToString();
                indexof = label.IndexOf(".");
                label = label.Remove(indexof);

                fragenindex = Convert.ToInt32(label) - 1;
                frage.Text = (fragenindex + 1) + ". " + Fragen[fragenindex];
            }

        }
        private void HandleMouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            Label clicked_label = new Label();
            clicked_label = sender as Label;
            clicked_label.BorderThickness = new Thickness(0, 0, 0, 1);
            clicked_label.BorderBrush = Brushes.White;
        }
        private void HandleMouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
            Label clicked_label = new Label();
            clicked_label = sender as Label;
            clicked_label.BorderThickness = new Thickness(0, 0, 0, 0);
            clicked_label.BorderBrush = Brushes.White;
        }

        private void volume_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement1.Volume = e.NewValue;
        }

        private void volume_slider_LostFocus(object sender, RoutedEventArgs e)
        {
            volume_expender.IsExpanded = false;
        }

        private void spielmodibox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string quizname;
            List<string> stringlist = new List<string>();

            QuizSelection.Items.Clear();


            stringlist.AddRange(Directory.GetFiles(Directory.GetCurrentDirectory() + "/quizxml/"));
            //test
            XmlDocument myXmlDocument = new XmlDocument();

            //test ende
            foreach (string path in stringlist)
            {
                myXmlDocument.Load(path);

                XmlNode node;
                node = myXmlDocument.DocumentElement;
                foreach (XmlNode node1 in node.ChildNodes)
                {
                    if (node1.Name == "quiztyp")
                    {
                        if (node1.InnerText == spielmodibox.Items[spielmodibox.SelectedIndex].ToString())
                        {
                            quizname = path.Replace(Directory.GetCurrentDirectory() + "/quizxml/", "");
                            quizname = quizname.Replace(".xml", "");
                            QuizSelection.Items.Add(quizname);
                        }
                    }
                }
            }
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            if (fragenindex < Fragen.Count - 1)
            {
                fragenindex = fragenindex + 1;
                frage.Text = (fragenindex + 1) + ". " + Fragen[fragenindex];
            }

        }

        private void btn_previous_Click(object sender, RoutedEventArgs e)
        {
            if (fragenindex > 0)
            {
                fragenindex = fragenindex - 1;
                frage.Text = (fragenindex + 1) + ". " + Fragen[fragenindex];
            }
        }
    }
}

