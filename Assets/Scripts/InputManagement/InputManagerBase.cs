using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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
            if (IsPointerOverUIObject(inputPoint))
                // Ignore input when on UI
                return false;

            var wp = Camera.main.ScreenToWorldPoint(inputPoint);
            var position = new Vector2(wp.x, wp.y);

            if (EventSystem.current.currentSelectedGameObject != null)
                return false;
            Collider2D[] hits = Physics2D.OverlapPointAll(position);
            if (hits == null || !hits.Any())
                return false;
            // TODO: process multiple hits
            Collider2D hit = hits.First();

            InputEvent inputEvent = new InputEvent { InputObject = hit.gameObject };
            foreach (IInputSubscriber subscriber in subs)
            {
                subscriber.Handle(inputEvent);
            }

            return true;
        }

        private bool IsPointerOverUIObject(Vector2 inputPoint)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = inputPoint;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
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
