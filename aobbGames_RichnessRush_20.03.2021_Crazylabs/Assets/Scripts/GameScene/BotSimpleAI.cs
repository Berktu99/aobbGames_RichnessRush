using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSimpleAI : MonoBehaviour
{
    public List<Vector3> aiPathWaypoints;

    private static BotSimpleAI instance;

    public static BotSimpleAI getInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;

        aiPathWaypoints = new List<Vector3>();

        // Generate 4 way points

        aiPathWaypoints.Add(new Vector3(0f,0f,0f));
        aiPathWaypoints.Add(new Vector3(Random.Range(-8f, 15f), 0f, Random.Range(18f, 28f)));
        aiPathWaypoints.Add(new Vector3(Random.Range(-7f, 8f), 0f, Random.Range(40f, 50f)));
        aiPathWaypoints.Add(new Vector3(-18f, 0f, 65f));
        aiPathWaypoints.Add(new Vector3(-18f, 0f, 70f));
    }

}
