using SleepSure.Model;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SleepSure.Services
{
    public class LightRESTService : ILightRESTService
    {
        //A http client to make http requests
        HttpClient _client;
        //A JSON Serialiser Options object for specifying how the json data is serialised or deserialised
        JsonSerializerOptions _serializerOptions;

        //A list oflights retrieved from the REST API
        public List<Light> Lights { get; private set; }

        public LightRESTService()
        {
            //Retrieved from "https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?view=net-maui-9.0&viewFallbackFrom=net-maui-7.0"
            //Needed to allow the android emulator to connect to the REST API over HTTPS
            var handler = new HttpClientHandler();

#if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
#endif
            //Instanstiates the http client passing the server certificate validation handler as a paramater
            _client = new HttpClient(handler);
            //Instansitates the serialiser options to use camelCase and indented lines for the JSON response
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        /// <summary>
        /// The RefreshLightsAsync method retrieves thelights from the REST API and returns them
        /// </summary>
        /// <returns>A list oflights</returns>

        public async Task<List<Light>> RefreshLightsAsync()
        {
            Lights = [];
            //Create a new string containing the URL for the rest API and appending the route for thelights
            string lightEndPoint = string.Concat(Constants.RestUrl, $"lights/{{0}}");
            //Create a URI object using the end point string and removing everything after the last / due to it being a GET request
            Uri uri = new Uri(string.Format(lightEndPoint, string.Empty));
            try
            {
                //Get thelights from the REST API and store the response message in a HttpResponseMessage object
                HttpResponseMessage response = await _client.GetAsync(uri);
                //Checks if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    //Stores the response as a string
                    string content = await response.Content.ReadAsStringAsync();
                    //Deserialises the response into the Lights list
                    Lights = JsonSerializer.Deserialize<List<Light>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            //Return an empty list of Lights if the response is failed or an exception occurs
            return Lights;
        }

        /// <summary>
        /// The SaveLightAsync method either saves alight to the REST API or updates an existinglights details based
        /// upon whether its a newlight or not
        /// </summary>
        /// <param name="light"></param>
        /// <param name="isNewLight"></param>

        public async Task SaveLightAsync(Light light, bool isNewLight)
        {
            //Create a new string containing the URL for the rest API and appending the route for thelights
            string lightEndPoint = string.Concat(Constants.RestUrl, $"lights/{{0}}");
            //Create the Uri object
            Uri uri;
            //Checks if thelight is a new light
            if (isNewLight)
                //Instanstiates the uri object with the end point and removing everything after the last / as its a POST request
                uri = new Uri(string.Format(lightEndPoint, string.Empty));
            else
                //Instanstiates the uri object with the end point and adding thelight Id after the / as its a PUT request
                uri = new Uri(string.Format(lightEndPoint, light.Id));

            try
            {
                //Serialises the request using the JSON serialiser options and storing it in a string
                string json = JsonSerializer.Serialize(light, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                //Checks if the light is a new light
                if (isNewLight)
                    //If it is Send a POST request to the RESTAPI
                    response = await _client.PostAsync(uri, content);
                else
                    //If its not a newlight send a PUT request to the RESTAPI
                    response = await _client.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"Light successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        /// <summary>
        /// The DeleteLightAsync method delete a specificedlight from the REST API
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteLightAsync(int id)
        {
            //Create a new string containing the URL for the rest API and appending the route for thelights
            string lightEndPoint = string.Concat(Constants.RestUrl, $"lights/{{0}}");
            //Instanstiates the uri object with the end point and adding thelight Id after the / as its a DELETE request
            Uri uri = new Uri(string.Format(lightEndPoint, id));

            try
            {

                HttpResponseMessage response = null;
                //Delete thelight from the REST API and record the response in a HttpResponseMessage object
                response = await _client.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"Light successfully deleted.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
