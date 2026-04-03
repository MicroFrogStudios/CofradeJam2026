using UnityEngine;

public class randomAnimations : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rn =Random.Range(0f, 1f);

        if (rn > 0.999f)
        {
            GetComponent<Animator>().SetTrigger("Callando");
            GetComponent<Animator>().SetBool("leyendo", true);
        }else if(rn < 0.001f)
        {
            
            GetComponent<Animator>().SetBool("leyendo", false);
        }
    }
}
