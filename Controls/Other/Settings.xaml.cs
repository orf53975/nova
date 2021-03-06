﻿using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ssi
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Certainty.Text = Properties.Settings.Default.UncertaintyLevel.ToString();
            Annotator.Text = Properties.Settings.Default.Annotator;
            DefaultZoom.Text = Properties.Settings.Default.DefaultZoomInSeconds.ToString();
            Segmentmindur.Text = Properties.Settings.Default.DefaultMinSegmentSize.ToString();
            Samplerate.Text = Properties.Settings.Default.DefaultDiscreteSampleRate.ToString();
            DrawwaveformCheckbox.IsChecked = Properties.Settings.Default.DrawVideoWavform;
            ContinuousHotkeysnum.Text = Properties.Settings.Default.ContinuousHotkeysNumber.ToString();
            string[] tokens = Properties.Settings.Default.DatabaseAddress.Split(':');
            if (tokens.Length == 2)
            {
                DBHost.Text = tokens[0];
                DBPort.Text = tokens[1];
            }


            string[] history =  Properties.Settings.Default.serverhistory.Split(';');

            DBHost.DropItems = history;
            DBHost.DropdownClosed += split;

        

            DBUser.Text = Properties.Settings.Default.MongoDBUser;
            DBPassword.Password = MainHandler.Decode(Properties.Settings.Default.MongoDBPass);
            DBConnnect.IsChecked = Properties.Settings.Default.DatabaseAutoLogin;
            UpdatesCheckbox.IsChecked = Properties.Settings.Default.CheckUpdateOnStart;
            OverwriteAnnotation.IsChecked = Properties.Settings.Default.DatabaseAskBeforeOverwrite;
            DownloadDirectory.Text = Properties.Settings.Default.DatabaseDirectory;
            CMLDirectory.Text = Properties.Settings.Default.CMLDirectory;

        }


        private  void split(object sender, RoutedEventArgs e)
        {
            string[] tokens = DBHost.Text.Split(':');
            if (tokens.Length == 2)
            {
                DBHost.Text = tokens[0];
                DBPort.Text = tokens[1];
            }
        }

        public double Uncertainty()
        {
            return double.Parse(Certainty.Text);
        }

        public string AnnotatorName()
        {
            return Annotator.Text;
        }

        public string ZoomInseconds()
        {
            return DefaultZoom.Text;
        }

        public string SegmentMinDur()
        {
            return Segmentmindur.Text;
        }

        public string DatabaseAddress()
        {
            return DBHost.Text + ":" + DBPort.Text;
        }

        public string MongoUser()
        {
            return DBUser.Text;
        }

        public string MongoPass()
        {
            return DBPassword.Password;
        }

        public string SampleRate()
        {
            return Samplerate.Text;
        }

        public bool DrawvideoWavform()
        {
            return (DrawwaveformCheckbox.IsChecked == true);
        }

        public string ContinuousHotkeyLevels()
        {
            return ContinuousHotkeysnum.Text;
        }

        public bool CheckforUpdatesonStartup()
        {
            return (UpdatesCheckbox.IsChecked == true);
        }

        public bool DBAutoConnect()
        {
            return (DBConnnect.IsChecked == true);
        }

        public bool DBAskforOverwrite()
        {
            return (OverwriteAnnotation.IsChecked == true);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void IntNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$|^[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void IntNumberValidationTextBoxContinuous(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[2-9]$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {

           

            if(DBHost.Text != "" && DBHost.Text != "")
            {

                if(Properties.Settings.Default.serverhistory == "") Properties.Settings.Default.serverhistory = DBHost.Text + ":" + DBPort.Text;
                else if(!Properties.Settings.Default.serverhistory.Contains(DBHost.Text + ":" + DBPort.Text))
                {
                    Properties.Settings.Default.serverhistory = Properties.Settings.Default.serverhistory + ";" + DBHost.Text + ":" + DBPort.Text;
                }

                   
          
            
            if (Properties.Settings.Default.DatabaseDirectory != DownloadDirectory.Text)                
            {
                if (Directory.Exists(DownloadDirectory.Text))
                {
                    Directory.CreateDirectory(DownloadDirectory.Text);
                    Properties.Settings.Default.DatabaseDirectory = DownloadDirectory.Text;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageTools.Warning("Directory does not exist '" + DownloadDirectory.Text + "'");
                    return;
                }
            }


            if (Properties.Settings.Default.CMLDirectory != CMLDirectory.Text)
            {
                if (Directory.Exists(CMLDirectory.Text))
                {
                    Directory.CreateDirectory(CMLDirectory.Text);
                    Properties.Settings.Default.CMLDirectory = CMLDirectory.Text;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageTools.Warning("Directory does not exist '" + CMLDirectory.Text + "'");
                    return;
                }
            }

            DialogResult = true;
            Close();

            }

            else
            {
                MessageBox.Show("Host and IP can't be empty!");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void PickDownloadDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = Properties.Settings.Default.DatabaseDirectory;
            dialog.ShowNewFolderButton = true;
            dialog.Description = "Select the folder where you want to store the media of your databases in.";
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;

            try
            {
                dialog.SelectedPath = Properties.Settings.Default.DatabaseDirectory;
                result = dialog.ShowDialog();

            }

            catch
            {
                dialog.SelectedPath = "";
                result = dialog.ShowDialog();
            }



            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DownloadDirectory.Text = dialog.SelectedPath;
            }
        }

        private void ViewDownloadDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(DownloadDirectory.Text))
            {
                Directory.CreateDirectory(DownloadDirectory.Text);
                Process.Start(DownloadDirectory.Text);
            }
        }

        private void PickCMLDirectory_Click(object sender, RoutedEventArgs e)
        {
            
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
               
                dialog.ShowNewFolderButton = true;
                dialog.Description = "Select the folder where the Cooperative machine learning tools are stored.";
                System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;
            try
            {
                dialog.SelectedPath = Properties.Settings.Default.CMLDirectory;
                result = dialog.ShowDialog();
               
            }

            catch
            {
                dialog.SelectedPath = "";
                result = dialog.ShowDialog();
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                CMLDirectory.Text = dialog.SelectedPath;
            }

        }


        private void ViewCMLDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(CMLDirectory.Text))
            {
                Directory.CreateDirectory(CMLDirectory.Text);
                Process.Start(CMLDirectory.Text);
            }
        }

        private void DBUser_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DBUser.SelectAll();
        }

        private void DBPassword_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DBPassword.SelectAll();
        }

        private void DBHost_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(DBHost.Text == "") DBHost.IsDropdownOpened = true;
           // DBHost.SelectAll();
        }

        private void DBPort_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DBPort.SelectAll();
        }

        private void DBHost_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (DBHost.Text == "") DBHost.IsDropdownOpened = true;
        }

        private void DBPort_GotMouseCapture(object sender, MouseEventArgs e)
        {
            DBPort.SelectAll();
        }

        private void DBUser_GotMouseCapture(object sender, MouseEventArgs e)
        {
            DBUser.SelectAll();
        }

        private void DBPassword_GotMouseCapture(object sender, MouseEventArgs e)
        {
            DBPassword.SelectAll();
        }

        private void drpAge_TextChanged(object sender, RoutedEventArgs e)
        {
            //if (tbAge != null)
            //{
            //    tbAge.Text = DBHost.Text;
            //}
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.serverhistory = "";

            DBHost.DropItems = null;

        }
    }
}