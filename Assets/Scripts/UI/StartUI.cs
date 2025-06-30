using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class StartUI : MonoBehaviour
{
    UIDocument DOM;
    private VisualElement UpperCoroutine;
    private VisualElement ButtomCoroutine;
    private VisualElement Logo;
    private Button Play;
    private Button TimeAttack;
    private Button Settings;
    private Button Exit;
    private void Awake()
    {
        DOM = GetComponent<UIDocument>();
        VisualElement root = DOM.rootVisualElement;
        
        UpperCoroutine = root.Q<VisualElement>("UpperCoroutine");
        ButtomCoroutine = root.Q<VisualElement>("ButtomCoroutine");
        Logo = root.Q<VisualElement>("Logo");
        Play = root.Q<Button>("PlayB");
        TimeAttack = root.Q<Button>("TimeAttackB");
        Settings = root.Q<Button>("SettingsB");
        Exit = root.Q<Button>("ExitB");
        
        //ScoreText = root.Q<Label>("Score");
        
        //GameOverText.visible = false;
        //PauseText.visible = false;
    }

    private void Start()
    {
        Invoke("CoroutineToClose",0.1f);
        Invoke("LogoToScreen",1.0f);
        Invoke("ButtonsToScreen",0.5f);

    }

    private void CoroutineToClose()
    {
        UpperCoroutine.AddToClassList("UpperCoroutineClose");
        ButtomCoroutine.AddToClassList("ButtomCoroutineClose");
    }

    private void LogoToScreen()
    {
        Logo.AddToClassList("LogoToScreen");
    }

    private void ButtonsToScreen()
    {
        Play.AddToClassList("LeftButtonsToScreen");
        TimeAttack.AddToClassList("LeftButtonsToScreen");
        Settings.AddToClassList("LeftButtonsToScreen");
        
        Exit.AddToClassList("RightButtonsToScreen");
    }
        void Update()
    {
        
    }
}
