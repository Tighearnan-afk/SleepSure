
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using SleepSure.Model;

namespace SleepSure.Services
{
    public class MotionSensorRESTService : IMotionSensorRESTService
    {
        //A http client to make http requests
        HttpClient _client;
        //A JSON Serialiser Options object for specifying how the json data is serialised or deserialised
        JsonSerializerOptions _serializerOptions;

        //A list of motion sensors retrieved from the REST API
        public List<MotionSensor> MotionSensors { get; private set; }

        public MotionSensorRESTService()
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
        /// The RefreshMotionSensorsAsync method retrieves the motion sensors from the REST API and returns them
        /// </summary>
        /// <returns>A list of motion sensors</returns>

        public async Task<List<MotionSensor>> RefreshMotionSensorsAsync()
        {
            MotionSensors = [];
            //Create a new string containing the URL for the rest API and appending the route for the motion sensors
            string motionSensorEndPoint = string.Concat(Constants.RestUrl, $"motionsensors/{{0}}");
            //Create a URI object using the end point string and removing everything after the last / due to it being a GET request
            Uri uri = new Uri(string.Format(motionSensorEndPoint, string.Empty));
            try
            {
                //Get the motion sensors from the REST API and store the response message in a HttpResponseMessage object
                HttpResponseMessage response = await _client.GetAsync(uri);
                //Checks if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    //Stores the response as a string
                    string content = await response.Content.ReadAsStringAsync();
                    //Deserialises the response into the MotionSensors list
                    MotionSensors = JsonSerializer.Deserialize<List<MotionSensor>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            //Return an empty list of MotionSensors if the response is failed or an exception occurs
            return MotionSensors;
        }

        /// <summary>
        /// The SaveMotionSensorAsync method either saves a motion sensor to the REST API or updates an existing motion sensors details based
        /// upon whether its a new motion sensor or not
        /// </summary>
        /// <param name="motionSensor"></param>
        /// <param name="isNewMotionSensor"></param>
        
        public async Task SaveMotionSensorAsync(MotionSensor motionSensor, bool isNewMotionSensor)
        {
            //Create a new string containing the URL for the rest API and appending the route for the motion sensors
            string motionSensorEndPoint = string.Concat(Constants.RestUrl, $"motionsensor/{{0}}");
            //Create the Uri object
            Uri uri;
            //Checks if the motion sensor is a new motion sensor
            if (isNewMotionSensor)
                //Instanstiates the uri object with the end point and removing everything after the last / as its a POST request
                uri = new Uri(string.Format(motionSensorEndPoint, string.Empty));
            else
                //Instanstiates the uri object with the end point and adding the motion sensor Id after the / as its a PUT request
                uri = new Uri(string.Format(motionSensorEndPoint, motionSensor.Id));

            try
            {
                //Serialises the request using the JSON serialiser options and storing it in a string
                string json = JsonSerializer.Serialize(motionSensor, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                //Checks if the motion sensor is a new motion sensor
                if (isNewMotionSensor)
                    //If it is Send a POST request to the RESTAPI
                    response = await _client.PostAsync(uri, content);
                else
                    //If its not a new motion sensor send a PUT request to the RESTAPI
                    response = await _client.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tCamera successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        /// <summary>
        /// The DeleteMotionSensorAsync method delete a specificed motion sensor from the REST API
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteMotionSensorAsync(int id)
        {
            //Create a new string containing the URL for the rest API and appending the route for the motion sensors
            string motionSensorEndPoint = string.Concat(Constants.RestUrl, $"motionsensors/{{0}}");
            //Instanstiates the uri object with the end point and adding the motion sensor Id after the / as its a DELETE request
            Uri uri = new Uri(string.Format(motionSensorEndPoint, id));

            try
            {

                HttpResponseMessage response = null;
                //Delete the motion sensor from the REST API and record the response in a HttpResponseMessage object
                response = await _client.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\Motion sensor successfully deleted.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
