namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Interface for elements of the map that supports coordinates and path finding
    /// </summary>
    public interface IMapElement
    {
        int X { get; set; }
        int Y { get; set; }

        int Row { get; set; }
        bool Free { get; set; }
        int List { get; set; }
        int FValue { get; set; }
        int GValue { get; set; }
        int HValue { get; set; }
        int ParentX { get; set; }
        int ParentZ { get; set; }
    }
}
