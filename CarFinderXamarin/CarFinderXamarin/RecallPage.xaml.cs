using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarFinderXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecallPage : ContentPage
    {
        private HttpClient client = new HttpClient();
        private class RecallInfo
        {
            public dynamic Summary { get; set; }
            public dynamic Consequence { get; set; }
            public dynamic Component { get; set; }
        }
        public RecallPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var recallList = await GetRecallInfo(Car.Year, Car.Make, Car.Model);
            foreach (var item in recallList)
            {
                if (item.Component == null & item.Consequence == null)
                {
                    var noInfo = new Label()
                    {
                        FormattedText = item.Summary,
                        Margin = new Thickness(10, 0),
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
                    };
                    recallStack.Children.Add(noInfo);
                }
                else
                {
                    var componentText = new FormattedString();
                    componentText.Spans.Add(new Span { Text = "Component", FontAttributes = FontAttributes.Bold });
                    componentText.Spans.Add(new Span { Text = ": " + item.Component });
                    var componentLabel = new Label()
                    {
                        FormattedText = componentText,
                        Margin = new Thickness(10, 0),
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label))
                    };

                    var summaryText = new FormattedString();
                    summaryText.Spans.Add(new Span { Text = "Summary", FontAttributes = FontAttributes.Bold });
                    summaryText.Spans.Add(new Span { Text = ": " + item.Summary });
                    var summaryLabel = new Label()
                    {
                        FormattedText = summaryText,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(10, 0),
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
                    };

                    var consequenceText = new FormattedString();
                    consequenceText.Spans.Add(new Span { Text = "Consequence", FontAttributes = FontAttributes.Bold });
                    consequenceText.Spans.Add(new Span { Text = ": " + item.Consequence });
                    var consequenceLabel = new Label()
                    {
                        FormattedText = consequenceText,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(10, 0),
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
                    };
                    recallStack.Children.Add(componentLabel);
                    recallStack.Children.Add(summaryLabel);
                    recallStack.Children.Add(consequenceLabel);
                }
            }
        }

        private async Task<List<RecallInfo>> GetRecallInfo(string year, string make, string model)
        {
            var querystring = "?year=" + year + "&make=" + make + "&model=" + model;
            var response = await client.GetAsync("http://prhinehart-carfinder.azurewebsites.net:80/api/Cars/GetRecallInfo" + querystring);
            var result = response.Content.ReadAsStringAsync().Result;
            var recallList = new List<RecallInfo>();
            if (result == "No recall information was found for this vehicle.")
            {
                var recallInfo = new RecallInfo()
                {
                    Summary = result
                };
                recallList.Add(recallInfo);
                return recallList;
            }
            var data = (JArray)JsonConvert.DeserializeObject(result);

            foreach (var item in data.Children())
            {
                var recallInfo = new RecallInfo()
                {
                    Summary = item["Summary"].ToString(),
                    Component = item["Component"].ToString(),
                    Consequence = item["Consequence"].ToString()
                };
                recallList.Add(recallInfo);
            }
            return recallList;
        }
    }
}