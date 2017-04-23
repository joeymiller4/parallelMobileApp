using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class QRDecodeTest : MonoBehaviour
{

	public QRCodeDecodeController e_qrController;

	public Text UiText;
	public GameObject scanLineObj;

    private string bundleURL;
    public static string assetName;

    public GameObject backMainMenu;
    public GameObject qrCodePanel;
    public GameObject contactPanel;
    public GameObject gesturePanel;
	public GameObject VrProjectPanel;
    public GameObject gestureImage;
    public GameObject screenshotBtn;
    public GameObject screenshotsPanel;
	public GameObject contentImage;
    public RectTransform screenshotContainer;

    private GameObject parentObject;
    private Vector2 ScreenSize;
    private Vector3 originalPos;
    private bool isMousePressed;

	public GameObject radialLoader;
	public Image progressLoadingBG;
	public Text currentProgress;
	[Range(0,100)]
	public float currentValue = 0.0f;

    // Use this for initialization
    void Start ()
    {
        if (e_qrController != null)
        {
            e_qrController.onQRScanFinished += qrScanFinished;//Add Finished Event
		}
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void qrScanFinished(string dataText)
	{
		UiText.text = dataText;
        bundleURL = dataText;

        if (scanLineObj != null)
        {
            scanLineObj.SetActive(false);
        }
        StartCoroutine(ScanAssetBundle());
    }

    IEnumerator ScanAssetBundle()
    {
		radialLoader.SetActive(true);
        while (!Caching.ready)
        yield return null;

        WWW www = new WWW(bundleURL);

		while (!www.isDone)
		{
			progressLoadingBG.fillAmount = currentValue;
			currentValue = www.progress;
			currentProgress.text = string.Format("{0:#0}%", www.progress * 100, www.url);
			Debug.Log(string.Format("{0:#0}%",www.progress * 100f, www.url));
			yield return null;
		}
        yield return www;
		currentProgress.text = ("LOADING...");

        AssetBundle bundle = www.assetBundle;

        if (bundleURL == "https://www.dropbox.com/s/4gq5w9n3kiyqxkw/searsbundle?dl=1")
        {
            assetName = ("SearsTower");
        }

        if (bundleURL == "https://www.dropbox.com/s/yx5sww0atxqhytu/olivebundle?dl=1")
        {
            assetName = ("OliveTower");
        }

        yield return new WaitForSeconds(0.1f);
		radialLoader.SetActive(false);
        Instantiate(bundle.LoadAsset(assetName));
        bundle.Unload(false);

        gesturePanel.SetActive(true);
        screenshotBtn.SetActive(true);

        //---------------------------------------------  DO NOT TOUCH  ---------------------------------------------//
        string path = Application.persistentDataPath + "/";
		System.IO.FileStream cache = new System.IO.FileStream(path + assetName + ".unity3d", System.IO.FileMode.Create);
        cache.Write(www.bytes, 0, www.bytes.Length);

/*      //download mainfest file
        string mainfestName = assetName + ".mainfest";
        www = new WWW(bundleURL + mainfestName);
        yield return www;
        cache = new System.IO.FileStream(path + mainfestName, System.IO.FileMode.Create);
        cache.Write(www.bytes, 0, www.bytes.Length);
*/
        cache.Close();
        Debug.Log("Bundle saved to: " + path);
        //---------------------------------------------  DO NOT TOUCH  ---------------------------------------------//
	}

    public void Reset()
    {
        if (e_qrController != null)
        {
            e_qrController.Reset();
        }

        if (backMainMenu != null)
        {
            backMainMenu.SetActive(false);
        }

        if (qrCodePanel != null)
        {
            qrCodePanel.SetActive(false);
        }

        if (contactPanel != null)
        {
            contactPanel.SetActive(false);
        }

        if (gesturePanel != null)
        {
            contactPanel.SetActive(false);
        }

        if (gestureImage != null)
        {
            gestureImage.SetActive(false);
        }

        if (screenshotBtn != null)
        {
            screenshotBtn.SetActive(false);
        }

		if (contentImage != null)
        {
            contentImage.SetActive(false);
        }

		if (radialLoader != null)
        {
            radialLoader.SetActive(false);
        }

		if (VrProjectPanel != null)
        {
            VrProjectPanel.SetActive(false);
        }


        if (screenshotsPanel != null)
        {
            screenshotContainer.anchoredPosition = new Vector2(150f, -150f);

            GameObject[] screenshotPrefabs = GameObject.FindGameObjectsWithTag("screenshotPrefab");
            for (var i = 0; i < screenshotPrefabs.Length; i++)
            {
                Destroy(screenshotPrefabs[i]);
            }
            screenshotsPanel.SetActive(false);
        }
        GameObject assetBundle = GameObject.FindGameObjectWithTag("AssetModel");
        GameObject.Destroy(assetBundle);

		GameObject projectsBtnPrefab = GameObject.FindGameObjectWithTag("ProjectBtn");
        GameObject.Destroy(projectsBtnPrefab);

        GameObject deviceCamera = GameObject.FindGameObjectWithTag("MainCamera");
        deviceCamera.transform.position = new Vector3(0, 0, -75);
        deviceCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

	public void ClickExit()
    {
        Application.Quit();
    }
}