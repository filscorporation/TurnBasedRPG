using System;
using System.Collections.Generic;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement;
using UnityEngine;

namespace Assets.Scripts.BattleManagement
{
    /// <summary>
    /// Controlls battle: turns, characters, ui
    /// </summary>
    public class BattleManager : MonoBehaviour, IUISubscriber
    {
        public MapManager MapManager;
        public PlayerController PlayerController;
        public EnemyController EnemyController;

        private const string endTurnButtonName = "EndTurnButton";
        private const string battleIconName = "BattleIcon";

        private Battle currentBattle;

        public void Start()
        {
            Validate();
            UIManager.Instance.Subscribe(endTurnButtonName, this);
        }

        private void Validate()
        {
            if (MapManager == null)
                throw new Exception("MapManager field should not be null");
            if (PlayerController == null)
                throw new Exception("PlayerController field should not be null");
            if (EnemyController == null)
                throw new Exception("EnemyController field should not be null");
        }

        /// <summary>
        /// Starts battle of player, enemy and everyone this enemy sees
        /// </summary>
        /// <param name="enemy"></param>
        public void StartBattleFromEnemyAttack(Enemy enemy)
        {
            Player player = PlayerController.Player;
            List<Enemy> enemies = EnemyController.GetEnemiesInSight(enemy);

            StartBattle(player, enemies);
        }

        /// <summary>
        /// Starts battle of player and all enemies in the list
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemies"></param>
        public void StartBattle(Player player, List<Enemy> enemies)
        {
            // Draw players action points
            UIManager.Instance.SetVariable(nameof(player.ActionPoints), player.ActionPointsMax);
            // Draw icon of active battle
            UIManager.Instance.SetVariable(battleIconName, 1);

            currentBattle = new Battle(player, enemies);
            currentBattle.State = BattleState.PlayersTurn;

            EnemyController.PutEnemiesToBattle(currentBattle, enemies, EnemyTurnEnd);

            PlayerController.PlayersTurn(PlayersTurnEnd);
            Debug.Log("Players turn");
            UIManager.Instance.SetVariable(endTurnButtonName, 1);
        }

        /// <summary>
        /// Ends battle and gives player reward
        /// </summary>
        /// <param name="battle"></param>
        private void PlayerWonBattle(Battle battle)
        {
            // Put player into free control state
            battle.Player.PlayerState = PlayerState.FreeControl;
            // Refresh current battle
            currentBattle = null;
            // Hide players action points
            UIManager.Instance.SetVariable(nameof(battle.Player.ActionPoints), -1);
            // Hide active battle icon
            UIManager.Instance.SetVariable(battleIconName, 0);
            // Hide end turn button
            UIManager.Instance.SetVariable(endTurnButtonName, 0);
        }

        /// <summary>
        /// Will be called when player finishes its turn
        /// </summary>
        private void PlayersTurnEnd()
        {
            Debug.Log("Enemy turn");
            UIManager.Instance.SetVariable(endTurnButtonName, 2);
            currentBattle.State = BattleState.EnemyTurn;
            EnemyController.EnemyTurn();
        }

        /// <summary>
        /// Will be called when enemy finishes its turn
        /// </summary>
        private void EnemyTurnEnd(bool allEnemyDied)
        {
            Debug.Log("Players turn");

            if (allEnemyDied)
            {
                // All enemies died
                PlayerWonBattle(currentBattle);
                return;
            }

            UIManager.Instance.SetVariable(endTurnButtonName, 1);
            currentBattle.State = BattleState.PlayersTurn;
            bool result = PlayerController.PlayersTurn(PlayersTurnEnd);
            if (!result)
            {
                // Player left battle 
                throw new NotImplementedException();
            }
        }

        private void EndTurn()
        {
            if (currentBattle == null || currentBattle.State == BattleState.EnemyTurn)
                return;
            Debug.Log("Ending turn");
            PlayerController.PlayersTurnEnd();
        }

        public void Handle(UIEvent uiEvent)
        {
            switch (uiEvent.ButtonName)
            {
                case endTurnButtonName:
                    EndTurn();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(uiEvent.ButtonName);
            }
        }
    }
}
