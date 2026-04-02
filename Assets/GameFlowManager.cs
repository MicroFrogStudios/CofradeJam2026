using System;
using UnityEngine;
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
    public float idleTimeoutTime = 13f;
    public float startIdleTime;
    public bool idle = true;
    public int timeoutCount = 0;

    public Animator[] shushers;
    public Animator Shark;
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
        
    }

    void EndChatEvent()
    {
        //give control back
        ActiveChat.indexPointer = 0;
        ActiveChat = null;
        startIdleTime = Time.time;
        idle = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startIdleTime + idleTimeoutTime < Time.time && idle)
        {
            timeoutCount++;
            idle = false;
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
}
