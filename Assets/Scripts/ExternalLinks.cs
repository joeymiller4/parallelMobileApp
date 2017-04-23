using System.Collections;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ExternalLinks : MonoBehaviour
{
    bool playAnim = false;

    void Start()
    {

    }

    void Update()
    {

    }

    public void linkWebsite()
    {
        Application.OpenURL("http://trinitycub3d.com/");
    }

    public void linkPlayStore()
    {
        Application.OpenURL("market://details?id=com.example.android");
    }

    public void linkAppStore()
        {
        Application.OpenURL("http://www.itunes.com/appname");
        }

    public void SendEmail()
    {
        string email = "contact.info@parallel.com";
        string subject = MyEscapeURL("Info:");
        string body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
}