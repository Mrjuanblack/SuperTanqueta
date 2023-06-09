using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSound : MonoBehaviour
{
    private float lifeTime;
    private float maxLifeTime;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        maxLifeTime = (float)(audioSource.clip.length + 0.2);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime >= maxLifeTime){
            Destroy(gameObject);
        }        
    }
}
