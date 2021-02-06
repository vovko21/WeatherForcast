using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeatherForcast.UI;

namespace WeatherForcast.ViewModel
{
    public class WeatherForcastViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> forecasts = new ObservableCollection<string>();
        private Forcast forcast;
        private DayOfWeek today = DateTime.Now.DayOfWeek;
        public DayOfWeek Today
        {
            get => today;
            set
            {
                if (today != value)
                {
                    today = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CityName { get; set; }
        private string temperatureUI;
        public string TemperatureUI
        {
            get => temperatureUI;
            set
            {
                if (temperatureUI != value)
                {
                    temperatureUI = value;
                    OnPropertyChanged();
                }
            }
        }
        private string weatherType;
        public string WeatherType
        {
            get => weatherType;
            set
            {
                if (weatherType != value)
                {
                    weatherType = value;
                    OnPropertyChanged();
                }
            }
        }
        private string tempFeelLike;
        public string TempFeelLikeUI
        {
            get => tempFeelLike;
            set
            {
                if (tempFeelLike != value)
                {
                    tempFeelLike = value;
                    OnPropertyChanged();
                }
            }
        }
        private string iconUI;
        public string IconUI
        {
            get => iconUI;
            set
            {
                if (iconUI != value)
                {
                    iconUI = Environment.CurrentDirectory;
                    iconUI = iconUI + "\\" + value;
                    OnPropertyChanged();
                }
            }
        }
        private string selectedDay;
        public string SelectedDay
        {
            get => selectedDay;
            set
            {
                if (selectedDay != value)
                {
                    selectedDay = value;
                    for (int i = 0; i < forecasts.Count; i++)
                    {
                        if (forecasts[i] == selectedDay)
                        {
                            TemperatureUI = (forcast.list[i].main.temp - 273.15).ToString("0.0");
                            WeatherType = forcast.list[i].weather[0].description;
                            TempFeelLikeUI = (forcast.list[i].main.feels_like - 273.15).ToString("0.0");

                            if (fileName != null)
                            {
                                File.Delete(fileName);
                            }
                            string icon = "";
                            icon = forcast.list[i].weather[0].icon;

                            SetIcon(icon);
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }
        private string fileName;
        public IEnumerable<string> Forecasts => forecasts;
        public ICommand searchForcast_cm;
        public ICommand SearchForcast_cm => searchForcast_cm;

        public event PropertyChangedEventHandler PropertyChanged;

        public WeatherForcastViewModel()
        {
            searchForcast_cm = new DelegateCommand(GetForecast, () => CityName != null); //() => CityName != string.Empty
            forecasts.Add("0 day");
            forecasts.Add("1 day");
            forecasts.Add("2 day");
            forecasts.Add("3 day");
            forecasts.Add("4 day");
        }

        private void GetForecast()
        {
            try
            {
                if(CityName == string.Empty) return;
                var url = $"http://api.openweathermap.org/data/2.5/forecast?q={CityName}&appid=19e4326e91ac01368d6875f712a0cd70";

                var request = WebRequest.CreateHttp(url);
                var responce = request.GetResponse();
                var stream = responce.GetResponseStream();
                var sr = new StreamReader(stream);

                var data = sr.ReadToEnd();
                forcast = JsonConvert.DeserializeObject<Forcast>(data);
                if (forcast != null)
                {
                    MessageBox.Show("Information geted");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void SetIcon(string icon)
        {
            try
            {
                var url = $"http://openweathermap.org/img/wn/{icon}@2x.png";

                fileName = $"icon.png";
                var httpCliet = new HttpClient();
                using (var responce = await httpCliet.GetAsync(url))
                {
                    if (responce.StatusCode == HttpStatusCode.OK)
                    {
                        var data = await responce.Content.ReadAsByteArrayAsync();
                        File.WriteAllBytes(fileName, data);
                    }
                    IconUI = fileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string sender = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }
}
