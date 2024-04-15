using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScript : MonoBehaviour
{
    public GameObject fadeOutObject;
    public Animation fadeInAnimation;
    public GameObject muteButton;
    public GameObject unmuteButton;
    public AudioSource audioSource;

    public GameObject blackImage;
    public GameObject cat1;
    public GameObject cat2;
    public GameObject cat3;
    public GameObject cat4;

    static bool firstStart = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.mute = GameController.isMute;
        muteButton.SetActive(!GameController.isMute);
        unmuteButton.SetActive(GameController.isMute);
        StartCoroutine(fadeOutCoroutine());
        //if (firstStart)
        //{
        //    fadeOutObject.SetActive(false);
        //    firstStart = false;
        //} else
        //{
        //    StartCoroutine(fadeOutCoroutine());
        //}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAgain()
    {
        StartCoroutine(fadeInCoroutine());
    }

    IEnumerator fadeInCoroutine()
    {
        fadeInAnimation.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator fadeOutCoroutine()
    {
        fadeOutObject.SetActive(false);

        yield return new WaitForSeconds(0.25f);
        cat1.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        cat2.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        cat3.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        cat4.SetActive(true);

        fadeOutObject.SetActive(true);
        blackImage.SetActive(false);
        yield return new WaitForSeconds(1f);
        fadeOutObject.SetActive(false);
    }

    public void ToggleSound()
    {
        if (audioSource.mute)
        {
            GameController.isMute = false;
            audioSource.mute = false;
            muteButton.SetActive(true);
            unmuteButton.SetActive(false);
        }
        else
        {
            GameController.isMute = true;
            audioSource.mute = true;
            muteButton.SetActive(false);
            unmuteButton.SetActive(true);
        }
    }
}
