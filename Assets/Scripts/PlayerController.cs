using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject persistentUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (UIManager.Instance == null)
        {
            Instantiate(persistentUI);
        }

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
