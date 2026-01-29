using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PetCare_system.Models
{
    public class GeoHelper
    {
        public static async Task<string> GetAddressFromCoordinatesAsync(double latitude, double longitude)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "PetCareSystemApp");

                // Force proper decimal formatting with "." as separator
                string lat = latitude.ToString(CultureInfo.InvariantCulture);
                string lon = longitude.ToString(CultureInfo.InvariantCulture);

                string url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lon}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return "Unknown Location";
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonString);

                var address = json["display_name"]?.ToString();
                return string.IsNullOrEmpty(address) ? "Unknown Location" : address;
            }
        }
    }
}