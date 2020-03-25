using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.CameraManagement
{
    /// <summary>
    /// Controlls camera
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;
        public static CameraManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<CameraManager>();
                return instance;
            }
        }

        public CameraFollow CameraFollow;

        private Queue<Transform> targets;

        public void Start()
        {
            CameraFollow = Camera.main.gameObject.GetComponent<CameraFollow>();
            if (CameraFollow == null)
                throw new Exception("CameraFollow component on main camera required");

            CameraFollow.OnNextTarget = NextTarget;
            CameraFollow.Target = FindObjectOfType<Player>().transform;

            targets = new Queue<Transform>();
        }

        /// <summary>
        /// Makes main camera to follow target
        /// </summary>
        /// <param name="target"></param>
        public void Follow(Transform target)
        {
            Debug.Log("Follow " + target);

            targets.Enqueue(target);

            if (!CameraFollow.HasTarget())
                NextTarget();
        }

        private void NextTarget()
        {
            if (targets.Any())
                CameraFollow.SetTarget(targets.Dequeue());
        }
    }
}
