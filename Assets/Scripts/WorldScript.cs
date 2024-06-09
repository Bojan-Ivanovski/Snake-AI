using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject game;

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            int X = UnityEngine.Random.Range(0, 15);
            int Y = UnityEngine.Random.Range(0, 7);
            Instantiate(game, new Vector3(X,Y,0), Quaternion.identity);
        }
    }



    void Update()
    {
    }
}
