using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public GameObject redLight;    // Assign Red Light GameObject in the inspector
    public GameObject yellowLight; // Assign Yellow Light GameObject in the inspector
    public GameObject greenLight;  // Assign Green Light GameObject in the inspector

    public float redGreenDuration = 5f; // Duration for Red and Green lights (seconds)
    public float yellowDuration = 2f;   // Duration for Yellow light (seconds)

    private int currentLightIndex = 0;  // To keep track of the current light

    void Start()
    {
        // Start the traffic light sequence
        InvokeRepeating("ChangeLight", 0f, redGreenDuration);  // Start with the Red light
    }

    // This method is called at each interval to switch lights
    void ChangeLight()
    {
        // Turn off all lights first
        redLight.SetActive(false);
        yellowLight.SetActive(false);
        greenLight.SetActive(false);

        // Determine which light to turn on based on currentLightIndex
        switch (currentLightIndex)
        {
            case 0:
                redLight.SetActive(true);  // Turn on Red light
                CancelInvoke();            // Stop the current InvokeRepeating
                InvokeRepeating("ChangeLight", redGreenDuration, redGreenDuration);  // Set timing for red and green light
                break;
            case 1:
                greenLight.SetActive(true);  // Turn on Green light
                CancelInvoke();              // Stop the current InvokeRepeating
                InvokeRepeating("ChangeLight", redGreenDuration, redGreenDuration);  // Set timing for red and green light
                break;
            case 2:
                yellowLight.SetActive(true); // Turn on Yellow light
                CancelInvoke();              // Stop the current InvokeRepeating
                InvokeRepeating("ChangeLight", yellowDuration, yellowDuration);      // Set timing for yellow light
                break;
        }

        // Increment and reset index to loop through the lights
        currentLightIndex = (currentLightIndex + 1) % 3;
    }
}
