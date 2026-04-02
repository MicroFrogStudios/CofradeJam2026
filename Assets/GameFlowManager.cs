using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameFlowManager : MonoBehaviour
{
    public ChatConversationSO[] Chats;
    public PhoneController phone;
    [HideInInspector]
    public ChatConversationSO ActiveChat;
    public float messageDelay = 1.2f;
    public float timePassed = 0;
    public static GameFlowManager Instance;
    public float idleTimeoutTime = 20f;
    public float startIdleTime;
    public bool idle = true;
    public int timeoutCount = 0;

    public Animator[] shushers;
    public Animator Shark;
    public Animator besugo;
    public Animator gameOverAnim;

    public GameObject GameOverUI;
    public void AddChatEvent(string chatName)
    {
        ActiveChat = Array.Find(Chats, chat => chat.chatName == chatName);
    }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AddChatEvent("start");
        startIdleTime = Time.time;

    }

    void EndChatEvent()
    {
        //give control back
        ActiveChat.indexPointer = 0;
        ActiveChat = null;
        startIdleTime = Time.time;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startIdleTime + idleTimeoutTime < Time.time && idle)
        {
            timeoutCount++;
            startIdleTime = Time.time;
            if (timeoutCount >= 4)
                return;
            AddChatEvent("timeout" + timeoutCount.ToString());
        }
            

        if (ActiveChat == null)
            return;

        timePassed += Time.deltaTime;
        if (timePassed > messageDelay )
        {
            timePassed = 0;
            if (ActiveChat.HasNext())
            {
                (string message, bool isLong ) = ActiveChat.GetNext();
                Debug.Log(message);
                phone.AddChatMessage(message,isLong );
                return;
            }

            EndChatEvent();
        }
    }

    public void SharkGameOver()
    { 
        StartCoroutine(animationAndFade());
    }

    IEnumerator animationAndFade()
    {
        AudioSource[] music = Camera.main.GetComponents<AudioSource>();
        foreach (AudioSource audio in music)
            audio.Stop(); 
        
        SlidesManager.Instance.StartCoroutine(SlidesManager.Instance.FadeOut());
        yield return new WaitForSeconds(2);
        SlidesManager.Instance.cinematicBars.Play();
        besugo.gameObject.SetActive(false);
        Shark.gameObject.SetActive(false);
        SlidesManager.Instance.StartCoroutine(SlidesManager.Instance.FadeIn());
        Camera.main.GetComponent<Animation>().Play();
        gameOverAnim.gameObject.SetActive(true);
        StartCoroutine(WaitForAnimation());
    }
    IEnumerator WaitForAnimation()
    {
        yield return new WaitUntil(() => gameOverAnim.GetCurrentAnimatorStateInfo(0).IsName("Animation_GameOverLast"));
        GameOverUI.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
