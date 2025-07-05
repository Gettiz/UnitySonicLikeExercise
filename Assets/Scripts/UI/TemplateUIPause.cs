using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class TemplateUIPause : MonoBehaviour
    {
        UIDocument DOM;
        private AudioSource playerAudio;

        [SerializeField] private UIManager uIManagerForP;

        private VisualElement UxmlPause;
        private VisualElement PauseMenu;

        private Button ResumeButton;
        private Button SettingsButton;
        private Button MenuButton;
        
        public AudioClip HoverButtonPauseSfx;
        public AudioClip ClickButtonPauseSfx;
        public AudioClip GamePaused;
        public AudioClip GameUnpaused;
        
        private List<VisualElement> LTRMarqueePause;
        private List<VisualElement> RTLMarqueePause;
        private float speed = 100f;
        
        private bool IsPaused = false;
        private bool BackToMenuHasStarted = false;
        
        public void Config(UIDocument uiDocument, AudioSource audioSource)
        {
            DOM = uiDocument;
            playerAudio = audioSource;
            VisualElement root = DOM.rootVisualElement;
            
            UxmlPause = root.Q<VisualElement>("TPauseUI");
            
            PauseMenu = root.Q<VisualElement>("PauseMenu");
            
            ResumeButton = root.Q<Button>("ResumeButton");
            SettingsButton = root.Q<Button>("SettingsButton");
            MenuButton = root.Q<Button>("MenuButton");

            uIManagerForP.PauseUI?.AddListener(ChangePause);
            
            LTRMarqueePause = root.Query<VisualElement>(className: "LTRMarqueeEffectPause").ToList();
            RTLMarqueePause = root.Query<VisualElement>(className: "RTLMarqueeEffectPause").ToList();
            PauseMenu.style.visibility = Visibility.Hidden;
            
            ResumeButton.RegisterCallback<ClickEvent>(ResumeGame);
            ResumeButton.RegisterCallback<MouseEnterEvent>(HoverButtonSoundPause);
            
            SettingsButton.RegisterCallback<ClickEvent>(OpenSettings);
            SettingsButton.RegisterCallback<MouseEnterEvent>(HoverButtonSoundPause);
            
            MenuButton.RegisterCallback<ClickEvent>(BackMenu);
            MenuButton.RegisterCallback<MouseEnterEvent>(HoverButtonSoundPause);
        }

        public void TInitInStart()
        {
            Invoke("SetupMarqueesPause",0.5f);
        }
        
        private void SetupMarqueesPause()
        {
            foreach (var containerPause in LTRMarqueePause)
            {
                StartCoroutine(ScrollTextPause(containerPause));
            }

            foreach (var containerPause in RTLMarqueePause)
            {
                StartCoroutine(ScrollTextOppositePause(containerPause));
            }
        }
        
        private void ResumeGame(ClickEvent evt)
        {
            PauseMenu.style.visibility = Visibility.Hidden;
            IsPaused = false;
            ClickButtonSoundPause();
        }

        private void OpenSettings(ClickEvent evt)
        {
            ClickButtonSoundPause();
        }

        private void BackMenu(ClickEvent evt)
        {
            ClickButtonSoundPause();
            if (!BackToMenuHasStarted)
            {
                BackToMenuHasStarted = true;
                IsPaused = false;
                UxmlPause.style.display = DisplayStyle.None;
                SceneManager.LoadSceneAsync("Scenes/Menu");
            }
        }
        
        private void HoverButtonSoundPause(MouseEnterEvent evt)
        {
            playerAudio.PlayOneShot(HoverButtonPauseSfx, 1);
        }
        
        private void ClickButtonSoundPause()
        {
            playerAudio.PlayOneShot(ClickButtonPauseSfx, 1);
        }

        public void ChangePause()
        {
            Debug.Log("PauseInvokeMade");
            if (IsPaused)
            {
                PauseMenu.style.visibility = Visibility.Hidden;
                playerAudio.PlayOneShot(GameUnpaused,1);
                IsPaused = false;
            }
            else
            {
                PauseMenu.style.visibility = Visibility.Visible;
                playerAudio.PlayOneShot(GamePaused,1);
                IsPaused = true;
            }
        }
        
        IEnumerator ScrollTextPause(VisualElement containerPause)
        {
            float resetA = containerPause.resolvedStyle.width;

            while (true)
            {
                float a = containerPause.transform.position.x + Time.deltaTime * speed;
                if (a > resetA) a = 0;

                containerPause.transform.position = new Vector3(a, 0, 0);
                yield return null;
            }
        }

        IEnumerator ScrollTextOppositePause(VisualElement containerPause)
        {
            float resetB = -containerPause.resolvedStyle.width;

            while (true)
            {
                float b = containerPause.transform.position.x + Time.deltaTime * -speed;
                if (b < resetB) b = 0;

                containerPause.transform.position = new Vector3(b, 0, 0);
                yield return null;
            }
        }
    }
}