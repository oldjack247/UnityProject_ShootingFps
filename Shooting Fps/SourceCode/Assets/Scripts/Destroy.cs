using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public Transform attackPoint;
    public bool isPlayerFlash;

    private void Awake()
    {
        attackPoint = GameObject.Find("AttackPoint").transform;
    }

    void Start()
    {
        Invoke("Destroyy", 1f);    
    }

    private void Update()
    {
        if(isPlayerFlash)
            transform.position = attackPoint.position;
    }

    void Destroyy()
    {
        Destroy(gameObject);
    }
}
