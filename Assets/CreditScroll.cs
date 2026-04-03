using UnityEngine;

public class CreditScroll : MonoBehaviour
{

    public float speed = 50f;
    void Update()
    {
        GetComponent<RectTransform>().anchoredPosition += speed * Time.deltaTime * Vector2.up;
    }
}
