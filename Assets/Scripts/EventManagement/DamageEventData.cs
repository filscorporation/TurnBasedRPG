using System;
using Assets.Scripts.CharactersManagement;

namespace Assets.Scripts.EventManagement
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
