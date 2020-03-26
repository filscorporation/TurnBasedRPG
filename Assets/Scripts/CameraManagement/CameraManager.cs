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
        public CameraFollow CameraFollow;

        private Transform mainTarget;
        private Transform subTarget;

        public void Start()
        {
            CameraFollow = Camera.main.gameObject.GetComponent<CameraFollow>();
            if (CameraFollow == null)
                throw new Exception("CameraFollow component on main camera required");
            
            mainTarget = FindObjectOfType<Player>().transform;
            CameraFollow.SetTarget(mainTarget);
        }

        /// <summary>
        /// Makes main camera to follow target
        /// </summary>
        /// <param name="target"></param>
        public void Follow(Transform target)
        {
            Debug.Log("Follow " + target);

            subTarget = target;
            CameraFollow.SetTarget(subTarget);
        }
    }
}
