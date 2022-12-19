using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace GISCable
{
    public class Program
    {
        private static List<string> GetBuildingListStr(string path)
        {
            var list = new List<string>();
            string? data;
            using(StreamReader reader = new StreamReader(path))
            {
                while((data = reader.ReadLine() ) != null)
                {
                    list.Add(data);
                }
            }
            return list;
        }
        static int h = 0;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter path: ");
            string path = @"C:\Users\lalka\Desktop\демки программная иненерия\examples\GISCable\Streets.txt";
            List<string> buildingsStr = GetBuildingListStr(path);
            foreach (var item in buildingsStr)
            {
                Console.WriteLine(item);
            }

            List<Building> buildings = await ParseBuildingsFromStr(buildingsStr);

            foreach (Building building in buildings)
                Console.WriteLine(building.ToString());

            // h
            foreach (var b in buildings)
            {
                h += b.Height * 2;
            }

            for (int firstBuild = 0; firstBuild < buildings.Count; firstBuild++)
            {
                for (int secondBuild = firstBuild+1; secondBuild < buildings.Count; secondBuild++)
                {
                    h += GetDistB1B2(buildings[firstBuild], buildings[secondBuild]);
                }
                buildings.RemoveAt(0);
            }
            Console.WriteLine(h);

        }

        public static int GetDistB1B2(Building b1, Building b2)
        {
            var x1 = b1.Latitude; var y1 = b1.Longitude;
            var x2 = b2.Latitude; var y2 = b2.Longitude;
            double xMin, xMax, yMin, yMax,x3,y3;
            double distPoint1to3 = 0.0; 
            double distPoint2to3 = 0.0;
            if (x1 < x2 && y1 > y2)
            {
                (x3, y3) = (x2, y1);
                distPoint1to3 = GeoMath.distance(x1, y1, x3, y3, 'M');
                distPoint2to3 = GeoMath.distance(x2, y2, x3, y3, 'M');
            }
            else if (x1 > x2 && y1< y2)
            {
                (x3, y3) = (x2, y1);
                distPoint1to3 = GeoMath.distance(x1, y1, x3, y3, 'M');
                distPoint2to3 = GeoMath.distance(x2, y2, x3, y3, 'M');
            }
            else if (x1 < x2 && y1 < y2)
            {
                (x3, y3) = (x2,y1);
                distPoint1to3 = GeoMath.distance(x1, y1, x3, y3, 'M');
                distPoint2to3 = GeoMath.distance(x2, y2, x3, y3, 'M');
            }
            else if (x1 > x2 && y1 > y2)
            {
                (x3, y3) = (x2, y1);
                distPoint1to3 = GeoMath.distance(x1, y1, x3, y3, 'M');
                distPoint2to3 = GeoMath.distance(x2, y2, x3, y3, 'M');
            }
            Console.WriteLine($"{distPoint1to3} - {distPoint2to3}");
            var height = distPoint1to3 + distPoint2to3;
            return (int)Math.Round(height);
        }

        private static async Task<List<Building>> ParseBuildingsFromStr(List<string> buildingsStr)
        {
            List<Building> results = new List<Building>();

            foreach(var building in buildingsStr)
            {
                Building b = new Building();
                await b.RunAsync(building);
                results.Add(b);
            }

            return results;
        }
    }

    public static class GeoMath
    {
        public static double distance(double lat1, double lon1, double lat2, double lon2, char unit)
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
                else if (unit == 'M')
                {
                    dist = dist * 1000;
                }
                return (dist);
            }
        }

        public static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        public static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        
    }

    public class Building
    {
        public int Level { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Height { get; set; }
        public string Query { get; set; } = string.Empty;

        private static string GetQuery(string text)
        {
            string result = text
                .Replace("д.","")
                .Replace('.', '+')
                .Replace(",", "+")
                .Replace("1", "1");

            return result;
        }

        public async Task RunAsync(string text)
        {
            Query = GetQuery(text);
            (Level,Latitude,Longitude) = await Client.GetData(Query);
            Height = Level * 3;
        }

        public override string ToString()
        {
            return $"{Level} - {Latitude} - {Longitude} - {Height}";
        }
    }

    public class Client
    {
        private static HttpClient _httpClient = new HttpClient();
        public static string StartUrl { get; } = @"https://nominatim.openstreetmap.org/search?q=";
        public static string EndUrl { get; } = @"&format=json&polygon_geojson=1&addressdetails=0&extratags=1";
        public static string QueryUrl { get; set; } = null!;


        public static async Task<(int level, double lat, double lon)> GetData(string query)//
        {
            _httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 YaBrowser/22.11.3.818 Yowser/2.5 Safari/537.36");
            QueryUrl = StartUrl + query + EndUrl;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, QueryUrl);
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(request);

            string? addrInfoJson = responseMessage.Content.ReadAsStringAsync().Result;
            dynamic? arr = JsonConvert.DeserializeObject(addrInfoJson);
            int iter = 0;
            int? level = null;
            string? latStr = arr?[0].lat.ToString().Replace(".",",");
            string? lonStr = arr?[0].lon.ToString().Replace(".", ",");
            double lat = double.Parse(latStr!);            
            double lon = double.Parse(lonStr!);
            while (iter < 10)
            {
                try
                {
                    level = SearchInJson(arr, iter);
                    if (level is int) break;
                }
                catch (Exception)
                {
                    iter++;
                    
                }
            }



            return await Task.FromResult<(int l, double la, double lo)>(((int)level, lat, lon));
        }
        private static int SearchInJson(dynamic arr, int iter)
        {
            string? buildingLevelsString = arr?[iter].extratags.ToString();
            string? formatLevels = buildingLevelsString?.
                Replace("building:levels", "").
                TrimEnd('}').
                TrimStart('{').
                Replace("\"", "").Replace("\r", "").Replace("\n", "").Replace(":", "");
            int level = int.Parse(formatLevels!);
            return level;
        }
        

        private static string CheckQuery(string text)
        {
            string result = "";
            foreach(char s in result)
            {
                if(true)
                {

                }
                result += s;
            }

            return result;
        }
    }
}