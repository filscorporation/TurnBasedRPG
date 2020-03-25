using System;
using System.Collections.Generic;
using Assets.Scripts.EventManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Controls damage effects that are showing up when any character takes damage
    /// </summary>
    public class DamageValueEffectController : MonoBehaviour
    {
        private static DamageValueEffectController instance;
        public static DamageValueEffectController Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<DamageValueEffectController>();
                return instance;
            }
        }

        public Transform DamageEffectsParent;
        public GameObject EffectPrefab;

        /// <summary>
        /// Creates effects of damage takken when this characters take damage
        /// </summary>
        /// <param name="characters"></param>
        public void AddToShowEffectList(IEnumerable<Character> characters)
        {
            foreach (Character character in characters)
            {
                character.OnCharacterTakeDamage += ShowEffect;
            }
        }

        private void ShowEffect(object sender, EventArgs e)
        {
            if (e is DamageEventData data && sender is Character character)
            {
                Vector3 p = Camera.main.WorldToScreenPoint(character.transform.position);
                p = new Vector3(p.x, p.y, -100);
                GameObject o = Instantiate(EffectPrefab, p, Quaternion.identity, DamageEffectsParent);
                Text text = o.transform.GetChild(0).gameObject.GetComponent<Text>();
                text.text = Mathf.RoundToInt(data.Damage.Value).ToString();
                Destroy(o, 2F);
            }
        }
    }
}
