using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.EnemyManagement
{
    /// <summary>
    /// Controls enemy behavour
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        public BattleManager BattleManager;

        public List<Enemy> Enemies;

        private Queue<Enemy> enemiesInTurn;
        private Battle currentButtle;

        public GameObject WarningEffectPrefab;

        private Action callWhenEnemyTurnDone;

        public void Start()
        {
            Validate();
            LoadAllEnemies();
        }

        private void Validate()
        {
            if (BattleManager == null)
                throw new Exception("BattleManager field should not be null");
        }

        private void LoadAllEnemies()
        {
            Enemies = FindObjectsOfType<Enemy>().ToList();
        }

        /// <summary>
        /// Passes control to enemy
        /// </summary>
        /// <param name="onEnemyTurnDone">After enemy turn done - pass control to battle manager</param>
        /// <param name="battle"></param>
        public void EnemyTurn(Action onEnemyTurnDone, Battle battle)
        {
            Debug.Log("Starting enemy turn");

            callWhenEnemyTurnDone = onEnemyTurnDone;

            // Sort all enemies by priority and put them into queue to act
            currentButtle = battle;
            enemiesInTurn = new Queue<Enemy>(currentButtle.Enemies.OrderBy(e => e.Priority(currentButtle)));

            ProcessEnemyFromQueue();
        }

        private void ProcessEnemyFromQueue()
        {
            if (!enemiesInTurn.Any())
            {
                // All enemies acted
                FinishEnemyTurn();
                return;
            }

            Enemy enemy = enemiesInTurn.Dequeue();
            if (enemy == null)
            {
                // Enemy probably died
                return;
            }

            enemy.Act(currentButtle, EnemyFinishedActing);
        }

        private void EnemyFinishedActing()
        {
            ProcessEnemyFromQueue();
        }

        private void FinishEnemyTurn()
        {
            Debug.Log("Enemy turn finished");
            // Do all turn end stuff
            foreach (Enemy enemy in currentButtle.Enemies)
            {
                enemy.ActionPoints = enemy.ActionPointsMax;
            }
            // Ready to pass the turn to player
            callWhenEnemyTurnDone();
        }

        /// <summary>
        /// Check if player in any enemies line of sight to start battle
        /// </summary>
        /// <param name="player"></param>
        public bool CheckIfStartBattle(Player player)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.InSight(player))
                {
                    BattleManager.StartBattleFromEnemyAttack(enemy);
                    return true;
                }
            }

            return false;
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

                enemy.Healthbar.Show();
            }
        }
    }
}
