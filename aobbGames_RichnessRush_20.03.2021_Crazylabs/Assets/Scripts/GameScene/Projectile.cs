using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float expireTime;

    private Quaternion direction;

    public int serialNumber;

    private void Start()
    {
        direction = transform.parent.localRotation;        
        transform.parent = null;
        transform.localRotation = direction;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (expireTime<=0)
        {
            Destroy(gameObject);
        }
        expireTime -= Time.deltaTime;

        GetComponent<Rigidbody>().velocity = transform.forward * speed * Time.deltaTime;
    }
}
