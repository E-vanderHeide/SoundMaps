﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public Text isEnabledByUser;
    public Text timeOut;
    public Text connectionFail;
    public Text status;
    public Text GPSData;

    private float latitude;
    private float longitude;

    private float oldLatitude;
    private float oldLongitude;


    IEnumerator Start()
    {
        oldLatitude = 0;
        oldLongitude = 0;

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

            latitude = (Input.location.lastData.latitude - 59) * 100000;
            longitude = (Input.location.lastData.longitude - 18) * 100000;

            GPSData.text = "Latitude: " + latitude + " " +
            System.Environment.NewLine +
            "Longitude: " + longitude + " " +
            System.Environment.NewLine +
            "Altitude: " + Input.location.lastData.altitude + " " +
            System.Environment.NewLine +
            "HorizontalAccuary :" + Input.location.lastData.horizontalAccuracy + " " +
            System.Environment.NewLine +
            "VerticalAccuary: " + Input.location.lastData.verticalAccuracy + " " +
            System.Environment.NewLine +
            "timestamp:" + Input.location.lastData.timestamp + " " +
            System.Environment.NewLine;

            movePlayer();

            oldLatitude = latitude;
            oldLongitude = longitude;

        }

    }

    void LateUpdate()
    {

        status.text = "Status: " + Input.location.status;

        latitude = (Input.location.lastData.latitude - 59) * 100000;
        longitude = (Input.location.lastData.longitude - 18) * 100000;


        GPSData.text = "Latitude: " + latitude + " " +
            System.Environment.NewLine +
            "Longitude: " + longitude + " " +
            System.Environment.NewLine +
            "Altitude: " + Input.location.lastData.altitude + " " +
            System.Environment.NewLine +
            "HorizontalAccuary :" + Input.location.lastData.horizontalAccuracy + " " +
            System.Environment.NewLine +
            "VerticalAccuary: " + Input.location.lastData.verticalAccuracy + " " +
            System.Environment.NewLine +
            "timestamp:" + Input.location.lastData.timestamp + " " +
            System.Environment.NewLine;

        movePlayer();

        oldLatitude = latitude;
        oldLongitude = longitude;

    }

    void movePlayer()
    {
        transform.Translate(latitude - oldLatitude, 0, longitude - oldLongitude, Space.World);
    }


}

