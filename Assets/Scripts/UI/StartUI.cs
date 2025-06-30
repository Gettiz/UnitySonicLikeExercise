using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class StartUI : MonoBehaviour
{
    UIDocument DOM;
    private VisualElement UpperCoroutine;
    private VisualElement ButtomCoroutine;
    
    private void Awake()
    {
        DOM = GetComponent<UIDocument>();
        VisualElement root = DOM.rootVisualElement;
        
        UpperCoroutine = root.Q<VisualElement>("UpperCoroutine");
        ButtomCoroutine = root.Q<VisualElement>("ButtomCoroutine");

        //ScoreText = root.Q<Label>("Score");
        
        //GameOverText.visible = false;
        //PauseText.visible = false;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        CoroutineToClose();
    }

    private void CoroutineToClose()
    {
        UpperCoroutine.AddToClassList("UpperCoroutineClose");
        ButtomCoroutine.AddToClassList("ButtomCoroutineClose");
    }

    void Update()
    {
        
    }
}
