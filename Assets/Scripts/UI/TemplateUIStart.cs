using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class TemplateUIStart : MonoBehaviour
    {
        UIDocument DOM;
        private AudioSource playerAudio;
        
        private VisualElement UpperCoroutine;
        private VisualElement ButtomCoroutine;
        private VisualElement Logo;
        private VisualElement Vignette;
        private VisualElement ChangeLevelStart;
        bool ChangeLevelStarted = false;

        private Button Play;
        private Button TimeAttack;
        private Button Settings;
        private Button Exit;

        private List<VisualElement> LTRMarquee;
        private List<VisualElement> RTLMarquee;
        private float speed = 200f;

        public AudioClip HoverButtonSfx;
        public AudioClip ClickButtonSfx;
        public AudioClip SoundTransitionSfx;
        public AudioClip SoundTransitionFinishSfx;
        
        public UnityEvent CloseTemplateStart;
        
        public void Config(UIDocument uiDocument, AudioSource audioSource)
        {
            DOM = uiDocument;
            playerAudio = audioSource;
            VisualElement root = DOM.rootVisualElement;

            UpperCoroutine = root.Q<VisualElement>("UpperCoroutine");
            ButtomCoroutine = root.Q<VisualElement>("ButtomCoroutine");
            Logo = root.Q<VisualElement>("Logo");
            Vignette = root.Q<VisualElement>("Vignette");
            ChangeLevelStart = root.Q<VisualElement>("ChangeLevelStart");

            Play = root.Q<Button>("PlayB");
            TimeAttack = root.Q<Button>("TimeAttackB");
            Settings = root.Q<Button>("SettingsB");
            Exit = root.Q<Button>("ExitB");

            LTRMarquee = root.Query<VisualElement>(className: "LTRMarqueeEffect").ToList();
            RTLMarquee = root.Query<VisualElement>(className: "RTLMarqueeEffect").ToList();

            Play.RegisterCallback<ClickEvent>(PlayButton);
            Play.RegisterCallback<MouseEnterEvent>(HoverButtonSound);

            TimeAttack.RegisterCallback<ClickEvent>(TimeAttackButton);
            TimeAttack.RegisterCallback<MouseEnterEvent>(HoverButtonSound);

            Settings.RegisterCallback<ClickEvent>(SettingsButton);
            Settings.RegisterCallback<MouseEnterEvent>(HoverButtonSound);

            Exit.RegisterCallback<ClickEvent>(ExitButton);
            Exit.RegisterCallback<MouseEnterEvent>(HoverButtonSound);
        }
        
        public void TInitStart()
        {
            Invoke("CoroutineToClose", 0.1f);
            Invoke("LogoToScreen", 1.0f);
            Invoke("ButtonsToScreen", 0.5f);
            Invoke("SetupMarquees", 0.1f);
            Invoke("ShowVignette", 0.1f);
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
                StartCoroutine(ScrollText(container));
            }

            foreach (var container in RTLMarquee)
            {
                StartCoroutine(ScrollTextOpposite(container));
            }
        }

        private void ShowVignette()
        {
            Vignette.RemoveFromClassList("ShowVignette");
        }

        IEnumerator ScrollText(VisualElement container)
        {
            float resetA = container.resolvedStyle.width;

            while (true)
            {
                float a = container.transform.position.x + Time.deltaTime * speed;
                if (a > resetA) a = 0;

                container.transform.position = new Vector3(a, 0, 0);
                yield return null;
            }
        }

        IEnumerator ScrollTextOpposite(VisualElement container)
        {
            float resetB = -container.resolvedStyle.width;

            while (true)
            {
                float b = container.transform.position.x + Time.deltaTime * -speed;
                if (b < resetB) b = 0;

                container.transform.position = new Vector3(b, 0, 0);
                yield return null;
            }
        }

        private void HoverButtonSound(MouseEnterEvent evt)
        {
            playerAudio.PlayOneShot(HoverButtonSfx, 1);
        }

        private void ClickButtonSound()
        {
            playerAudio.PlayOneShot(ClickButtonSfx, 1);
        }

        private void PlayButton(ClickEvent evt)
        {
            ClickButtonSound();
            ChangeLevel();
        }

        private void ChangeLevel()
        {
            if (!ChangeLevelStarted)
            {
                ChangeLevelStarted = true;
                ChangeLevelStart.AddToClassList("ChangeLevelStart");
                playerAudio.PlayOneShot(SoundTransitionSfx, 1);
                var loadingOperation = SceneManager.LoadSceneAsync("Scenes/Game");
                loadingOperation.completed += _ => ChangeLevelStart.RegisterCallbackOnce<TransitionEndEvent>(ChangeLevelStartEnd);
            }
        }

        private void ChangeLevelStartEnd(TransitionEndEvent evt)
        {
            CloseTemplateStart?.Invoke();
        }

        /*private void StartLevel()
        {
            ChangeLevelStart.RemoveFromClassList("ChangeLevelStart");
            playerAudio.PlayOneShot(SoundTransitionFinishSfx, 1);
        }*/
        
        private void TimeAttackButton(ClickEvent evt)
        {
            ClickButtonSound();
        }

        private void SettingsButton(ClickEvent evt)
        {
            ClickButtonSound();
        }

        private void ExitButton(ClickEvent evt)
        {
            ClickButtonSound();
        }

        
    }
}