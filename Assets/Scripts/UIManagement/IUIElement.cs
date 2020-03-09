namespace Assets.Scripts.UIManagement
{
    /// <summary>
    /// Controls real UI element and changes it
    /// </summary>
    public interface IUIElement
    {
        string Name { get; }

        void SetValue(int value);

        void SetValue(string value);
    }
}
