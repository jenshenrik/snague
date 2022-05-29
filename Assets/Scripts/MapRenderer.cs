using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemapWalls;
    [SerializeField]
    private Tile wallTile;
    [SerializeField]
    private Tilemap tilemapFloor;
    [SerializeField]
    private Tile floorTile;

    public void DrawTilemap(Map map)
    {
        for (int x = 0; x < map.GetWidth(); x++)
        {
            for (int y = 0; y < map.GetHeight(); y++)
            {
                var position = new Vector3Int(x, y, 0);
                if (map.GetTile(x, y) == Map.WALL)
                {
                    tilemapWalls.SetTile(position, wallTile);
                }
                else
                {
                    tilemapFloor.SetTile(position, floorTile);
                }
            }
        }
    }

    public Tilemap GetWallTilemap()
    {
        return tilemapWalls;
    }
}
