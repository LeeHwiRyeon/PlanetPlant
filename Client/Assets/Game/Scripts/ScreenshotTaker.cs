using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour {
#if UNITY_EDITOR
    private string screenshotFolderPath = "Screenshots/";
    public KeyCode screenshotKey = KeyCode.F9;
    public int superSize = 1;
    public bool isPaused = false;


    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == screenshotKey) {
            StartCoroutine(TakeScreenshot());
        }
    }



    private void Update()
    {
        if (isPaused) {
            PauseGame();
        } else {
            ResumeGame();
        }

        if (Input.GetKeyDown(screenshotKey)) {
            StartCoroutine(TakeScreenshot());

        }
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string filePath = Path.Combine(screenshotFolderPath, fileName);

        if (!Directory.Exists(screenshotFolderPath)) {
            Directory.CreateDirectory(screenshotFolderPath);
        }

        ScreenCapture.CaptureScreenshot(filePath, superSize);
        Debug.Log("Screenshot saved to: " + filePath);

        yield return new WaitForSecondsRealtime(0.5f);
        ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
    }
#endif
}