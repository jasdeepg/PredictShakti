/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Net;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DataVisualizationOnWindowsPhone
{
    public class Forecast : INotifyPropertyChanged
    {

        #region member variables

        // name of city forecast is for
        private string cityName;
        // elevation of city
        private int height;
        // source of information
        private string credit;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Accessors

        // collection of forecasts for each time period
        public ObservableCollection<ForecastInfo> ForecastList
        {
            get;
            set;
        }

        public String CityName
        {
            get
            {
                return cityName;
            }
            set
            {
                if (value != cityName)
                {
                    cityName = value;
                    NotifyPropertyChanged("CityName");
                }
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value != height)
                {
                    height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        public String Credit
        {
            get
            {
                return credit;
            }
            set
            {
                if (value != credit)
                {
                    credit = value;
                    NotifyPropertyChanged("Credit");
                }
            }
        }

        #endregion


        #region constructors

        public Forecast()
        {
            ForecastList = new ObservableCollection<ForecastInfo>();
        }

        #endregion


        #region private Helpers

        /// <summary>
        /// Raise the PropertyChanged event and pass along the property that changed
        /// </summary>
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        /// <summary>
        /// Get a forecast for the given latitude and longitude
        /// </summary>
        public void GetForecast(string latitude, string longitude)
        {
            // form the URI
            UriBuilder fullUri = new UriBuilder("http://forecast.weather.gov/MapClick.php");
            fullUri.Query = "lat=" + latitude + "&lon=" + longitude + "&FcstType=dwml";

            // initialize a new WebRequest
            HttpWebRequest forecastRequest = (HttpWebRequest)WebRequest.Create(fullUri.Uri);

            // set up the state object for the async request
            ForecastUpdateState forecastState = new ForecastUpdateState();
            forecastState.AsyncRequest = forecastRequest;

            // start the asynchronous request
            forecastRequest.BeginGetResponse(new AsyncCallback(HandleForecastResponse),
                forecastState);
        }

        /// <summary>
        /// Handle the information returned from the async request
        /// </summary>
        /// <param name="asyncResult"></param>
        private void HandleForecastResponse(IAsyncResult asyncResult)
        {
            // get the state information
            ForecastUpdateState forecastState = (ForecastUpdateState)asyncResult.AsyncState;
            HttpWebRequest forecastRequest = (HttpWebRequest)forecastState.AsyncRequest;

            // end the async request
            forecastState.AsyncResponse = (HttpWebResponse)forecastRequest.EndGetResponse(asyncResult);

            Stream streamResult;

            string newCredit = "";
            string newCityName = "";
            int newHeight = 0;

            // create a temp collection for the new forecast information for each 
            // time period
            ObservableCollection<ForecastInfo> newForecastList =
                new ObservableCollection<ForecastInfo>();

            try
            {
                // get the stream containing the response from the async call
                streamResult = forecastState.AsyncResponse.GetResponseStream();

                // load the XML
                XElement xmlWeather = XElement.Load(streamResult);

                // start parsing the XML.  You can see what the XML looks like by 
                // browsing to: 
                // http://forecast.weather.gov/MapClick.php?lat=44.52160&lon=-87.98980&FcstType=dwml

                // find the source element and retrieve the credit information
                XElement xmlCurrent = xmlWeather.Descendants("source").First();
                newCredit = (string)(xmlCurrent.Element("credit"));

                // find the source element and retrieve the city and elevation information
                xmlCurrent = xmlWeather.Descendants("location").First();
                newCityName = (string)(xmlCurrent.Element("city"));
                newHeight = (int)(xmlCurrent.Element("height"));

                // find the first time layout element
                xmlCurrent = xmlWeather.Descendants("time-layout").First();

                int timeIndex = 1;

                // search through the time layout elements until you find a node 
                // contains at least 12 time periods of information. Other nodes can be ignored
                while (xmlCurrent.Elements("start-valid-time").Count() < 12)
                {
                    xmlCurrent = xmlWeather.Descendants("time-layout").ElementAt(timeIndex);
                    timeIndex++;
                }

                ForecastPeriod newPeriod;
                

                // For each time period element, create a new forecast object and store
                // the time period name.
                // Time periods vary depending on the time of day the data is fetched.  
                // You may get "Today", "Tonight", "Monday", "Monday Night", etc
                // or you may get "Tonight", "Monday", "Monday Night", etc
                // or you may get "This Afternoon", "Tonight", "Monday", "Monday Night", etc

               
                foreach (XElement curElement in xmlCurrent.Elements("start-valid-time"))
                {
                    try
                    {
                        newPeriod = new ForecastPeriod();
                        newPeriod.TimeName = (string)(curElement.Attribute("period-name"));
                        //newForecastList.Add(newInfo);
                    }
                    catch (FormatException)
                    {

                    }
                }

                // now read in the weather data for each time period
                GetMinMaxTemperatures(xmlWeather, newForecastList);

                // copy the data over
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // copy forecast object over
                    Credit = newCredit;
                    Height = newHeight;
                    CityName = newCityName;

                    ForecastList.Clear();

                    // copy the list of forecast time periods over
                    foreach (ForecastInfo forecastInfo in newForecastList)
                    {
                        ForecastList.Add(forecastInfo);
                    }
                });


            }
            catch (FormatException)
            {
                // there was some kind of error processing the response from the web
                // additional error handling would normally be added here
                return;
            }

        }


        /// <summary>
        /// Get the minimum and maximum temperatures for all the time periods
        /// </summary>
        private void GetMinMaxTemperatures(XElement xmlWeather, ObservableCollection<ForecastInfo> newForecastList)
        {
            XElement xmlCurrent;

            // Find the temperature parameters.   if first time period is "Tonight",
            // then the Daily Minimum Temperatures are listed first.
            // Otherwise the Daily Maximum Temperatures are listed first


            xmlCurrent = xmlWeather.Descendants("parameters").First();
            ForecastInfo newInfo;
            // then get the Daily Maximum Temperatures
            int count = 0;
            foreach (XElement curElement in xmlCurrent.Elements("temperature").
                ElementAt(0).Elements("value"))
            {
                count += 1;
            }

            double[] Ccfs = new double[count];
            int index = 0;
            foreach (XElement curElement in xmlCurrent.Elements("temperature").
                ElementAt(0).Elements("value"))
            {
                Ccfs[index] = double.Parse(curElement.Value);
                index += 1;
            }

            index = 0;
            foreach (XElement curElement in xmlCurrent.Elements("temperature").
                ElementAt(1).Elements("value"))
            {
                Ccfs[index] += double.Parse(curElement.Value);
                Ccfs[index] /= 2.0;
                index += 1;
            }

            int date = 19;
            for (index = 0; index < count; index += 1)
            {
                newInfo = new ForecastInfo();
                newInfo.Date = string.Format("4/{0}", date);
                newInfo.Ccf = Ccfs[index] * -0.0366 + 2.4553;
                newForecastList.Add(newInfo);
                date += 1;
            }

            /*
            int ElementIndex = 0;
            if (string.Compare(xmlCurrent.Elements("temperature").ElementAt(0).Attribute("type").ToString(), "maximum") == 0)
            {
                ElementIndex = 1;
            }

            // get the Daily Minimum Temperatures
            int Index = 19;
            ForecastInfo newInfo;
            // then get the Daily Maximum Temperatures
            foreach (XElement curElement in xmlCurrent.Elements("temperature").
                ElementAt(ElementIndex).Elements("value"))
            {

                newInfo = new ForecastInfo();
                newInfo.Date = string.Format("4/{0}", Index);
                newInfo.Ccf = double.Parse(curElement.Value)*-0.053 + 2.9164;
                Index += 1;
                newForecastList.Add(newInfo);
            }
             * 
             * */
        }
    }


    /// <summary>
    /// State information for our BeginGetResponse async call
    /// </summary>
    public class ForecastUpdateState
    {
        public HttpWebRequest AsyncRequest { get; set; }
        public HttpWebResponse AsyncResponse { get; set; }
    }
}
