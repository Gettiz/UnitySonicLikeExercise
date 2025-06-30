using System.Collections;
using System.Collections.Generic;
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

    private List<VisualElement> LTRMarquee;
    private List<VisualElement> RTLMarquee;
    private List<Label> MarqueeLText;
    private List<Label> MarqueeRText;
    private float speed = 200f;
    private float resetA;
    private float resetB;


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

        LTRMarquee = root.Query<VisualElement>(className: "LTRMarqueeEffect").ToList();
        RTLMarquee = root.Query<VisualElement>(className: "RTLMarqueeEffect").ToList();
        MarqueeRText = root.Query<Label>(className: "MarqueeRText").ToList();
        MarqueeLText = root.Query<Label>(className: "MarqueeLText").ToList();

        //ScoreText = root.Q<Label>("Score");

        //GameOverText.visible = false;
        //PauseText.visible = false;
    }

    private void Start()
    {
        Invoke("CoroutineToClose", 0.1f);
        Invoke("LogoToScreen", 1.0f);
        Invoke("ButtonsToScreen", 0.5f);
        Invoke("SetupMarquees", 0.1f);
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

    private void SetupMarquees()
    {
        foreach (var container in LTRMarquee)
        {
            Label Text = container.Q<Label>(className: "MarqueeRText");
            StartCoroutine(ScrollText(container, Text));
        }

        foreach (var container in RTLMarquee)
        {
            Label Text = container.Q<Label>(className: "MarqueeLText");
            StartCoroutine(ScrollTextOpposite(container, Text));
        }
    }

    IEnumerator ScrollText(VisualElement container, Label Text)
    {
        resetA = container.resolvedStyle.width;


        while (true)
        {
            float a = container.transform.position.x + Time.deltaTime * speed;
            if (a > resetA) a = -container.resolvedStyle.width;
            
            container.transform.position = new Vector3(a, 0, 0);

            yield return null;
        }
    }

    IEnumerator ScrollTextOpposite(VisualElement container, Label Text)
    {
        resetB = container.resolvedStyle.width;
        while (true)
        {
            float b = container.transform.position.x + Time.deltaTime * -speed;
            if (b < -resetB) b = container.resolvedStyle.width;

            container.transform.position = new Vector3(b, 0, 0);
            yield return null;
        }
    }

    void Update()
    {
    }
}