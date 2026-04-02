using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneController : MonoBehaviour
{

    public GameObject IncomingTextPrefab;
    public Transform ContentContainer;
    public GameObject VoiceMessagePrefab;

    public void AddChatMessage(string text)
    {
       GameObject MessageInstance =  Instantiate(IncomingTextPrefab,ContentContainer);
       MessageInstance.GetComponentInChildren<TMP_Text>().text = text;
        AudioManager.instance.Play("noti1");
    }
   
}
