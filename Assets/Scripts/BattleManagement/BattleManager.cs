using System;
using System.Collections.Generic;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
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
        public RewardManager RewardManager;
        public PlayerController PlayerController;
        public EnemyController EnemyController;

        private const string endTurnButtonName = "EndTurnButton";
        private const string battleIconName = "BattleIcon";

        private Battle currentBattle;

        public void Start()
        {
            RewardManager = GetComponent<RewardManager>();
            Validate();
            UIManager.Instance.Subscribe(endTurnButtonName, this);
        }

        private void Validate()
        {
            if (MapManager == null)
                throw new Exception("MapManager field should not be null");
            if (RewardManager == null)
                throw new Exception("RewardManager field should not be null");
            if (PlayerController == null)
                throw new Exception("PlayerController field should not be null");
            if (EnemyController == null)
                throw new Exception("EnemyController field should not be null");
        }

        /// <summary>
        /// Starts battle of player, enemy and everyone this enemy sees
        /// </summary>
        /// <param name="enemy"></param>
        public bool StartBattleFromEnemyAttack(Enemy enemy)
        {
            Player player = PlayerController.Player;
            List<Enemy> enemies = EnemyController.GetEnemiesInSight(enemy);

            return StartBattle(player, enemies);
        }

        /// <summary>
        /// Starts battle of player and all enemies in the list
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemies"></param>
        public bool StartBattle(Player player, List<Enemy> enemies)
        {
            Battle battle = new Battle(player, enemies);

            // Start battle events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnBattleStart(battle, token);
            if (token.ShouldBeCancelled)
                return false;

            currentBattle = battle;

            // Draw players action points
            UIManager.Instance.SetVariable(nameof(player.ActionPoints), player.ActionPointsMax);
            // Draw icon of active battle
            UIManager.Instance.SetVariable(battleIconName, 1);

            EnemyController.PutEnemiesToBattle(currentBattle, enemies, EnemyTurnEnd);

            PlayersTurnStart();

            return true;
        }

        /// <summary>
        /// Begins players turn - calls events, sets ui, sets battle state, passes control to player
        /// </summary>
        /// <returns></returns>
        private bool PlayersTurnStart()
        {
            // Start players turn events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnPlayersTurnBegin(currentBattle, token);
            if (token.ShouldBeCancelled)
                throw new NotImplementedException("Can not cancel players turn start");

            Debug.Log("Players turn");
            currentBattle.State = BattleState.PlayersTurn;
            UIManager.Instance.SetVariable(endTurnButtonName, 1);
            return PlayerController.PlayersTurn(PlayersTurnEnd);
        }

        /// <summary>
        /// Ends battle and gives player reward
        /// </summary>
        /// <param name="battle"></param>
        private bool PlayerWonBattle(Battle battle)
        {
            // End battle events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnBattleEnd(battle, token);
            if (token.ShouldBeCancelled)
                return false;

            // Put players controller from battle mode
            PlayerController.PlayerEndBattle();
            // Refresh current battle
            currentBattle = null;
            // Hide players action points
            UIManager.Instance.SetVariable(nameof(battle.Player.ActionPoints), -1);
            // Hide active battle icon
            UIManager.Instance.SetVariable(battleIconName, 0);
            // Hide end turn button
            UIManager.Instance.SetVariable(endTurnButtonName, 0);

            RewardManager.GivePlayerReward(battle);

            return true;
        }

        /// <summary>
        /// Will be called when player finishes its turn
        /// </summary>
        private void PlayersTurnEnd()
        {
            // End players turn events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnPlayersTurnEnd(currentBattle, token);
            if (token.ShouldBeCancelled)
                throw new NotImplementedException("Can not cancel players turn ending");

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
                if (PlayerWonBattle(currentBattle))
                    return;
            }

            bool result = PlayersTurnStart();
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
