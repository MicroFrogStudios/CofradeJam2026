using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlidesManager : MonoBehaviour
{

    public Image fadeImage;
    public Animation cinematicBars;

    public static SlidesManager Instance;
    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }


    public void NextScene()
    {
        StartCoroutine(FadeOutToNextScene());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {

            NextScene();
        }
    }

    public IEnumerator FadeIn()
    {
        Color c = fadeImage.color;
        for (float alpha = 1f; alpha > 0f; alpha -= 0.01f)
        {
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }


    }

    IEnumerator FadeOutToNextScene()
    {
        Color c = fadeImage.color;
        for (float alpha = 0f; alpha < 1f; alpha += 0.01f)
        {
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % sceneCount);
    }

    public IEnumerator FadeOut()
    {
        Color c = fadeImage.color;
        for (float alpha = 0f; alpha < 1f; alpha += 0.01f)
        {
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }
    }

}
