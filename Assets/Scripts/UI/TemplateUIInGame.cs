using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class TemplateUIInGame : MonoBehaviour
    {
        [SerializeField] private UIManager uIManager;
        [SerializeField] private TemplateUIStart StartLevelUI;
        
        UIDocument DOM;
        private AudioSource playerAudio;

        private VisualElement ChangeLevelStart;

        public AudioClip SoundTransitionStartSfx;
        
        private bool levelHasStarted = false;
        
        //public static TemplateUIInGame Instance { get; private set; }
        public bool gameHasStarted = false;
        

        public void Config(UIDocument uiDocument, AudioSource audioSource)
        {
            DOM = uiDocument;
            playerAudio = audioSource;
            VisualElement root = DOM.rootVisualElement;

            ChangeLevelStart = root.Q<VisualElement>("ChangeLevelInGame");

            //Instance = this;

            StartLevelUI.ChangeBoolInGame?.AddListener(GameStarted);
            uIManager.InGameUI?.AddListener(StartLevel);
        }

        public void TInitInStart()
        {
            
        }
        
        public void SetGameStartedFlag(bool started)
        {
            gameHasStarted = started;
        }

        private void GameStarted()
        {
            gameHasStarted = true;
        }
        
        private void StartLevel()
        {
            if (!levelHasStarted && gameHasStarted)
            {
                levelHasStarted = true;
                playerAudio.PlayOneShot(SoundTransitionStartSfx, 1);
                ChangeLevelStart.AddToClassList("ChangeLevelStartInGame");
            }
        }
    }
}