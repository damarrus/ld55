using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    public GameObject fadeOutObject;
    public Animation fadeInAnimation;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.mute = GameController.isMute;
        StartCoroutine(fadeOutCoroutine());
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
        SceneManager.LoadScene("StartScene");
    }

    IEnumerator fadeOutCoroutine()
    {
        yield return new WaitForSeconds(1f);
        fadeOutObject.SetActive(false);
    }

    public void ToggleSound()
    {
        if (audioSource.mute)
        {
            GameController.isMute = false;
            audioSource.mute = false;

        }
        else
        {
            GameController.isMute = true;
            audioSource.mute = true;

        }
    }
}
