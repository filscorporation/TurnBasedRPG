using System;
using Assets.Scripts.MapManagement;
using UnityEngine;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Player or enemy in the game. Controlled by CharacterController
    /// </summary>
    public class Character : MapObject
    {
        public float MovingSpeed = 0.2F;
    }
}
