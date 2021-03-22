using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public float radius;
    private LayerMask ground;
    public float fallSpeed;

    private void Awake()
    {
        ground = 10;
    }

    //private void FixedUpdate()
    //{
    //    if( Physics.CheckSphere(transform.position, radius, ground))
    //    {
    //        Debug.Log("is in ground");

    //    }
    //    else
    //    {
    //        Debug.Log("NOT GROUNDED");
    //        transform.localPosition -= new Vector3(0f, fallSpeed * Time.deltaTime ,0f);
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NormalGround") || collision.gameObject.CompareTag("WaterGround"))
        {
            if (gameObject.GetComponent<Rigidbody>() != null)
            {
                //Debug.Log("HEY");
                Destroy(GetComponent<Rigidbody>());

                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.layer = 12;
                gameObject.tag = "Coin";

                Destroy(gameObject.GetComponent<CoinManager>());
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("NormalGround") || other.CompareTag("WaterGround"))
    //    {
    //        if (gameObject.GetComponent<Rigidbody>() != null)
    //        {
    //            Debug.Log("HEY");
    //            Destroy(GetComponent<Rigidbody>());
    //        }
    //    }
    //}

}
