using UnityEngine;
using UnityEngine.Video;

public class videoController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if  (GetComponent<VideoPlayer>().isPaused)
            SlidesManager.Instance.NextScene();
    }
}
