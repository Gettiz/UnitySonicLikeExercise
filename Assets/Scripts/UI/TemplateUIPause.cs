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
        
        private VisualElement Color;
        
        private bool IsPaused = false;
            
        public void Config(UIDocument uiDocument, AudioSource audioSource)
        {
            DOM = uiDocument;
            playerAudio = audioSource;
            VisualElement root = DOM.rootVisualElement;
            
            Color = root.Q<VisualElement>("ColorPause");
            

            uIManagerForP.PauseUI?.AddListener(ChangePause);
        }

        public void ChangePause()
        {
            Debug.Log("PauseInvokeMade");
            if (IsPaused)
            {
                Color.RemoveFromClassList("ChangeColorPause");
                IsPaused = false;
            }
            else
            {
                Color.AddToClassList("ChangeColorPause");
                IsPaused = true;
            }
        }
        public void TInitInStart()
        {
            //invokes
        }
    }

}
