using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Teleporter : MonoBehaviour
{
    private static Teleporter instance;

    public static Teleporter getInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        if(other.GetComponent<ThirdPersonCharController>().coinAmount > 500)
    //        {
    //            other.gameObject.transform.position = transform.GetChild(1).position;
    //        }                
    //    }
    //}
}
