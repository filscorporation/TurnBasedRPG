using Assets.Scripts.PlayerManagement;

namespace Assets.Scripts.UIManagement
{
    /// <summary>
    /// Interface for classes that subscribe to UI input
    /// </summary>
    public interface IUISubscriber
    {
        /// <summary>
        /// Gets event from UI input
        /// </summary>
        /// <param name="uiEvent"></param>
        void Handle(UIEvent uiEvent);
    }
}
