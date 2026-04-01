using TMPro;
using UnityEngine;

public class toggleButton : MonoBehaviour
{

    private bool toggled = false;
    public string onString;
    public string offString;

    public void Toggle()
    {
        toggled = !toggled;
        
        GetComponentInChildren<TMP_Text>().text = toggled ? onString : offString;


    }



}
