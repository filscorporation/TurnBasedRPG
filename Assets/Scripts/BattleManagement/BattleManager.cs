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
        /// Starts battle from of player, enemy and everyone this enemy sees
        /// </summary>
        /// <param name="enemy"></param>
        public void StartBattleFromEnemyAttack(Enemy enemy)
        {
            Player player = PlayerController.Player;
            UIManager.Instance.SetVariable(nameof(player.ActionPoints), player.ActionPoints);
            player.State = PlayerState.InBattle;

            List<Enemy> enemies = EnemyController.GetEnemiesInSight(enemy);
            EnemyController.PutEnemiesToBattle(enemies);

            currentBattle = new Battle(player, enemies);
        }

        private void EndTurn()
        {
            Debug.Log("Ending turn");
            UIManager.Instance.SetVariable(endTurnButtonName, 0);
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
