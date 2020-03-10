namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Damage information
    /// </summary>
    public class Damage
    {
        public Damage(Character source, float value)
        {
            Value = value;
            Source = source;
        }

        public float Value;

        public Character Source;
    }
}
