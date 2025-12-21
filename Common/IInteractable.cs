using UnityEngine;

namespace JYW.Game.Commons
{
    [System.Serializable]
    public class AimingIntractionPrompt
    {
        public string promptText;
        public KeyCode interactionKey = KeyCode.E;
    }
    public interface IInteractable
    {
        public abstract AimingIntractionPrompt AimingIntractionPrompt { get; set; }
        public abstract void Interact();


    }
}