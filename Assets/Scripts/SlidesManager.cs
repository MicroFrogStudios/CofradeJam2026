using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlidesManager : MonoBehaviour
{

    public Image fadeImage;

    void Start()
    {
        StartCoroutine(FadeIn());
    }


    public void NextScene()
    {
        StartCoroutine(FadeOut());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {

            NextScene();
        }
    }

    IEnumerator FadeIn()
    {
        Color c = fadeImage.color;
        for (float alpha = 1f; alpha > 0f; alpha -= 0.01f)
        {
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }


    }

    IEnumerator FadeOut()
    {
        Color c = fadeImage.color;
        for (float alpha = 0f; alpha < 1f; alpha += 0.01f)
        {
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }





}
