using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public UnityEvent InGameUI;
    
    private VisualElement UxmlStart;
    private VisualElement UxmlGame;
    private VisualElement UxmlLoading;
    private VisualElement UxmlPause;

    [SerializeField] private UIDocument DOM;

    [SerializeField] private AudioSource playerAudio;

    private string sceneName;
    
    [SerializeField] private TemplateUIStart TUIStart;
    [SerializeField] private TemplateUIInGame TUIInGame;
    [SerializeField] private TemplateUIPause TUIPause;
    
    

    public void Awake()
    {
        DontDestroyOnLoad();

        VisualElement root = DOM?.rootVisualElement;
        UxmlStart = root.Q<VisualElement>("TStartUI");
        UxmlGame = root.Q<VisualElement>("TGameUI");
        UxmlLoading = root.Q<VisualElement>("TLoadingUI");
        UxmlPause = root.Q<VisualElement>("TPauseUI");

        UxmlStart.style.display = DisplayStyle.None;
        UxmlGame.style.display = DisplayStyle.None;
        UxmlPause.style.display = DisplayStyle.None;
        
        DisplayUIByScene();
        
        TUIStart.Config(DOM, playerAudio);
        TUIInGame.Config(DOM, playerAudio);
        TUIPause.Config(DOM, playerAudio);
    }

    private void Start()
    {
        TUIStart.TInitStart();
        TUIInGame.TInitInStart();
        TUIPause.TInitInStart();

        TUIStart.CloseTemplateStart.AddListener(StartDisableStartUI);
    }

    private void DisplayUIByScene()
    {
        sceneName = SceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        switch (sceneName)
        {
            case "Menu":
                UxmlStart.style.display = DisplayStyle.Flex;
                break;
            case "Game":
                UxmlGame.style.display = DisplayStyle.Flex;
                Invoke("DelayInvokeEventGameMethod",1.0f);
                break;
        }
    }

    private void DelayInvokeEventGameMethod()
    {
        InGameUI?.Invoke();
    }
    public void StartDisableStartUI()
    {
        UxmlStart.style.display = DisplayStyle.None;
        DisplayUIByScene();
    }


    private void DontDestroyOnLoad()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
    }
}