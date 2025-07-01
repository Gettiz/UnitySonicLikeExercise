using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument DOM;
    
    [SerializeField] private AudioSource playerAudio;

    [SerializeField] private TemplateUIStart TUIStart;
    
    
    private void Awake()
    {
        TUIStart.Config(DOM,playerAudio);
    }

    private void Start()
    {
        TUIStart.TInitStart();
        
    }

    
    void Update()
    {
       
    }
}