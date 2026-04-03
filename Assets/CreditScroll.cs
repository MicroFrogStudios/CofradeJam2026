using System.Collections;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    float ogPos = -2815.52f;
    float endPos = 2858f;
    public float speed = 50f;
    bool ending = false;
    public Animator tiburon;
    void Update()
    {

        if (ending)
            return;

        if (GetComponent<RectTransform>().anchoredPosition.y < endPos)
        {
            GetComponent<RectTransform>().anchoredPosition += speed * Time.deltaTime * Vector2.up;
            return;
        }
        ending = true;
        StartCoroutine(EndingShush());


    }

    IEnumerator EndingShush()
    {
        tiburon.SetTrigger("Tiburon_Callando_01");
        yield return new WaitForSeconds(1.3f);
        SlidesManager.Instance.NextScene();

    }


}
