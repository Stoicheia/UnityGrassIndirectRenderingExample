namespace MagicGrass.StarStrings
{
    public interface IInteractable
    {
        public void Interact(Interactor interactor);
        public string PromptText { get; }
    }
}