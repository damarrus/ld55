using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public GameObject fadeOutObject;
    public Animation fadeInAnimation;

    // Start is called before the first frame update
    void Start()
    {
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
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator fadeOutCoroutine()
    {
        yield return new WaitForSeconds(1f);
        fadeOutObject.SetActive(false);
    }
}
