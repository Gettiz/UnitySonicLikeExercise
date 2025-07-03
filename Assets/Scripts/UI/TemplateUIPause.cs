using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class TemplateUIPause : MonoBehaviour
    {
        UIDocument DOM;
        private AudioSource playerAudio;

        [SerializeField] private UIManager uIManagerForP;

        private VisualElement PauseMenu;
        
        private List<VisualElement> LTRMarqueePause;
        private List<VisualElement> RTLMarqueePause;
        private float speed = 200f;
        
        private bool IsPaused = false;

        public void Config(UIDocument uiDocument, AudioSource audioSource)
        {
            DOM = uiDocument;
            playerAudio = audioSource;
            VisualElement root = DOM.rootVisualElement;

            PauseMenu = root.Q<VisualElement>("PauseMenu");

            uIManagerForP.PauseUI?.AddListener(ChangePause);
            
            
            
            LTRMarqueePause = root.Query<VisualElement>(className: "LTRMarqueeEffectPause").ToList();
            RTLMarqueePause = root.Query<VisualElement>(className: "RTLMarqueeEffectPause").ToList();
            PauseMenu.style.visibility = Visibility.Hidden;
        }

        public void ChangePause()
        {
            Debug.Log("PauseInvokeMade");
            if (IsPaused)
            {
                PauseMenu.style.visibility = Visibility.Hidden;
                IsPaused = false;
            }
            else
            {
                PauseMenu.style.visibility = Visibility.Visible;
                IsPaused = true;
            }
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