using System;
using System.Collections.Generic;
using Assets.Scripts.Events;
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

        public GameObject EffectPrefab;
        private Canvas canvas;

        public void Start()
        {
            canvas = FindObjectOfType<Canvas>();
        }

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
                Vector2 p = Camera.main.WorldToScreenPoint(character.transform.position);
                GameObject o = Instantiate(EffectPrefab, p, Quaternion.identity, canvas.transform);
                Text text = o.transform.GetChild(0).gameObject.GetComponent<Text>();
                text.text = Mathf.RoundToInt(data.Damage.Value).ToString();
                Destroy(o, 2F);
            }
        }
    }
}
