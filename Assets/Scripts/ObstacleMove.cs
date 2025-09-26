using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float speed = 10f;
    protected bool canMove = false; 

    void Update()
    {
        if (canMove)
        {
            
            transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        }

        
        if (transform.position.z < Camera.main.transform.position.z - 20f)
        {
            Destroy(gameObject);
        }
    }
}
