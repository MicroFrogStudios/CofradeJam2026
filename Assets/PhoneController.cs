using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhoneController : MonoBehaviour
{

    public GameObject IncomingTextLongPrefab;
    public GameObject IncomingTextShortPrefab;
    public Transform ContentContainer;
    public GameObject VoiceMessagePrefab;
    [HideInInspector]
    public int audioCounter = 0;

    public void AddChatMessage(string text, bool isLong)
    {
        GameObject obj = isLong ? IncomingTextLongPrefab : IncomingTextShortPrefab;
        GameObject MessageInstance =  Instantiate(obj, ContentContainer);
        MessageInstance.GetComponentInChildren<TMP_Text>().text = text;
        AudioManager.instance.Play("noti1");
        GetComponentInChildren<ScrollRect>().content.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 200);
    }

    public void AddVoiceMessage(bool tooQuiet)
    {
        AudioManager.instance.Play("noti2");
        Instantiate(VoiceMessagePrefab, ContentContainer);
        if (tooQuiet)
        {
            GameFlowManager.Instance.AddChatEvent("tooQuiet");
        }
        else
        {
            audioCounter++;
            GameFlowManager.Instance.AddChatEvent("afterAudio" + audioCounter.ToString());
            if (audioCounter >= 5)
            {
                GameFlowManager.Instance.SiluroEnding();
            }
        }
    }


}
