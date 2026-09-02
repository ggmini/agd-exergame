using UnityEngine;

public class PlayerSignUp : MonoBehaviour
{
    const int MAX_PLAYERS = 4;
    private int PlayerCount = 0;

    private string[] ConnectedDevices = { "", "", "", "" };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDeviceConnected(string deviceID)
    {
        if (PlayerCount >= MAX_PLAYERS)
        {
            Debug.Log("Tried connecting additional Device, but max Player Count is already reached");
            return;
        }

        // Register device under first free slot
        for (int i=0; i<ConnectedDevices.Length; i++)
        {
            if (ConnectedDevices[i].Equals(""))
            {
                ConnectedDevices[i] = deviceID;
                break;
            }
        }

        PlayerCount++;
        UpdateCanvas();

    }

    private void OnDeviceDisconnected(string deviceID)
    {
        if (PlayerCount == 0)
        {
            Debug.Log("Tried disconnecting Device, but there are no connected devices");
            return;
        }

        // Search and remove disconnected device
        for (int i = 0; i < ConnectedDevices.Length; i++)
        {
            if (ConnectedDevices[i].Equals(deviceID))
            {
                ConnectedDevices[i] = "";
                break;
            }
        }

        PlayerCount--;
        UpdateCanvas();

    }

    private void UpdateCanvas()
    {
        
    }
}
