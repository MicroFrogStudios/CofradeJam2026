using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Transform endPoint;
    public float speed = 1f;
    public float followDistance = 0.5f;

    public Transform nazareno1;
    public Transform nazareno2;
    public Transform paso;

    private bool move = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Move"))
        {
            move = true;
        }
    }

    private void Update()
    {
        if (!move) return;

        if (paso != null)
        {
            paso.position = Vector2.MoveTowards(paso.position, endPoint.position, speed * Time.deltaTime);
        }

        if (nazareno2 != null && paso != null)
        {
            if (Vector2.Distance(nazareno2.position, paso.position) > followDistance)
            {
                nazareno2.position = Vector2.MoveTowards(nazareno2.position, paso.position, speed * Time.deltaTime);
            }
        }

        if (nazareno1 != null && nazareno2 != null)
        {
            if (Vector2.Distance(nazareno1.position, nazareno2.position) > followDistance)
            {
                nazareno1.position = Vector2.MoveTowards(nazareno1.position, nazareno2.position, speed * Time.deltaTime);
            }
        }
    }
}