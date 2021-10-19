using BingMapsRESTToolkit;
using BingMapsRESTToolkit.Extensions;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BingMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        private string BingMapsKey = System.Configuration.ConfigurationManager.AppSettings.Get("BingMapsKey");
        private string SessionKey;
        
        public MapWindow()
        {
            InitializeComponent();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(BingMapsKey);
            MyMap.CredentialsProvider.GetCredentials((c) =>
            {
                SessionKey = c.ApplicationId;
            });

          
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text != null)
            {
                string key = SessionKey;
                string tempquery = SearchBox.Text;

                var requestGeocode = new GeocodeRequest()
                {
                    Query = tempquery,
                    IncludeIso2 = true,
                    MaxResults = 25,
                    BingMapsKey = key
                };

                var response = await requestGeocode.Execute();

                UpdateMap(response);
            }

        }

        private void UpdateMap(Response response)
        {
            MyMap.Children.Clear();

            var location = response.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.Location;
            var latitude = location.GeocodePoints[0].Coordinates[0];
            var longitude = location.GeocodePoints[0].Coordinates[1];
            var center = new Microsoft.Maps.MapControl.WPF.Location(latitude, longitude);

            MyMap.SetView(center, 10);

            var loc = new Microsoft.Maps.MapControl.WPF.Location(latitude, longitude);

            MyMap.Children.Add(new Pushpin { Location = loc });

           

        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            MyMap.ZoomLevel += .5;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            MyMap.ZoomLevel -= .5;
        }
    }
}
