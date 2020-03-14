using Assets.Scripts.CharactersManagement;
using System;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Info about damage character took
    /// </summary>
    public class DamageEventData : EventArgs
    {
        public Damage Damage;

        public DamageEventData(Damage damage)
        {
            Damage = damage;
        }
    }
}
