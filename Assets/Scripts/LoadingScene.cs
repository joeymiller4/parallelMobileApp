using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public GameObject centerText;

    public Animation leftPanelAnim;
    public Animation rightPanelAnim;

    public GameObject leftPanel;
    public GameObject rightPanel;
    public GameObject leftText;
    public GameObject rightText;

    private bool keepPlaying;

    public GameObject menuPanel;

    void Start()
    {
        keepPlaying = true;
        StartCoroutine(AppearAfterSeconds());
    }

    IEnumerator AppearAfterSeconds()
    {
        while (keepPlaying == true)
        {
            //Screen.orientation = ScreenOrientation.LandscapeLeft;

            yield return new WaitForSeconds(1.5f);
            leftPanelAnim.GetComponent<Animation>().Play("SlideLeft");
            rightPanelAnim.GetComponent<Animation>().Play("SlideRight");

            leftText.SetActive(false);
            rightText.SetActive(false);

            yield return new WaitForSeconds(1.5f);
            centerText.SetActive(false);

            leftPanel.SetActive(false);
            rightPanel.SetActive(false);

            menuPanel.SetActive(true);

            keepPlaying = false;
        }

        yield return null;
    }
}