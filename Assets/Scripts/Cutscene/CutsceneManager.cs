using System;
using Cinemachine;
using UnityEngine;

namespace Cutscene
{
    public class CutsceneManager : MonoBehaviour
    {
        public Animator Animator;
        
        public static CutsceneManager Instance;

        private bool isShowing;
        private Action shownAction;
        
        private void Awake()
        {
            Instance = this;
        }

        public void Show(string key, Action onShown = null)
        {
            if (isShowing) return;

            isShowing = true;
            shownAction = onShown;
            Animator.Play(key);
        }

        // Animation event
        void Action()
        {
            shownAction?.Invoke();
        }
        
        void Finished()
        {
            isShowing = false;
        }
    }
}