using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPowerUp : MonoBehaviour
{
    Transform gfx;

    public float  speed;

    private void Awake()
    {
        gfx = transform.GetChild(0);
    }

    private void Update()
    {
        simpleAnim();
    }
    private void simpleAnim()
    {
        gfx.Rotate(new Vector3(0f,speed,0f) * Time.deltaTime);
    }
   
}
