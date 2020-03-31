using Assets.Scripts.MapManagement;
using Assets.Scripts.RoomsManagement;
using UnityEngine;

namespace Assets.Scripts.InteractableObjects
{
    /// <summary>
    /// Entance to next level on the map
    /// </summary>
    public class Entrance : InteractableObject
    {
        public Sprite EntranceIcon;
        protected override Sprite ButtonIcon => EntranceIcon;

        public object RoomsManager { get; private set; }

        /// <summary>
        /// Index of the room where this entrance leaves.
        /// Zero if there is none for now (will be generated)
        /// </summary>
        public int RoomToIndex = 0;

        /// <summary>
        /// Part of the room where this entrance is placed
        /// </summary>
        public Direction Direction;

        protected override void Interact()
        {
            RoomManager.Instance.MoveToNextRoom(this);
        }
    }
}
