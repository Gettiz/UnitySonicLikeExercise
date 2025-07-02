using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
        public bool gameHasStarted = false;

        public void Config(UIDocument uiDocument, AudioSource audioSource)
        {
            DOM = uiDocument;
            playerAudio = audioSource;
            VisualElement root = DOM.rootVisualElement;

            ChangeLevelStart = root.Q<VisualElement>("ChangeLevelInGame");

            //Events
        }

        public void TInitInStart()
        {
            uIManager.InGameUI.AddListener(StartLevel);
            StartLevelUI.ChangeBoolInGame.AddListener(GameStarted);
            Invoke("StartLevel",0.1f);
        }

        private void GameStarted()
        {
            gameHasStarted = true;
        }
        private void StartLevel()
        {
            if (!levelHasStarted && gameHasStarted)
            {
                playerAudio.PlayOneShot(SoundTransitionStartSfx, 1);
                ChangeLevelStart.AddToClassList("ChangeLevelStartInGame");
                levelHasStarted = true;
            }
        }
    }
}