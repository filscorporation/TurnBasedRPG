﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
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
        private Battle currentBattle;

        public GameObject WarningEffectPrefab;

        private Action<bool> callWhenEnemyTurnDone;

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
        public void EnemyTurn(/*Action<bool> onEnemyTurnDone, Battle battle*/)
        {
            Debug.Log("Starting enemy turn");

            //callWhenEnemyTurnDone = onEnemyTurnDone;

            // Sort all enemies by priority and put them into queue to act
            enemiesInTurn = new Queue<Enemy>(currentBattle.Enemies.OrderBy(e => e.Priority(currentBattle)));

            ProcessEnemyFromQueue();
        }

        private void ProcessEnemyFromQueue()
        {
            if (!enemiesInTurn.Any())
            {
                if (currentBattle.Enemies.All(e => e == null))
                {
                    // All enemies deid
                    FinishEnemyTurn(true);
                }
                // All enemies acted
                FinishEnemyTurn(false);
                return;
            }

            Enemy enemy = enemiesInTurn.Dequeue();
            if (enemy == null)
            {
                // Enemy probably died
                ProcessEnemyFromQueue();
                return;
            }

            enemy.Act(currentBattle, EnemyFinishedActing);
        }

        private void EnemyFinishedActing()
        {
            ProcessEnemyFromQueue();
        }

        /// <summary>
        /// Finishes enemy turn and says to battle manager to pass turn to a player
        /// </summary>
        /// <param name="allDied">True if all enemies died and no need to continue the battle</param>
        private void FinishEnemyTurn(bool allDied)
        {
            Debug.Log("Enemy turn finished");
            // Do all turn end stuff
            foreach (Enemy enemy in currentBattle.Enemies)
            {
                enemy.ActionPoints = enemy.ActionPointsMax;
            }
            // Ready to pass the turn to player
            callWhenEnemyTurnDone(allDied);
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

        private bool CheckIfEndBattle()
        {
            return currentBattle?.Enemies?.All(e => e.State == CharacterState.Dead) ?? true;
        }

        /// <summary>
        /// Gets all enemies in sight of one, including it
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public List<Enemy> GetEnemiesInSight(Enemy enemy)
        {
            List<Enemy> result = new List<Enemy>();
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
        public void PutEnemiesToBattle(Battle battle, List<Enemy> enemies, Action<bool> onEnemyTurnDone)
        {
            currentBattle = battle;
            callWhenEnemyTurnDone = onEnemyTurnDone;
            foreach (Enemy enemy in enemies)
            {
                // Subscribe to enemy death
                enemy.OnEnemyDied += HandleEnemyDead;

                GameObject eff = Instantiate(WarningEffectPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
                Destroy(eff, 3F);

                enemy.Healthbar.Show();
            }
        }

        private void HandleEnemyDead(object sender, EventArgs args)
        {
            if (sender is Enemy enemy)
            {
                Enemies.Remove(enemy);
            }

            if (CheckIfEndBattle())
            {
                // All enemies died
                callWhenEnemyTurnDone(true);
            }
        }
    }
}
