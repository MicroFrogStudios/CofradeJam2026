using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().anchoredPosition += 50f * Time.deltaTime * Vector2.up;
    }
}
