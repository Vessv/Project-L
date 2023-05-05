using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 targetPosition;
    Rigidbody2D rb;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {

        // Calcula la dirección de la posición A a la posición B
        Vector2 direction = targetPosition - (Vector2)transform.position;

        // Calcula el ángulo en radianes
        float angleRadians = Mathf.Atan2(direction.y, direction.x);

        // Convierte el ángulo en un quaternion
        Quaternion rotation = Quaternion.Euler(0, 0, angleRadians * Mathf.Rad2Deg - 94f);

        // Aplica la rotación al objeto
        transform.rotation = rotation;


        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;

       
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            Destroy(this.gameObject);
        }
    }
}
