using UnityEngine;
using System.Collections;

public class LocationManager : MonoBehaviour
{
    IEnumerator Start()
    {
        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled by user");
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait <= 0)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            Debug.Log("Location: " +
                      Input.location.lastData.latitude + " " +
                      Input.location.lastData.longitude + " " +
                      Input.location.lastData.altitude);
        }

        // Optional: Stop the location service if you don't need continuous updates
        // Input.location.Stop();
    }
}
