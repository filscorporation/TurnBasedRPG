using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CameraManagement;
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
        protected CameraManager CameraManager;

        public List<Enemy> Enemies;

        private Queue<Enemy> enemiesInTurn;
        private Battle currentBattle;

        public GameObject WarningEffectPrefab;

        private Action<bool> callWhenEnemyTurnDone;

        public void Start()
        {
            CameraManager = FindObjectOfType<CameraManager>();

            Validate();
            LoadAllEnemies();
        }

        private void Validate()
        {
            if (BattleManager == null)
                throw new Exception("BattleManager field should not be null");
            if (CameraManager == null)
                throw new Exception("CameraManager field should not be null");
        }

        private void LoadAllEnemies()
        {
            Enemies = FindObjectsOfType<Enemy>().ToList();
            DamageValueEffectController.Instance.AddToShowEffectList(Enemies);

            foreach (Enemy enemy in Enemies)
            {
                // Subscribe to enemy death
                enemy.OnCharacterDied += HandleEnemyDead;
                // Subscribe to enemy take damage
                enemy.OnCharacterTakeDamage += HandleEnemyTakeDamage;
            }
        }

        /// <summary>
        /// Passes control to enemy
        /// </summary>
        public void EnemyTurn()
        {
            Debug.Log("Starting enemy turn");

            // Sort all enemies by priority and put them into queue to act
            enemiesInTurn = new Queue<Enemy>(currentBattle.Enemies.OrderBy(e => e.Priority(currentBattle)));

            ProcessEnemyFromQueue();
        }

        private void ProcessEnemyFromQueue()
        {
            while (true)
            {
                if (!enemiesInTurn.Any())
                {
                    if (currentBattle.Enemies.All(e => e == null))
                    {
                        // All enemies deid
                        FinishEnemyTurn(true);
                        return;
                    }
                    // All enemies acted
                    FinishEnemyTurn(false);
                    return;
                }

                Enemy enemy = enemiesInTurn.Dequeue();
                if (enemy == null)
                {
                    // Enemy probably died
                    continue;
                }

                // Set camera focus on current acting enemy
                CameraManager.Follow(enemy.transform);
                enemy.Act(currentBattle, EnemyFinishedActing);
                return;
            }
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
        public bool TryStartBattle(Player player)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.InSight(player))
                {
                    return BattleManager.StartBattleFromEnemyAttack(enemy);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if player in any enemies line of sight and add it to current battle
        /// </summary>
        /// <param name="player"></param>
        public bool TryAddEnemyToBattle(Player player)
        {
            bool result = false;
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.InSight(player))
                {
                    if (AddEnemyToCurrentBattle(enemy))
                        result = true;
                }
            }

            return result;
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
        /// <param name="battle"></param>
        /// <param name="enemies"></param>
        /// <param name="onEnemyTurnDone"></param>
        public void PutEnemiesToBattle(Battle battle, List<Enemy> enemies, Action<bool> onEnemyTurnDone)
        {
            currentBattle = battle;
            callWhenEnemyTurnDone = onEnemyTurnDone;
            foreach (Enemy enemy in enemies)
            {
                GameObject eff = Instantiate(WarningEffectPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
                Destroy(eff, 3F);

                enemy.Healthbar.Show();
            }

            // Set camera focus on first enemy we going to fight
            CameraManager.Follow(enemies.First().transform);
        }

        private bool AddEnemyToCurrentBattle(Enemy enemy)
        {
            if (currentBattle == null)
                throw new Exception("Current battle is null, cant add enemy");

            if (currentBattle.Enemies.Contains(enemy))
                return false;

            currentBattle.Enemies.Add(enemy);
            GameObject eff = Instantiate(WarningEffectPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
            Destroy(eff, 3F);
            enemy.Healthbar.Show();
            return true;
        }

        private void HandleEnemyTakeDamage(object sender, EventArgs args)
        {
            if (sender is Enemy enemy)
            {
                AddEnemyToCurrentBattle(enemy);
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
