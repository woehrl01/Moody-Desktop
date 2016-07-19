using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public Vote()
        {
            InitializeComponent();

            b1.Click += (sender, args) =>
            {
                Send(1,0);
            };
            b2.Click += (sender, args) =>
            {
                Send(2, 0);
            };
            b3.Click += (sender, args) =>
            {
                Send(3, 0);
            };
            b4.Click += (sender, args) =>
            {
                Send(4, 0);
            };
        }

        public void Send(int mood, int loation)
        {
            
        }
    }
}
