using System.Collections.Generic;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.PlayerManagement;

namespace Assets.Scripts.BattleManagement
{
    public enum BattleState
    {
        PlayersTurn,
        EnemyTurn,
    }

    /// <summary>
    /// Information about one battle: player and enemies
    /// </summary>
    public class Battle
    {
        public Player Player;

        public List<Enemy> Enemies;

        public BattleState State;

        public Battle(Player player, List<Enemy> enemies)
        {
            Player = player;
            Enemies = enemies;
            State = BattleState.PlayersTurn;
        }
    }
}
