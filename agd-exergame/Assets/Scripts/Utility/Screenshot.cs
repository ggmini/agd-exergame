using UnityEngine;
using UnityEngine.InputSystem;

public class Screenshot : MonoBehaviour
{
    int count = 0;
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            string screenshotName = $"DebugScreenshot {count}.png";
            ScreenCapture.CaptureScreenshot(screenshotName);
            Debug.Log("Took Screenshot: " + screenshotName);
            count++;
            Test();
        }
    }

    /// <summary>
    /// wow
    /// </summary>
    // ok ok man dange
    private void Test()
    {
        Debug.Log("wtf man");
    }



}
