using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Moody_Desktop
{
    public partial class Vote : Window
    {
        private string _address;
        private int _location;

        public Vote(string url, int location, Point screenCoordinates)
        {
            _address = url;
            _location = location;
            InitializeComponent();

            Left = screenCoordinates.X;
            Top = screenCoordinates.Y;

            if (location == -1)
            {
                MessageBox.Show("Something went wrong!");
                return;
            }

            b1.Click += (sender, args) =>
            {
                Send(1);
            };
            b2.Click += (sender, args) =>
            {
                Send(2);
            };
            b3.Click += (sender, args) =>
            {
                Send(3);
            };
            b4.Click += (sender, args) =>
            {
                Send(4);
            };
        }

        private void Send(int mood)
        {
            new Thread(new ThreadStart(async () =>
            {
                bool succes = await SendAsync(_address, mood, _location);
                if (succes)
                {
                    MessageBox.Show("Send mood");
                }
                else
                {
                    MessageBox.Show("Error while sending mood!");
                }
            })).Start();
        }

        private Task<bool> SendAsync(string url, int mood, int location)
        {
            return Task.Run(() => SendToServer(url, mood, location));
        }

        private class MoodyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 10 * 1000;
                return w;
            }
        }

        private bool SendToServer(string url, int mood, int location)
        {
            try
            {
                string address = "http://" + url + "/api/entry/";
                using (WebClient client = new MoodyWebClient())
                {
                    var reqparm = new NameValueCollection
                    {
                        {"mood", mood.ToString()},
                        { "location", location.ToString()}
                    };
                    byte[] responsebytes = client.UploadValues(address, "POST", reqparm);
                    string responsebody = System.Text.Encoding.UTF8.GetString(responsebytes);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
