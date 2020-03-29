using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.SkillManagement;
using System;
using System.Linq;

namespace Assets.Scripts.GameDataManagement
{
    /// <summary>
    /// Game main data to be saved and loaded
    /// </summary>
    [Serializable]
    public class GameData
    {
        public int Seed;

        public PlayerData Player;

        public RoomData Room;
    }

    [Serializable]
    public class PlayerData
    {
        public short OnTileX;

        public short OnTileY;

        public float MovingSpeed;

        public short ActionPoints;

        public short ActionPointsMax;

        public float Health;

        public float HealthMax;

        public string[] Skills;

        public short Level;

        public short SkillPoints;

        public int Experience;

        public SkillData[] SkillBook;

        public ConsumableData[] Consumables;

        public string[] Items;

        public int Gold;

        public PlayerData(Player player)
        {
            OnTileX = (short)player.OnTile.X;
            OnTileY = (short)player.OnTile.Y;
            MovingSpeed = player.MovingSpeed;
            ActionPoints = (short)player.ActionPoints;
            ActionPointsMax = (short)player.ActionPointsMax;
            Health = player.Health;
            HealthMax = player.HealthMax;
            Skills = player.Skills.Select(s => s.Name).ToArray();
            Level = (short)player.Level;
            SkillPoints = (short)player.SkillPoints;
            Experience = player.Experience;
            SkillBook = player.SkillBook.Select(s => new SkillData(s)).ToArray();
            Consumables = player.Inventory.Consumables.Select(c => new ConsumableData(c)).ToArray();
            Items = player.Inventory.Items.Select(i => i.Name).ToArray();
            Gold = player.Inventory.Gold;
        }
    }

    [Serializable]
    public class SkillData
    {
        public string Name;

        public short Level;

        public SkillData(Skill skill)
        {
            Name = skill.Name;
            Level = (short)skill.Level;
        }
    }

    [Serializable]
    public class ConsumableData
    {
        public string Name;

        public short Amount;

        public ConsumableData(Consumable consumable)
        {
            Name = consumable.Name;
            // TODO: amount
            Amount = 1;
        }
    }

    [Serializable]
    public class RoomData
    {
        public FieldData Field;

        public EnemyData[] Enemies;

        public EntranceData[] Entrances;

        public ChestData[] Chests;
    }

    [Serializable]
    public class FieldData
    {
        public short Width;

        public short Height;

        public TileData[,] Tiles;
    }

    [Serializable]
    public class TileData
    {
        public short Details;

        public short Dirt;

        public short Tree;

        public short OnGroundObject;

        public TileData(TileType type)
        {
            Details = type.Details;
            Dirt = type.Dirt;
            Tree = type.Tree;
            OnGroundObject = type.OnGroundObject;
        }
    }

    [Serializable]
    public class EnemyData
    {
        public string Name;

        public short OnTileX;

        public short OnTileY;

        public float Health;

        public EnemyData(Enemy enemy)
        {
            Name = enemy.Name;
            OnTileX = (short)enemy.OnTile.X;
            OnTileY = (short)enemy.OnTile.Y;
            Health = enemy.Health;
        }
    }

    [Serializable]
    public class EntranceData
    {
        public short OnTileX;

        public short OnTileY;

        public EntranceData(Entrance entrance)
        {
            OnTileX = (short)entrance.OnTile.X;
            OnTileY = (short)entrance.OnTile.Y;
        }
    }

    [Serializable]
    public class ChestData
    {
        public short OnTileX;

        public short OnTileY;

        public bool IsLooted;

        public string[] Items;

        public ChestData(Chest chest)
        {
            OnTileX = (short)chest.OnTile.X;
            OnTileY = (short)chest.OnTile.Y;
            IsLooted = chest.IsLooted;
            Items = chest.Items.ToArray();
        }
    }
}
