public interface Interactable
{
    void StartInteract();
    void StopInteract();
    bool IsInteractable { get; }

    void Highlight();
    void Unhighlight();
}