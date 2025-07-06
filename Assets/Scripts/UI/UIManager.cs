using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public UnityEvent InGameUI;
    public UnityEvent PauseUI;
    
    private VisualElement UxmlStart;
    private VisualElement UxmlGame;
    private VisualElement UxmlLoading;
    private VisualElement UxmlPause;

    [SerializeField] private UIDocument DOM;

    [SerializeField] private AudioSource playerAudio;

    private string sceneName;
    
    [SerializeField] public TemplateUIStart TUIStart;
    [SerializeField] public TemplateUIInGame TUIInGame;
    [SerializeField] public TemplateUIPause TUIPause;
    
    public PlayerInput uIInput;

    public void Awake()
    {
        DontDestroyOnLoad();

        VisualElement root = DOM.rootVisualElement;
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
        

        TUIStart.CloseTemplateStart.AddListener(StartDisableStartUI);

        Instance = this;

        uIInput.actions["Pause"].performed += ChangePause;
    }

    public void ChangePause(InputAction.CallbackContext uiContext)
    {
        PauseUI?.Invoke();
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
                UxmlPause.style.display = DisplayStyle.Flex;
                TUIPause.TInitInStart();
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
        if (Instance != null && Instance == this)
        {
            Debug.Log("Duplicated UI Instance found");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
}