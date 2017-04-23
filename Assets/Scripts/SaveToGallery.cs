using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveToGallery : MonoBehaviour
{
    bool presentlyProcessingScreenshot = false;

    int screenWidth;
    int screenHeight;
    bool currentlyRescanning;
    bool runningOnAndroid;

    public GameObject ScreenshotBtn;

    public GameObject screenshotBtnPrefab;
    public GameObject screenshotBtnContainer;

    public ArrayList _projectImages = new ArrayList();
    Texture2D newTexture;
    private Sprite screenshotImg;
    private SpriteRenderer sr;

    string subject = "eg. subject";
    string body = "eg. body text";

    void Awake()
    {
        sr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        runningOnAndroid = Application.platform == RuntimePlatform.Android;

        ScreenshotBtn.GetComponent<Button>().onClick.AddListener(() => SaveScreenshot());
    }

    void LateUpdate()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    IEnumerator manuallyCodedScreenshot(string path, bool hideGUI, string basepath)
    {
        Texture2D screencap = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        // disable canvas prior to taking screenshot
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        // take screenshot, load PNG to path
        screencap.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0, true);
        screencap.Apply();
        File.WriteAllBytes(path, screencap.EncodeToPNG());

        // wait (1) second to return canvas
        yield return new WaitForSeconds(1);
        // enable canvas after taking screenshot
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;

        // refresh gallery if running android
        if (runningOnAndroid)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.plugin.screenshot.ScreenShotPlugin");
            pluginClass.CallStatic("RefreshGallery", new object[2] { activity, path });
        }

        presentlyProcessingScreenshot = false;
        Debug.Log("Screenshot has been captured/saved");
        Debug.Log(path);
    }

    public void SaveScreenshot()
    {
        if (!presentlyProcessingScreenshot)
        {
            presentlyProcessingScreenshot = true;
            // save to 'T3Screenshots' folder in gallery
            string path = (!runningOnAndroid ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                : "/storage/emulated/0/Pictures") + "/T3Screenshots/";
            string basepath = path;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // save image name/title
            path += ("Test Screenshot " + DateTime.UtcNow + ".jpg").Replace(' ', '_').Replace(':', '_').Replace('/', '_');

            int addendum = 0;
            while (Directory.Exists(path + (addendum == 0 ? "" : " (" + addendum + ")"))) addendum++;
            path += (addendum == 0 ? "" : " (" + addendum + ")");
            StartCoroutine(manuallyCodedScreenshot(path, true, basepath));
        }
    }
    public void LoadFromGallery()
    {
        StartCoroutine(LoadGallery());
    }

    private IEnumerator LoadGallery()
    {
        string _imageFolderPath = (!runningOnAndroid ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        : "/storage/emulated/0/Pictures") + "/T3Screenshots/";

        List<Texture2D> _projectImages = new List<Texture2D>();
        Debug.Log("Checking if directory exists");
        if (Directory.Exists(_imageFolderPath))
        {
            // make sure your list is empty your list
            _projectImages.Clear();
            Debug.Log("Get all paths for existing images");
            string[] dirPaths = Directory.GetFiles(_imageFolderPath, "*.jpg", SearchOption.AllDirectories);
            if (dirPaths.Length > 0)
            {
                //load each images
                foreach (string path in dirPaths)
                {
                    string url = "";
                    // look if the path is an url or a local path
                    if (path.Contains("http:"))
                    {
                        url = path;
                    }
                    else
                    {
                        url = "file:///" + path;
                    }
                    // load specific image
                    WWW www = new WWW(url);
                    yield return www;

                    // load image into texture
                    newTexture = www.texture;
                    Debug.Log(url);

                    screenshotImg = Sprite.Create(newTexture, new Rect(0.0f, 0.0f, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

                    GameObject container = Instantiate(screenshotBtnPrefab) as GameObject;
                    container.GetComponent<Image>().sprite = screenshotImg;
                    container.transform.SetParent(screenshotBtnContainer.transform, false);

                    container.GetComponent<Button>().onClick.AddListener(() => ShareScreenshots());

                    _projectImages.Add(newTexture);
                }
            }
        }
    }

    public void ShareScreenshots()
    {
        //execute the below lines if being run on a Android device
        #if UNITY_ANDROID
        //Reference of AndroidJavaClass class for intent
        AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
        //Reference of AndroidJavaObject class for intent
        AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
        //call setAction method of the Intent object created
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //set the type of sharing that is happening
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        //add data to be passed to the other activity i.e., the data to be sent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
        //get the current activity
        AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //start the activity by sending the intent data
        currentActivity.Call ("startActivity", intentObject);
        #endif
    }
}

