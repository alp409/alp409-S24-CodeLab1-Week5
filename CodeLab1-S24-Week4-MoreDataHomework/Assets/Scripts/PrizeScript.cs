using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrizeScript : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        GameManager.instance.Score++;
        Debug.Log("Score++");
        transform.position = new Vector3(
           Random.Range(-10,10),
           Random.Range(-5, 5));
    }
}
