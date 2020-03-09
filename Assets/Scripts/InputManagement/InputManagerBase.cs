using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// Controls all input and notifies subscribers about clicks or touches
    /// </summary>
    public abstract class InputManagerBase : MonoBehaviour
    {
        private readonly List<IInputSubscriber> subs = new List<IInputSubscriber>();

        protected bool IsNeedToCheckForInput = true;

        public void Update()
        {
            if (IsNeedToCheckForInput)
                CheckForInput();
        }

        protected abstract void CheckForInput();

        /// <summary>
        /// Sends input events to all subscribers
        /// </summary>
        /// <param name="inputPoint"></param>
        protected bool ProcessInput(Vector2 inputPoint)
        {
            var wp = Camera.main.ScreenToWorldPoint(inputPoint);
            var position = new Vector2(wp.x, wp.y);

            Collider2D hit = Physics2D.OverlapPoint(position);
            if (hit == null)
                return false;

            InputEvent inputEvent = new InputEvent { InputObject = hit.gameObject };
            foreach (IInputSubscriber subscriber in subs)
            {
                subscriber.Handle(inputEvent);
            }

            return true;
        }

        /// <summary>
        /// Subscribe to get input notifications
        /// </summary>
        /// <param name="sub"></param>
        public void Subscribe(IInputSubscriber sub)
        {
            subs.Add(sub);
        }
    }
}
