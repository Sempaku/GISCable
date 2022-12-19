using Dadata;
using Dadata.Model;
using GoogleMaps.LocationServices;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using GeoDataSource;
namespace GISCable
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            /*List<string> addr = new List<string>()
            {
                "Екатеринбург, ул. Вайнера, д. 16"
            };

            string url = @"https://nominatim.openstreetmap.org/search?q=%D0%95%D0%BA%D0%B0%D1%82%D0%B5%D1%80%D0%B8%D0%BD%D0%B1%D1%83%D1%80%D0%B3+%D1%83%D0%BB+%D0%92%D0%B0%D0%B9%D0%BD%D0%B5%D1%80%D0%B0+16&format=json&polygon_geojson=1&addressdetails=0&extratags=1";

            HttpClient httpClient = new HttpClient();
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 YaBrowser/22.11.3.818 Yowser/2.5 Safari/537.36");
            
            HttpResponseMessage responseMessage= await httpClient.SendAsync(request);

            var addrInfoJson = responseMessage.Content.ReadAsStringAsync().Result;
            Console.WriteLine(addrInfoJson);

            dynamic arr = Newtonsoft.Json.JsonConvert.DeserializeObject(addrInfoJson);
            string buildingLevelsString = arr[0].extratags.ToString();
            string formatLevels = buildingLevelsString.
                Replace("building:levels","").
                TrimEnd('}').
                TrimStart('{').
                Replace("\"", "").Replace("\r", "").Replace("\n", "").Replace(":","");
            int level = int.Parse(formatLevels);
            Console.WriteLine(level);*/



            double distance(double lat1, double lon1, double lat2, double lon2, char unit)
            {
                if ((lat1 == lat2) && (lon1 == lon2))
                {
                    return 0;
                }
                else
                {
                    double theta = lon1 - lon2;
                    double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                    dist = Math.Acos(dist);
                    dist = rad2deg(dist);
                    dist = dist * 60 * 1.1515;
                    if (unit == 'K')
                    {
                        dist = dist * 1.609344;
                    }
                    else if (unit == 'N')
                    {
                        dist = dist * 0.8684;
                    }
                    return (dist);
                }
            }

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //::  This function converts decimal degrees to radians             :::
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            double deg2rad(double deg)
            {
                return (deg * Math.PI / 180.0);
            }

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //::  This function converts radians to decimal degrees             :::
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            double rad2deg(double rad)
            {
                return (rad / Math.PI * 180.0);
            }

            int MetersFromKilometers(double km)
            {
                return (int)(km * 1000);
            }
            Console.WriteLine(MetersFromKilometers(distance(55.719547, 52.388231, 55.719928, 52.388226,'K')));

        }
    }
}