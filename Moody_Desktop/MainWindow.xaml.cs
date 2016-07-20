using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Moody;
using Newtonsoft.Json;

namespace Moody_Desktop
{
    public partial class MainWindow : Window
    {
        private List<Loc> _locationList;
        private string _address;

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            button.Click += delegate(object sender, RoutedEventArgs args)
            {
                Point screenCoordinates = PointToScreen(new Point(0, 0));
                Dictionary<string, double> dimens = new Dictionary<string, double>
                {
                    {"Height", Height},
                    {"Width", Width}
                };

                Vote vote = new Vote(_address, GetIdByName(comboBox.SelectedItem.ToString()),screenCoordinates);
                vote.Show();
                Close();
            };

            textBox.GotFocus += (sender, args) =>
            {
                if (textBox.Text == "Server Address")
                {
                    textBox.Text = "";
                }
            };

            textBox.LostFocus += (sender, args) =>
            {
                if (textBox.Text == "")
                {
                    textBox.Text = "Server Address";
                }
            };
        }

        private int GetIdByName(string toString)
        {
            foreach (Loc l in _locationList)
            {
                if (l.Location == toString)
                {
                    return l.Identiefier;
                }
            }
            return -1;
        }

        private void GetLocations(string address)
        {
            try
            {
                new Thread(new ThreadStart(async () =>
                {
                    var data = await LoadLocationAsync(address);

                    Application.Current.Dispatcher.Invoke(new Action((() =>
                    {
                        if (data != null)
                        {
                            comboBox.Items.Clear();
                            foreach (var item in data)
                            {
                                comboBox.Items.Add(item);
                            }
                            comboBox.SelectedItem = data.ToArray()[0];
                            comboBox.IsEnabled = true;

                            button.IsEnabled = true;
                        }
                    })));         
                })).Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong!");
            }
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && textBox.Text != null)
            {
                GetLocations(textBox.Text);
            }
        }

        private Task<List<string>> LoadLocationAsync(string address)
        {
            return Task.Run(() => DownloadLocations(address));
        }

        private List<string> DownloadLocations(string address)
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
                _address = address;
                return locs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("Something went wrong!");
                return null;
            }
        }
    }
}
