using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeEvents : MonoBehaviour
{
    public GameObject particleEffect;

    public void EnableParticle()
    {
        particleEffect.SetActive(true);
    }

    public void DisableParticle()
    {
        particleEffect.SetActive(false);
    }
    public void Die()
    {

        Destroy(transform.root.gameObject);
    }
}
