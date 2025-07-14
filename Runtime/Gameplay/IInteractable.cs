namespace Darkness.Runtime.Gameplay {
    public interface IInteractable {
        public string PromptMessage { get; }
        void Interact();
    }
}