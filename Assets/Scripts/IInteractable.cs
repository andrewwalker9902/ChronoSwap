public interface IInteractable
{
    void NotifyPress(string tag, float timestamp);
    void NotifyRelease(string tag);
}
