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
    public partial class ImageDisplay : CarouselPage
    {
        private HttpClient client = new HttpClient();

        public class ImageSearch
        {
            public string imageLink { get; set; }
            public int imageHeight { get; set; }
            public int imageWidth { get; set; }
        }
        public ImageDisplay()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var imageList = await GetGoogleImages(Car.Year, Car.Make, Car.Model);
            
            foreach(var item in imageList)
            {
                var contentPage = new ContentPage
                {
                    Content = new StackLayout
                    {
                        BackgroundColor = Color.BlueViolet,
                        Children =
                        {
                            new Label
                            {
                                Text = "Swipe to see more pictures",
                                HorizontalOptions = LayoutOptions.Center,
                                FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label))
                            },
                            new Image
                            {
                                Source = item.imageLink,
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                WidthRequest = item.imageWidth * 2,
                                HeightRequest = item.imageHeight * 2
                            }
                        }
                    }
                };

                Children.Add(contentPage);
            }
        }

        private async Task<List<ImageSearch>> GetGoogleImages(string year, string make, string model)
        {
            var querystring = "?year=" + year + "&make=" + make + "&model=" + model;
            var response = await client.GetAsync("http://prhinehart-carfinder.azurewebsites.net:80/api/Cars/GetGoogleImages" + querystring);
            var result = response.Content.ReadAsStringAsync().Result;
            var data = (JArray)JsonConvert.DeserializeObject(result);
            var imageList = new List<ImageSearch>();
            foreach (var item in data.Children())
            {
                var imageSearch = new ImageSearch()
                {
                    imageLink = item["ThumbnailLink"].ToString(),
                    imageHeight = (int) item["ThumbnailHeight"],
                    imageWidth = (int) item["ThumbnailWidth"]
                };
                imageList.Add(imageSearch);
            }
            return imageList;
        }
    }
}