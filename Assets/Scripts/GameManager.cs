using Assets.Scripts.SkillManagement;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Controles all game processes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public void Awake()
        {
            LoadSkills();
        }

        private void LoadSkills()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Skill)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Skill))))
            {
                Skill skill = (Skill)Activator.CreateInstance(type);
                Skill.SkillDictionary[skill.Name] = skill;
            }
        }
        
        //private void GenerateField(int x, int y)
        //{
        //    for (int i = 0; i < x; i++)
        //    {
        //        for (int j = 0; j < y; j++)
        //        {
        //            GameObject go = Instantiate(TileGO, new Vector3(i - x/2, j - y/2), Quaternion.identity, Map.transform);
        //            Tile tile = go.GetComponent<Tile>();
        //            tile.X = i;
        //            tile.Y = j;
        //        }
        //    }
        //}
    }
}
