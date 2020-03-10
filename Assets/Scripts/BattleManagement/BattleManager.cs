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
            UIManager.Instance.SetVariable(nameof(player.ActionPoints), player.ActionPoints);

            List<Enemy> enemies = EnemyController.GetEnemiesInSight(enemy);
            EnemyController.PutEnemiesToBattle(enemies);
            
            UIManager.Instance.SetVariable(battleIconName, 1);
            currentBattle = new Battle(player, enemies);
            currentBattle.State = BattleState.PlayersTurn;

            PlayerController.PlayersTurn(PlayersTurnEnd);
            Debug.Log("Players turn");
            UIManager.Instance.SetVariable(endTurnButtonName, 1);
        }

        /// <summary>
        /// Will be called when player finishes its turn
        /// </summary>
        private void PlayersTurnEnd()
        {
            Debug.Log("Enemy turn");
            UIManager.Instance.SetVariable(endTurnButtonName, 2);
            currentBattle.State = BattleState.EnemyTurn;
            EnemyController.EnemyTurn(EnemyTurnEnd);
        }

        /// <summary>
        /// Will be called when enemy finishes its turn
        /// </summary>
        private void EnemyTurnEnd()
        {
            Debug.Log("Players turn");
            UIManager.Instance.SetVariable(endTurnButtonName, 1);
            currentBattle.State = BattleState.PlayersTurn;
            PlayerController.PlayersTurn(PlayersTurnEnd);
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
