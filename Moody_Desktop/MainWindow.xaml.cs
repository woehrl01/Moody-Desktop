using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Moody;
using Newtonsoft.Json;

namespace Moody_Desktop
{
    public partial class MainWindow : Window
    {
        private List<Loc> _locationList;
        private List<string> _dropdownData = null; 

        public MainWindow()
        {
            InitializeComponent();
            button.Click += delegate(object sender, RoutedEventArgs args)
            {
                Vote vote = new Vote();
                vote.Show();
                this.Close();
            };
        }

        private void GetLocations(string address)
        {
            DownloadLocations(address);
            if (_dropdownData != null)
            {
                comboBox.Items.Clear();
                foreach (var item in _dropdownData)
                {
                    comboBox.Items.Add(item);
                }
                comboBox.SelectedItem = _dropdownData.ToArray()[0];

                button.IsEnabled = true;
            }
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && textBox.Text != null)
            {
                GetLocations(textBox.Text);
            }
        }

        private void DownloadLocations(string address)
        {
            try
            {
                WebRequest request = WebRequest.Create("http://" + address + "/api/locations");
                request.Method = "GET";
                request.Timeout = 10000;
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                reader.Close();
                response.Close();

                _locationList = JsonConvert.DeserializeObject<List<Loc>>(content);
                List<string> locs = new List<string>();

                foreach (Loc l in _locationList)
                {
                    locs.Add(l.Location);
                }
                _dropdownData = locs;
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}
