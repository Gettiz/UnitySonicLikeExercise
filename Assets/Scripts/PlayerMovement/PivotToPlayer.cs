using System;
using UnityEngine;

public class PivotToPlayer : MonoBehaviour
{
    public GameObject PivotPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.position = PivotPlayer.transform.position + PivotPlayer.transform.up;
    }
}
