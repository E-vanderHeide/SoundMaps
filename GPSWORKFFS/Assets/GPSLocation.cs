using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GPSLocation : MonoBehaviour
{

    public Text isEnabledByUser;
    public Text timeOut;
    public Text connectionFail;
    public Text status;
    public Text GPSData;

    IEnumerator Start()
    {

        isEnabledByUser.text = "Ok";
        timeOut.text = "No Time Out: OK";
        connectionFail.text = "Connection: OK";

        // Start service before querying location
        Input.location.Start(5f, 5f);

        isEnabledByUser.text = "Enabled by user: " + Input.location.isEnabledByUser.ToString();

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


            GPSData.text = "Latitude: " + Input.location.lastData.latitude + " " +
            System.Environment.NewLine +
            "Longitude: " + Input.location.lastData.longitude + " " +
            System.Environment.NewLine +
            "Altitude: " + Input.location.lastData.altitude + " " +
            System.Environment.NewLine +
            "HorizontalAccuary :" + Input.location.lastData.horizontalAccuracy + " " +
            System.Environment.NewLine +
            "VerticalAccuary: " + Input.location.lastData.verticalAccuracy + " " +
            System.Environment.NewLine +
            "timestamp:" + Input.location.lastData.timestamp + " " +
            System.Environment.NewLine;
        }

    }

    void LateUpdate()
    {

        
        status.text = "Status: " + Input.location.status;

        GPSData.text = "Latitude: " + Input.location.lastData.latitude + " " +
            System.Environment.NewLine +
            "Longitude: " + Input.location.lastData.longitude + " " +
            System.Environment.NewLine +
            "Altitude: " + Input.location.lastData.altitude + " " +
            System.Environment.NewLine +
            "HorizontalAccuary :" + Input.location.lastData.horizontalAccuracy + " " +
            System.Environment.NewLine +
            "VerticalAccuary: " + Input.location.lastData.verticalAccuracy + " " +
            System.Environment.NewLine +
            "timestamp:" + Input.location.lastData.timestamp + " " +
            System.Environment.NewLine;


    }


}
