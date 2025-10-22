using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 2f;
    protected bool canMove = false; 

    void Update()
    {
        if (canMove)
        {
           
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
        }

        
        if (transform.position.z < Camera.main.transform.position.z - 20f)
        {
            Destroy(gameObject);
        }
    }
}
