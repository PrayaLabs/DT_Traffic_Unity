using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ThingSpeakManager : MonoBehaviour
{
    // Public variables for user input in the Unity Inspector
    public int channelID;
    public string writeAPIKey;
    public string readAPIKey;

    // References to PointLights
    public GameObject pointLight1;
    public GameObject pointLight2;
    public GameObject pointLight3;

    // Fields values
    private string field1, field2, field3, field4;

    void Start()
    {
        // Start reading data from ThingSpeak every second
        InvokeRepeating(nameof(ReadDataFromThingSpeak), 0f, 1f);
    }

    // This method sends a 1 or 0 to ThingSpeak's field1 to control lights
    public void SendDataToThingSpeak(int value)
    {
        StartCoroutine(WriteDataToField1(value));
    }

    // Send data to field1 in ThingSpeak
    private IEnumerator WriteDataToField1(int value)
    {
       
        string url = $"https://api.thingspeak.com/update?api_key={writeAPIKey}&field1={value}";
        Debug.Log(url);
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successfully wrote data to field1: " + value);
        }
        else
        {
            Debug.LogError("Failed to write data to field1: " + request.error);
        }
    }

    // This method reads data from fields 1, 2, 3, and 4 of ThingSpeak
    private void ReadDataFromThingSpeak()
    {
        StartCoroutine(ReadData());
    }

    // Coroutine to read the data
    private IEnumerator ReadData()
    {
        string url = $"https://api.thingspeak.com/channels/{channelID}/feeds.json?api_key={readAPIKey}&results=1";

        UnityWebRequest request = UnityWebRequest.Get(url);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            
            ProcessResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Failed to read data from ThingSpeak: " + request.error);
        }
    }

    // Process the JSON response from ThingSpeak
    private void ProcessResponse(string jsonResponse)
    {
        // Parse the JSON data from ThingSpeak
        ThingSpeakResponse response = JsonUtility.FromJson<ThingSpeakResponse>(jsonResponse);

        if (response.feeds.Length > 0)
        {
            field1 = (response.feeds[0].field1); // Read field1
            field2 = (response.feeds[0].field2); // Read field2
            field3 = (response.feeds[0].field3); // Read field3
            field4 = (response.feeds[0].field4); // Read field4

            Debug.Log($"Field 1: {field1}, Field 2: {field2}, Field 3: {field3}, Field 4: {field4}");

            // Perform operations based on field1
            if (field1 == "1")
            {
                UpdateLightsBasedOnFields();
            }
            else
            {
                // If field1 is 0, deactivate all point lights
                DeactivateAllLights();
            }
        }
        else
        {
            Debug.LogWarning("No data available in the feed.");
        }
    }

    // Update the point lights based on field2, field3, and field4 values
    private void UpdateLightsBasedOnFields()
    {
        if (field2 == "1")
        {
            pointLight1.SetActive(true);
            pointLight2.SetActive(false);
            pointLight3.SetActive(false);
        }
        else if (field3 == "1")
        {
            pointLight1.SetActive(false);
            pointLight2.SetActive(true);
            pointLight3.SetActive(false);
        }
        else if (field4 == "1")
        {
            pointLight1.SetActive(false);
            pointLight2.SetActive(false);
            pointLight3.SetActive(true);
        }
        else
        {
            // Turn off all lights if no field is 1
            DeactivateAllLights();
        }
    }

    // Method to deactivate all point lights
    private void DeactivateAllLights()
    {
        pointLight1.SetActive(false);
        pointLight2.SetActive(false);
        pointLight3.SetActive(false);
    }

    // Class for parsing the JSON response from ThingSpeak
    [System.Serializable]
    public class ThingSpeakResponse
    {
        public Feed[] feeds;
    }

    [System.Serializable]
    public class Feed
    {
        public string field1;
        public string field2;
        public string field3;
        public string field4;
    }
}
