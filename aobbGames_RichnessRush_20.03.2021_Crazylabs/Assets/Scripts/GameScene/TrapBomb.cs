using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBomb : MonoBehaviour
{
    public float delay;
    private float countdown;

    private bool hasExploded;

    public float burstRadius;

    public float force;

    LayerMask playerMask;

    private void Awake()
    {

        countdown = delay;
        hasExploded = false;

        playerMask = LayerMask.GetMask("Player");
    }

    Collider[] playersInBurstRadius;

    private void LateUpdate()
    {
        if (hasExploded)
        {
            return;
        }

        playersInBurstRadius = Physics.OverlapSphere(transform.position, burstRadius, playerMask, QueryTriggerInteraction.UseGlobal);

        foreach (Collider player in playersInBurstRadius)
        {
            player.GetComponent<PlayerPowersManager>().isInBlastRadius();
        }
    }

    private void Update()
    {
        countdown -= Time.deltaTime;        

        if (countdown<=0 && !hasExploded)
        {
            hasExploded = true;
            explode();
        }
    }

    private void explode()
    {
        //Visual
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }

        //Logic
        foreach (Collider player in playersInBurstRadius)
        {
            player.GetComponent<PlayerPowersManager>().caughtInBombExplosion();
        }


        GetComponent<AudioSource>().Play();
        //FindObjectOfType<AudioManager>().play("BombGoOff");

        //Destroy
        Destroy(gameObject, 2f);       
    }
}
