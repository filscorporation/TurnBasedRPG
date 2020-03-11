using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.MapManagement;

namespace Assets.Scripts.PlayerManagement
{
    /// <summary>
    /// Information about targets skill was casted on
    /// </summary>
    public class SkillTarget
    {
        public List<Character> CharacterTargets;

        public Tile TileTarget;

        public SkillTarget(Tile tile)
        {
            TileTarget = tile;
        }

        public SkillTarget(Character character)
        {
            CharacterTargets = new List<Character> { character };
        }

        public SkillTarget(List<Character> characters)
        {
            CharacterTargets = characters;
        }
    }
}
