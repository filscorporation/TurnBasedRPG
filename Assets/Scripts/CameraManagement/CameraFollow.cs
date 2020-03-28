using System;
using UnityEngine;

namespace Assets.Scripts.CameraManagement
{
    /// <summary>
    /// Script to folow position
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        public bool NeedToFollow = true;
        
        public Transform Target;
        private const float borderX = 0.25F;
        private const float borderY = 0.35F;
        private const float cameraSpeed = 2F;
        private const float cameraZ = -10F;
        private const float eps = 0.1F;
        private bool hasNextTarget = false;
        private Vector2 nextTarget;

        public void LateUpdate()
        {
            if (Target != null && NeedToFollow)
                FollowTarget();
        }

        private void FollowTarget()
        {
            if (hasNextTarget)
            {
                if (Vector2.Distance(transform.position, nextTarget) < eps)
                {
                    hasNextTarget = false;
                    return;
                }

                Vector2 tp = Vector2.Lerp(transform.position, nextTarget, Time.deltaTime * cameraSpeed);
                transform.position = new Vector3(tp.x, tp.y, cameraZ);
            }

            CheckIfNeedToFollow();
        }

        private void CheckIfNeedToFollow()
        {
            Vector2 p = Target.position;
            Vector2 sp = Camera.main.WorldToScreenPoint(p);
            if (sp.x < Screen.width * borderX || sp.x > Screen.width * (1 - borderX)
            || sp.y < Screen.height * borderY || sp.y > Screen.height * (1 - borderY))
            {
                hasNextTarget = true;
                nextTarget = p;
            }
        }

        public void SetTarget(Transform target)
        {
            this.Target = target;
            CheckIfNeedToFollow();
        }
    }
}
