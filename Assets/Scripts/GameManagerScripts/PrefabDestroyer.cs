using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDestroyer : MonoBehaviour
{
    public float timeBeforeDelete = 0.5f;


    private void Update()
    {
        if(timeBeforeDelete <= 0) Destroy(gameObject);
        timeBeforeDelete = timeBeforeDelete - Time.deltaTime;
    }
}
