using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.EnemyManagement
{
    /// <summary>
    /// Controls enemy behavour
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        public MapManager MapManager;
        public BattleManager BattleManager;

        public List<Enemy> Enemies;

        public GameObject WarningEffectPrefab;

        public void Start()
        {
            Validate();
            LoadAllEnemies();
        }

        private void Validate()
        {
            if (MapManager == null)
                throw new Exception("MapManager field should not be null");
            if (BattleManager == null)
                throw new Exception("BattleManager field should not be null");
        }

        private void LoadAllEnemies()
        {
            Enemies = FindObjectsOfType<Enemy>().ToList();
        }

        /// <summary>
        /// Check if player in any enemies line of sight to start battle
        /// </summary>
        /// <param name="player"></param>
        public void CheckIfStartBattle(Player player)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.InSight(player))
                    BattleManager.StartBattleFromEnemyAttack(enemy);
            }
        }

        /// <summary>
        /// Gets all enemies in sight of one, including it
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public List<Enemy> GetEnemiesInSight(Enemy enemy)
        {
            List<Enemy> result = new List<Enemy> { enemy };
            foreach (Enemy e in Enemies)
            {
                if (enemy.InSight(e))
                    result.Add(e);
            }
            return result;
        }

        /// <summary>
        /// Makes all preparation before battle for enemies
        /// </summary>
        /// <param name="enemies"></param>
        public void PutEnemiesToBattle(List<Enemy> enemies)
        {
            foreach (Enemy enemy in enemies)
            {
                GameObject eff = Instantiate(WarningEffectPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
                Destroy(eff, 3F);
            }
        }
    }
}
