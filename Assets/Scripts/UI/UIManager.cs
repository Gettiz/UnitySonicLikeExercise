using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    private VisualElement UxmlStart;
    private VisualElement UxmlGame;
    private VisualElement UxmlLoading;
    private VisualElement UxmlPause;
    
    [SerializeField] private UIDocument DOM;
    
    [SerializeField] private AudioSource playerAudio;

    [SerializeField] private TemplateUIStart TUIStart;
    [SerializeField] private TemplateUIInGame TUIInGame;
    
    public void Awake()
    {
        VisualElement root = DOM.rootVisualElement;
        UxmlStart = root.Q<VisualElement>("TStartUI");
        UxmlGame = root.Q<VisualElement>("TGameUI");
        UxmlLoading = root.Q<VisualElement>("TLoadingUI");
        UxmlPause = root.Q<VisualElement>("TPauseUI");
        
        DontDestroyOnLoad();

        TUIStart.Config(DOM, playerAudio);
        TUIInGame.Config(DOM, playerAudio);
    }
    private void Start()
    {
        TUIStart.TInitStart();
        TUIInGame.TInitInStart();

        TUIStart.CloseTemplateStart.AddListener(StartDisableStartUI);
    }
    public void StartDisableStartUI()
    {
        Invoke("DisableStartUI",2f);
    }

    private void DisableStartUI()
    {
        UxmlStart.style.display = DisplayStyle.None;
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