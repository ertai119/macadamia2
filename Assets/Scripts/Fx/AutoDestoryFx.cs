using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestoryFx : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration); 
    }
}
