using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.Phone.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DataVisualizationOnWindowsPhone
{
    public class Pse : PhoneApplicationPage
    {

        public PseUpdateState PseState { get; set; }
        private CookieContainer Cookies { get; set; }
        private string MeterId { get; set; }
        private Double[] CcfValues { get; set; }
        public ObservableCollection<ActivityInfo> CcfInfo
        {
            get;
            set;
        }

        public Forecast Forecast;

        #region Constructors

        public Pse()
        {
            PseState = new PseUpdateState();
            Cookies = new CookieContainer();
            PseState.HtmlResult = "";
            this.MeterId = "";
            this.CcfInfo = new ObservableCollection<ActivityInfo>();

            Forecast = new Forecast();
        }

        public void PredictCcf()
        {
            Forecast.GetForecast("47.67", "-122.12");
        }

        #endregion

        public IAsyncResult PseLogin(string Username, string Password)
        {
            UriBuilder FullUri = new UriBuilder(string.Format("https://my.pse.com/SUSO/Login.aspx?user={0}&password={1}", Username, Password));
            HttpWebRequest PseRequest = (HttpWebRequest)WebRequest.Create(FullUri.Uri);
            PseRequest.CookieContainer = this.Cookies;
            PseRequest.AllowAutoRedirect = true;
            PseState.AsyncRequest = PseRequest;
            IAsyncResult AsyncResult = PseRequest.BeginGetResponse(new AsyncCallback(HandlePseResponse),
                                                                   PseState);

            return AsyncResult;
        }

        private void EnergyGuideLink(HttpWebRequest PseRequest, string Uri)
        {
            UriBuilder FullUri = new UriBuilder(Uri);
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)WebRequest.Create(FullUri.Uri);
            EnergyGuideRequest.CookieContainer = PseRequest.CookieContainer;
            EnergyGuideRequest.AllowAutoRedirect = true;
            PseUpdateState EnergyGuideState = new PseUpdateState();
            EnergyGuideState.AsyncRequest = EnergyGuideRequest;
            IAsyncResult AsyncResult = EnergyGuideRequest.BeginGetResponse(new AsyncCallback(HandleEnergyGuideResponse),
                                                                           EnergyGuideState);

            return;
        }

        private void EnergyGuideLink2(HttpWebRequest PseRequest, string Uri)
        {
            UriBuilder FullUri = new UriBuilder(Uri);
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)WebRequest.Create(FullUri.Uri);
            EnergyGuideRequest.CookieContainer = PseRequest.CookieContainer;
            EnergyGuideRequest.AllowAutoRedirect = true;
            PseUpdateState EnergyGuideState = new PseUpdateState();
            EnergyGuideState.AsyncRequest = EnergyGuideRequest;
            IAsyncResult AsyncResult = EnergyGuideRequest.BeginGetResponse(new AsyncCallback(HandleEnergyGuideResponse2),
                                                                           EnergyGuideState);

            return;
        }

        private void EnergyGuideLink3(HttpWebRequest PseRequest, string Uri)
        {
            UriBuilder FullUri = new UriBuilder(Uri);
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)WebRequest.Create(FullUri.Uri);
            EnergyGuideRequest.CookieContainer = PseRequest.CookieContainer;
            EnergyGuideRequest.AllowAutoRedirect = true;
            PseUpdateState EnergyGuideState = new PseUpdateState();
            EnergyGuideState.AsyncRequest = EnergyGuideRequest;
            IAsyncResult AsyncResult = EnergyGuideRequest.BeginGetResponse(new AsyncCallback(HandleEnergyGuideResponse3),
                                                                           EnergyGuideState);

            return;
        }

        private void EnergyGuideLink4(HttpWebRequest PseRequest, string Uri)
        {
            UriBuilder FullUri = new UriBuilder(Uri);
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)WebRequest.Create(FullUri.Uri);
            EnergyGuideRequest.CookieContainer = PseRequest.CookieContainer;
            EnergyGuideRequest.AllowAutoRedirect = true;
            PseUpdateState EnergyGuideState = new PseUpdateState();
            EnergyGuideState.AsyncRequest = EnergyGuideRequest;
            IAsyncResult AsyncResult = EnergyGuideRequest.BeginGetResponse(new AsyncCallback(HandleEnergyGuideResponse4),
                                                                           EnergyGuideState);

            return;
        }

        private void EnergyGuideLink5(HttpWebRequest PseRequest, string Uri)
        {
            UriBuilder FullUri = new UriBuilder(Uri);
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)WebRequest.Create(FullUri.Uri);
            EnergyGuideRequest.CookieContainer = PseRequest.CookieContainer;
            EnergyGuideRequest.AllowAutoRedirect = true;
            PseUpdateState EnergyGuideState = new PseUpdateState();
            EnergyGuideState.AsyncRequest = EnergyGuideRequest;
            IAsyncResult AsyncResult = EnergyGuideRequest.BeginGetResponse(new AsyncCallback(HandleEnergyGuideResponse5),
                                                                           EnergyGuideState);

            return;
        }

        private void HandlePseResponse(IAsyncResult AsyncResult)
        {
            PseUpdateState LocalPseState = (PseUpdateState)AsyncResult.AsyncState;
            HttpWebRequest PseRequest = (HttpWebRequest)LocalPseState.AsyncRequest;
            LocalPseState.AsyncResponse = (HttpWebResponse)PseRequest.EndGetResponse(AsyncResult);

            Stream StreamResult;

            try
            {
                StreamResult = LocalPseState.AsyncResponse.GetResponseStream();
            }
            catch (FormatException)
            {
                return;
            }

            StreamReader StreamReader = new StreamReader(StreamResult);
            LocalPseState.HtmlResult = StreamReader.ReadToEnd();
            StreamReader.Close();
            LocalPseState.AsyncResponse.Close();

            EnergyGuideLink(PseRequest, "https://my.pse.com/PartnerLink/PartnerDirect.aspx?partner=Nexus&FwdUrl=Analyze");
            return;
        }

        private void HandleEnergyGuideResponse(IAsyncResult AsyncResult)
        {
            PseUpdateState LocalPseState = (PseUpdateState)AsyncResult.AsyncState;
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)LocalPseState.AsyncRequest;
            LocalPseState.AsyncResponse = (HttpWebResponse)EnergyGuideRequest.EndGetResponse(AsyncResult);

            Stream StreamResult;

            try
            {
                StreamResult = LocalPseState.AsyncResponse.GetResponseStream();
            }
            catch (FormatException)
            {
                return;
            }

            StreamReader StreamReader = new StreamReader(StreamResult);
            LocalPseState.HtmlResult = StreamReader.ReadToEnd();
            StreamReader.Close();
            LocalPseState.AsyncResponse.Close();

            Regex Action = new Regex(@"action='(?<url>[:|.|/|\w]*)'");
            MatchCollection ActionMatches = Action.Matches(LocalPseState.HtmlResult);
            if (ActionMatches.Count != 1)
            {
                return;
            }

            string Url = "";
            foreach (Match ActionUrl in ActionMatches)
            {
                Url = ActionUrl.Groups["url"].Value;
            }

            Regex NameValue = new Regex(@"name='(?<name>\w*)' value='(?<value>\w*)'");
            MatchCollection NameValueMatches = NameValue.Matches(LocalPseState.HtmlResult);
            if (NameValueMatches.Count > 0)
            {
                Url += "?";
                int Count = 0;
                foreach (Match NameValueMatch in NameValueMatches)
                {
                    string NameValueString = string.Format("{0}={1}", NameValueMatch.Groups["name"].Value, NameValueMatch.Groups["value"].Value);
                    Url += NameValueString;
                    Count += 1;
                    if (Count < NameValueMatches.Count)
                    {
                        Url += "&";
                    }
                }
            }

            EnergyGuideLink2(EnergyGuideRequest, Url);
            return;
        }

        private void HandleEnergyGuideResponse2(IAsyncResult AsyncResult)
        {
            PseUpdateState LocalPseState = (PseUpdateState)AsyncResult.AsyncState;
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)LocalPseState.AsyncRequest;
            LocalPseState.AsyncResponse = (HttpWebResponse)EnergyGuideRequest.EndGetResponse(AsyncResult);

            Stream StreamResult;

            try
            {
                StreamResult = LocalPseState.AsyncResponse.GetResponseStream();
            }
            catch (FormatException)
            {
                return;
            }

            StreamReader StreamReader = new StreamReader(StreamResult);
            LocalPseState.HtmlResult = StreamReader.ReadToEnd();
            StreamReader.Close();
            LocalPseState.AsyncResponse.Close();

            Regex Action = new Regex(@"window.location.href='(?<url>[\?|&|=|:|.|/|\w]*)'");
            MatchCollection ActionMatches = Action.Matches(LocalPseState.HtmlResult);
            if (ActionMatches.Count != 1)
            {
                return;
            }

            string Url = "";
            foreach (Match ActionUrl in ActionMatches)
            {
                Url = ActionUrl.Groups["url"].Value;
            }

            Url = "https://www.energyguide.com/customercare/" + Url;
            EnergyGuideLink3(EnergyGuideRequest, Url);
            return;
        }

        private void HandleEnergyGuideResponse3(IAsyncResult AsyncResult)
        {
            PseUpdateState LocalPseState = (PseUpdateState)AsyncResult.AsyncState;
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)LocalPseState.AsyncRequest;
            LocalPseState.AsyncResponse = (HttpWebResponse)EnergyGuideRequest.EndGetResponse(AsyncResult);

            Stream StreamResult;

            try
            {
                StreamResult = LocalPseState.AsyncResponse.GetResponseStream();
            }
            catch (FormatException)
            {
                return;
            }

            StreamReader StreamReader = new StreamReader(StreamResult);
            LocalPseState.HtmlResult = StreamReader.ReadToEnd();
            StreamReader.Close();
            LocalPseState.AsyncResponse.Close();

            Regex Action = new Regex(@"window.location.href='(?<url>[\?|&|=|:|.|/|\w]*)'");
            MatchCollection ActionMatches = Action.Matches(LocalPseState.HtmlResult);
            if (ActionMatches.Count != 1)
            {
                return;
            }

            string Url = "";
            foreach (Match ActionUrl in ActionMatches)
            {
                Url = ActionUrl.Groups["url"].Value;
            }

            Url = "https://www.energyguide.com" + Url;
            EnergyGuideLink4(EnergyGuideRequest, Url);
            return;
        }

        private void HandleEnergyGuideResponse4(IAsyncResult AsyncResult)
        {
            PseUpdateState LocalPseState = (PseUpdateState)AsyncResult.AsyncState;
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)LocalPseState.AsyncRequest;
            LocalPseState.AsyncResponse = (HttpWebResponse)EnergyGuideRequest.EndGetResponse(AsyncResult);

            Stream StreamResult;

            try
            {
                StreamResult = LocalPseState.AsyncResponse.GetResponseStream();
            }
            catch (FormatException)
            {
                return;
            }

            StreamReader StreamReader = new StreamReader(StreamResult);
            LocalPseState.HtmlResult = StreamReader.ReadToEnd();
            StreamReader.Close();
            LocalPseState.AsyncResponse.Close();

            Regex MeterId = new Regex(@"Gas: (?<MeterId>[0-9]*)");
            Match MeterIdMatch = MeterId.Match(LocalPseState.HtmlResult);
            this.MeterId = MeterIdMatch.Groups["MeterId"].Value;
            string Url = string.Format("https://www.energyguide.com/LoadAnalysis/LoadAnalysis.aspx?referrerid=73&enccuid=chCzldiBdT1bn6C3q4MZuA==&meterid={0}&date={1}&mdd=3&p=2&c=1", this.MeterId, "04/01/2012");
            EnergyGuideLink5(EnergyGuideRequest, Url);
            return;
        }

        private void HandleEnergyGuideResponse5(IAsyncResult AsyncResult)
        {
            PseUpdateState LocalPseState = (PseUpdateState)AsyncResult.AsyncState;
            HttpWebRequest EnergyGuideRequest = (HttpWebRequest)LocalPseState.AsyncRequest;
            LocalPseState.AsyncResponse = (HttpWebResponse)EnergyGuideRequest.EndGetResponse(AsyncResult);

            Stream StreamResult;

            try
            {
                StreamResult = LocalPseState.AsyncResponse.GetResponseStream();
                StreamReader StreamReader = new StreamReader(StreamResult);
                LocalPseState.HtmlResult = StreamReader.ReadToEnd();
                StreamReader.Close();
                LocalPseState.AsyncResponse.Close();

                Regex DailyCcf = new Regex(@"shape=""poly"" coords=""[0-9,]*"" alt=""(?<ccf>[0-9.]*) CCF""");
                MatchCollection DailyCcfMatches = DailyCcf.Matches(LocalPseState.HtmlResult);
                if (DailyCcfMatches.Count == 0)
                {
                    return;
                }

                int Count = 0;
                int Index = 0;
                double[] CcfValues = new double[DailyCcfMatches.Count / 2];
                ObservableCollection<ActivityInfo> LocalCcfInfo = new ObservableCollection<ActivityInfo>();
                foreach (Match Ccf in DailyCcfMatches)
                {
                    if (Count < (DailyCcfMatches.Count / 2))
                    {
                        Count += 1;
                        continue;
                    }

                    CcfValues[Index] = Convert.ToDouble(Ccf.Groups["ccf"].Value);
                    Index += 1;
                    
                }

                int Date = 1;
                for (Index = (DailyCcfMatches.Count / 2) - 1; Index >= 0; Index -= 1)
                {
                    LocalCcfInfo.Add(new ActivityInfo { Date = String.Format("4/{0}", Date), Ccf = CcfValues[Index] });
                    Date += 1;
                }

                /*
                while (Index < 30)
                {
                    Index += 1;
                    LocalCcfInfo.Add(new ActivityInfo { Date = String.Format("4/{0}", Index), Ccf = 0.0 });
                }
                */

                // copy the data over
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.CcfInfo.Clear();

                    // copy the list of forecast time periods over
                    foreach (ActivityInfo Info in LocalCcfInfo)
                    {
                        this.CcfInfo.Add(Info);
                    }
                });
            }
            catch (FormatException)
            {
                return;
            }

            return;
        }
    }

    /// <summary>
    /// State information for our BeginGetResponse async call
    /// </summary>
    public class PseUpdateState
    {
        public HttpWebRequest AsyncRequest { get; set; }
        public HttpWebResponse AsyncResponse { get; set; }
        public string HtmlResult { get; set; }
    }
}
