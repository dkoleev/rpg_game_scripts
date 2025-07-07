namespace Darkness.Runtime.Logic
{
    public interface IInteractable
    {
        public string PromptMessage { get; }
        void Interact();
    }
}