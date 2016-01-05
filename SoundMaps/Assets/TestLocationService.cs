using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TestLocationService : MonoBehaviour{

    public Text isEnabledByUser;
    public Text timeOut;
    public Text connectionFail;
    public Text GPSData;

    IEnumerator Start()
    {
        // First, check if user has location service enabled

        isEnabledByUser.text = "Ok";
        timeOut.text = "OK";
        connectionFail.text = "OK";
        
        // Start service before querying location
        Input.location.Start();

        isEnabledByUser.text = Input.location.isEnabledByUser.ToString();

        print(Input.location.isEnabledByUser);
        
        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            timeOut.text = "Time Out";

            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            connectionFail.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + 
                Input.location.lastData.longitude + " " + 
                Input.location.lastData.altitude + " " + 
                Input.location.lastData.horizontalAccuracy + " " + 
                Input.location.lastData.timestamp);

            GPSData.text = "Location: " + Input.location.lastData.latitude + " " +
                Input.location.lastData.longitude + " " +
                Input.location.lastData.altitude + " " +
                Input.location.lastData.horizontalAccuracy + " " +
                Input.location.lastData.timestamp;

        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}