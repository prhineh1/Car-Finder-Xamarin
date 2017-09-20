using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarFinderXamarin
{
    public partial class MainPage : ContentPage
    {
        private HttpClient client = new HttpClient();
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var yearsList = await GetCarYears();
            yearPicker.ItemsSource = yearsList;
            if (Car.Year != null)
            {
                yearPicker.SelectedItem = Car.Year;
            }
        }

        private async void OnyearPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Car.Year = (string)picker.ItemsSource[selectedIndex];
                var intYear = Convert.ToInt32(Car.Year);
                var makesList = await GetCarMakes(intYear);

                makesPicker.ItemsSource = makesList;
                makesPicker.IsVisible = true;
                carSearch.IsVisible = false;
                recallSearch.IsVisible = false;
                if (Car.Make != null)
                {
                    makesPicker.SelectedItem = Car.Make;
                }
            }
        }

        private async void makesPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Car.Make = (string)picker.ItemsSource[selectedIndex];
                var intYear = Convert.ToInt32(Car.Year);
                var modelsList = await GetCarModels(intYear, Car.Make);

                modelsPicker.ItemsSource = modelsList;
                modelsPicker.IsVisible = true;
                carSearch.IsVisible = false;
                recallSearch.IsVisible = false;
                if (Car.Model != null)
                {
                    modelsPicker.SelectedItem = Car.Model;
                }
            }
        }

        private void modelsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Car.Model = (string)picker.ItemsSource[selectedIndex];
            }
            carSearch.IsVisible = true;
            recallSearch.IsVisible = true;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            NavigationPage.SetBackButtonTitle(this, "Back");
            await Navigation.PushAsync(new ImageDisplay());
        }

        private async Task<List<string>> GetCarYears()
        {
            var response = await client.GetAsync("http://prhinehart-carfinder.azurewebsites.net:80/api/Cars/GetModelYears");
            var result = response.Content.ReadAsStringAsync().Result;
            var data = (JArray)JsonConvert.DeserializeObject(result);
            var yearsList = new List<string>();
            foreach (JValue item in data.Children())
            {
                var prop = item.Value.ToString();
                yearsList.Add(prop);
            }
            return yearsList;
        }

        private async Task<List<string>> GetCarMakes(int year)
        {
            var response = await client.GetAsync("http://prhinehart-carfinder.azurewebsites.net:80/api/Cars/GetMakesForYear?year=" + year);
            var result = response.Content.ReadAsStringAsync().Result;
            var data = (JArray)JsonConvert.DeserializeObject(result);
            var makesList = new List<string>();
            foreach (JValue item in data.Children())
            {
                var prop = item.Value.ToString();
                makesList.Add(prop);
            }
            return makesList;
        }

        private async Task<List<string>> GetCarModels(int year, string make)
        {
            var querystring = "?year=" + year + "&make=" + make;
            var response = await client.GetAsync("http://prhinehart-carfinder.azurewebsites.net:80/api/Cars/GetModelsForYearAndMake" + querystring);
            var result = response.Content.ReadAsStringAsync().Result;
            var data = (JArray)JsonConvert.DeserializeObject(result);
            var modelsList = new List<string>();
            foreach (JValue item in data.Children())
            {
                var prop = item.Value.ToString();
                modelsList.Add(prop);
            }
            return modelsList;
        }

        private async void recallSearch_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecallPage());
        }
    }
}
