using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorDie : MonoBehaviour
{
    public void Die()
    {
        
        Destroy(transform.root.gameObject);
    }
}
