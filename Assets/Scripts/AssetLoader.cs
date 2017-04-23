using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtension
{
	public static void AddEventListener <T>(this Button button, T param, Action<T> OnClick)
	{
		button.onClick.AddListener(delegate
		{
			OnClick(param);
		});
	}
}
public class AssetLoader : MonoBehaviour
{
	public GameObject menuPanelBtn;
    public GameObject gesturePanel;
    public GameObject screenshotBtn;
	public GameObject orientationPanel;

	public GameObject projectPanel;
	public GameObject projectsBtnPrefab;
    public RectTransform projectsBtnContainer;
	public GameObject vrPanel;

	public Transform vrScenesPanel;
	public Button vrScenesBtnPrefab;
	public Text labelText;

	private bool vrLoading;

	private string path;
	private string assetName;
	AssetBundle bundle;

	void Start()
	{
		this.GetComponent<Image>().enabled = true;

		vrLoading = true;
	}

	public void OpenAssetBundles()
    {
		StartCoroutine(OpenBundles());
    }

	IEnumerator OpenBundles()
	{
		string path = "file:///C:/Users/J.%20MILLER/AppData/LocalLow/T3/Parallel_Test/" + assetName;

		using (WWW www = WWW.LoadFromCacheOrDownload(path, 1))
        {
            yield return www;
            if (www.error != null)
            throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;
            Instantiate(bundle.LoadAsset(assetName));
            bundle.Unload(false);
			Debug.Log("Bundle loaded from: " + path);
			www.Dispose();
        } 
		GameObject projectsBtnPrefab = GameObject.FindGameObjectWithTag("ProjectBtn");
        GameObject.Destroy(projectsBtnPrefab);

		menuPanelBtn.SetActive(true);
		gesturePanel.SetActive(true);
		screenshotBtn.SetActive(true);
		projectPanel.SetActive(false);
	}

	public void LoadAssetBundles()
    {
		//assetName = QRDecodeTest.assetName;
		assetName = ("SearsTower");
		StartCoroutine(LoadBundles());
    }

	IEnumerator LoadBundles()
	{
		//string path = "file:///C:/Users/J.%20MILLER/AppData/LocalLow/T3/Parallel_Test/" + assetName; //IF WINDOWS
        string path = "file:///storage/emulated/0/Android/data/com.T3.Parallel_Test/files/" + assetName; //IF ANDROID
		WWW www = WWW.LoadFromCacheOrDownload(path, 1);
		{
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.Log(www.error);
				yield break;
			}
			AssetBundle bundle = www.assetBundle;
			string[] bundleFolder = bundle.GetAllAssetNames();

			foreach (string assetName in bundleFolder)
			{
				GameObject container = Instantiate(projectsBtnPrefab) as GameObject;
				container.GetComponent<Text>().text = Path.GetFileNameWithoutExtension(assetName);
				container.transform.SetParent(projectsBtnContainer.transform, false);

				container.GetComponent<Button>().onClick.AddListener(() => OpenAssetBundles());
			}
			bundle.Unload(false);
		}
	}

	public void LoadVrScenes()
	{
		StartCoroutine(LoadScenes());
	}

	IEnumerator LoadScenes()
	{
		string url = "https://www.dropbox.com/s/i7zfkz9rdbylbth/vrplanets?dl=1"; //IF WINDOWS

		using (WWW www = new WWW(url))
		{
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				yield break;
			}
			
			bundle = www.assetBundle;
			string[] scenes = bundle.GetAllScenePaths();

			foreach (string sceneName in scenes)
			{
				labelText.text = Path.GetFileNameWithoutExtension(sceneName);

				var clone = Instantiate(vrScenesBtnPrefab.gameObject) as GameObject;
				clone.GetComponent<Button>().AddEventListener(labelText.text, LoadAssetSceneBundle);

				clone.SetActive(true);
				clone.transform.SetParent(vrScenesPanel);
			}
		}
	}

	public void LoadAssetSceneBundle(string sceneName)
	{
		StartCoroutine(OrientationDisplay());

		SceneManager.LoadScene(sceneName);
	}

	IEnumerator OrientationDisplay()
	{
	while (vrLoading == true)
        {
			orientationPanel.SetActive(true);
			vrPanel.SetActive(false);
			yield return new WaitForSeconds(5);
			orientationPanel.SetActive(false);
		}
	}
}