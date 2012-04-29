using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace DataVisualizationOnWindowsPhone
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
            UserText.Text = "cstevens99";
            PassText.Password = "charlie123";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.Content = "Logging in...";
            Streams(UserText.Text, PassText.Password);
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
        }

        public void Streams(string Username, string Password)
        {
            Username = "cstevens99";
            Password = "charlie123";

            //Navigate to chart page and fill in data as appropriate
            NavigationService.Navigate(new Uri("/Chart.xaml?user=" + Username + "&pass=" + Password, UriKind.Relative));
        }

    }
}