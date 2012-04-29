// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Controls.DataVisualization.Charting;

namespace DataVisualizationOnWindowsPhone
{
    public class EnergyData
    {
        public int CCF { get; set; } 
        public int WeatherHigh { get; set; } 
        public EnergyData(int ccf, int weatherhigh)
        {
            CCF = ccf;
            WeatherHigh = weatherhigh;
        }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        private Pse PseInformation;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // get the city, latitude, and longitude out of the query string 
            string userName = this.NavigationContext.QueryString["user"];
            string passWord = this.NavigationContext.QueryString["pass"];

            AnalyzeStreams(userName, passWord);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            // Set appropriate template for new orientation
            ControlTemplate template;
            switch (e.Orientation)
            {
                case PageOrientation.Portrait:
                case PageOrientation.PortraitDown:
                case PageOrientation.PortraitUp:
                default:
                    template = Application.Current.Resources["PhoneChartPortraitTemplate"] as ControlTemplate;
                    break;
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                case PageOrientation.LandscapeRight:
                    template = Application.Current.Resources["PhoneChartLandscapeTemplate"] as ControlTemplate;
                    break;
            }

            CcfChart.Template = template;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            PseInformation.PredictCcf();
            Button1.Visibility = Visibility.Collapsed;
        }

        public void AnalyzeStreams (string username, string password){
            string UID = username;
            string PassID = password;

            PseInformation = new Pse();
            IAsyncResult Result = PseInformation.PseLogin(UID, PassID);
            CcfChart.DataContext = PseInformation;
            ((LineSeries)CcfChart.Series[0]).ItemsSource = PseInformation.CcfInfo;

            // set source for ForecastList box in page to our list of forecast time periods
            ((LineSeries)CcfChart.Series[1]).ItemsSource = PseInformation.Forecast.ForecastList;
            //Forecast = new Forecast();
            //Forecast.GetForecast("47.67", "-122.12");
            return;
        }
    }

    // Class for storing information about an activity
    public class ActivityInfo
    {
        public string Date { get; set; }
        public double Ccf { get; set; }
    }

    // Class for storing information about an activity
    public class ForecastInfo
    {
        public string Date { get; set; }
        public double Ccf { get; set; }
    }
}
