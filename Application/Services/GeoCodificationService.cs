using Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace Application.Services
{
    public class GeoCodificationService : IGeoCodificationService
    {

        public async Task<string> GetAddressCoordinates(string address)
        {
            using (var client = new HttpClient())
            {
                var url = new Uri($"https://geocoding.geo.census.gov/geocoder/locations/onelineaddress?address={address}&benchmark=2020&format=json");
                var response = await client.GetAsync(url);
                dynamic coordinatesResponse = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync()) ?? new JObject();
                if (coordinatesResponse.result.addressMatches.Count == 0)
                    throw new Exception("No Coordinates Found In The Provided Address");
                var coordinates = new AddressCoordinatesModel
                {
                    Name = coordinatesResponse.result.input.address.address,    
                    Latitude = coordinatesResponse.result.addressMatches[0].coordinates.y,
                    Longitude = coordinatesResponse.result.addressMatches[0].coordinates.x
                };

                return await GetWeatherFromCoordinates(coordinates);
            }
        }
        public async Task<string> GetWeatherFromCoordinates(AddressCoordinatesModel addressCoordinates)
        {
            using (var client = new HttpClient())
            {
                var url = new Uri($"https://api.weather.gov/points/{addressCoordinates.Latitude},{addressCoordinates.Longitude}");

                client.DefaultRequestHeaders.Add("User-Agent" , "MyWeatherApp/1.0 (contact@myweatherapp.com)");
                var request = await client.GetAsync($"https://api.weather.gov/points/{addressCoordinates.Latitude},{addressCoordinates.Longitude}");
                if (!request.IsSuccessStatusCode)
                    throw new Exception("Error Invalid Parameters");              

                dynamic response = JsonConvert.DeserializeObject<JObject>(await request.Content.ReadAsStringAsync()) ?? new JObject(); 
                var requestForecast = await client.GetAsync(response.properties.forecast.ToString());
                return await requestForecast.Content.ReadAsStringAsync();
            }
        }
    } 
}
 