using Assets.Scripts.InteractableObjects;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.SkillManagement;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameDataManagement
{
    /// <summary>
    /// Manages loading and clear spawning of the player
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        private static PlayerSpawner instance;
        public static PlayerSpawner Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<PlayerSpawner>();
                return instance;
            }
        }

        public GameObject PlayerPrefab;

        /// <summary>
        /// Load and spawn player from data
        /// </summary>
        /// <param name="playerData">Loaded player data</param>
        /// <returns></returns>
        public Player SpawnPlayer(PlayerData playerData)
        {
            Tile tile = MapManager.Instance.Field[playerData.OnTileX, playerData.OnTileY];
            Player player = SpawnPlayer(tile);

            player.MovingSpeed = playerData.MovingSpeed;
            player.ActionPoints = playerData.ActionPoints;
            player.ActionPointsMax = playerData.ActionPointsMax;
            player.Health = playerData.Health;
            player.HealthMax = playerData.HealthMax;
            player.Skills = playerData.Skills.Select(s => GetSkillFromString(s)).ToList();
            player.Level = playerData.Level;
            player.SkillPoints = playerData.SkillPoints;
            player.Experience = playerData.Experience;
            // TODO: skill level
            player.SkillBook = playerData.SkillBook.Select(s => Skill.SkillDictionary[s.Name]).ToList();
            // TODO: amount
            player.Inventory.Consumables = playerData.Consumables
                .Select(c => Consumable.ConsumablesDictionary[c.Name]).ToList();
            player.Inventory.Items = playerData.Items.Select(i => Item.ItemDictionary[i]).ToList();
            player.Inventory.Gold = playerData.Gold;

            player.SetIsLoaded();

            return player;
        }

        /// <summary>
        /// Spawns player at a random entrance
        /// </summary>
        public void SpawnPlayer()
        {
            foreach (Tile tile in FindObjectsOfType<Entrance>().Select(d => d.OnTile))
            {
                Tile freeTile = MapManager.Instance.GetNeighbours(tile).FirstOrDefault(t => t.Free);
                if (freeTile != null)
                {
                    SpawnPlayer(freeTile);
                    return;
                }
            }
            // No free tiles near entrance
            SpawnPlayer(FindObjectOfType<Entrance>().OnTile);
        }

        /// <summary>
        /// Spawns player at a certain entrance
        /// </summary>
        /// <param name="entranceDirection"></param>
        /// <param name="playerData"></param>
        public void SpawnPlayer(Direction entranceDirection, PlayerData playerData)
        {
            Player player = SpawnPlayer(playerData);

            Entrance entrance = FindObjectsOfType<Entrance>().First(e => e.Direction == entranceDirection);

            Tile freeTile = MapManager.Instance.GetNeighbours(entrance.OnTile).FirstOrDefault(t => t.Free);
            if (freeTile == null)
                freeTile = entrance.OnTile;

            player.OnTile = freeTile;
            player.gameObject.transform.position = freeTile.transform.position;
        }

        /// <summary>
        /// Put player on the tile
        /// </summary>
        /// <param name="tile">Tile</param>
        /// <returns></returns>
        protected Player SpawnPlayer(Tile tile)
        {
            GameObject go = Instantiate(PlayerPrefab, tile.transform.position, Quaternion.identity);
            Player player = go.GetComponent<Player>();
            player.OnTile = tile;
            FindObjectOfType<PlayerController>().Player = player;

            return player;
        }

        /// <summary>
        /// Get skill or consumable from its name that can be in a skillbar
        /// </summary>
        /// <param name="skillName"></param>
        /// <returns></returns>
        protected Skill GetSkillFromString(string skillName)
        {
            if (Skill.SkillDictionary.TryGetValue(skillName, out Skill skill))
                return skill;
            return Consumable.ConsumablesDictionary[skillName].UsageEffect;
        }
    }
}
