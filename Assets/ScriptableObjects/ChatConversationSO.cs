using UnityEngine;

[CreateAssetMenu(fileName = "ChatConversationSO", menuName = "Scriptable Objects/ChatConversationSO")]
public class ChatConversationSO : ScriptableObject
{
    public string chatName;
    public string[] messagesArray;

    public int indexPointer = 0;

    public bool[] isLong;
    public bool HasNext()
    {
        return indexPointer < messagesArray.Length;
    }
    public (string, bool) GetNext()
    {
        var res  = (messagesArray[indexPointer] , isLong[indexPointer]);
        indexPointer++;
        return res;
    }

}
