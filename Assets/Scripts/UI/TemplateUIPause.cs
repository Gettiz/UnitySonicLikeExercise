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
            
        public void Config(UIDocument uiDocument, AudioSource audioSource)
        {
            DOM = uiDocument;
            playerAudio = audioSource;
            VisualElement root = DOM.rootVisualElement;
                
            //Events
        }
        public void TInitInStart()
        {
            //invokes
        }
    }

}
