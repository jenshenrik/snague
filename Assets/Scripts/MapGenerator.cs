using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Size")]

    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    [Header("Initialization")]
    [SerializeField]
    [Range(0, 100)]
    private int randomFillPercent;

    [SerializeField]
    private string seed;
    [SerializeField]
    private bool useRandomSeed;

    [Header("Processing")]
    [SerializeField]
    private int smoothingPasses;
    [SerializeField]
    private int wallThresholdSize;
    [SerializeField]
    private int roomThresholdSize;

    private int[,] map;
    
    public Map GenerateMap()
    {
        var map = new Map(width, height);
        // for (int x = 0; x < width; x++)
        // {
        //     for (int y = 0; y < height; y++)
        //     {
        //         if (x == 0 || x == width - 1 || y == 0  || y == height - 1)
        //         {
        //             map.SetTile(x, y, Map.WALL);
        //         }
        //         else
        //         {
        //             map.SetTile(x, y, Map.FLOOR);
        //         }
        //     }
        // }

        // for (int i = 0; i < smoothingPasses; i++)
        // {
        //     SmoothMap();
        // }
        // ProcessMap();
        
        // DrawTilemap();
        return map;
    }

}

public struct Coord
{
    public int tileX;
    public int tileY;

    public Coord(int x, int y)
    {
        tileX = x;
        tileY = y;
    }
}

public class Room : IComparable<Room>
{
    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<Room> connectedRooms;
    public int roomSize;
    public bool IsAccesibleFromMainRoom;
    public bool IsMainRoom;

    public Room() { }

    public Room(List<Coord> roomTiles, int[,] map)
    {
        tiles = roomTiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();
        edgeTiles = new List<Coord>();

        foreach (var tile in tiles)
        {
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x == tile.tileX || y == tile.tileY)
                    {
                        if (map[x, y] == Map.WALL)
                        {
                            edgeTiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA.IsAccesibleFromMainRoom)
        {
            roomB.SetAcceibleFromMainRoom();
        }
        else if (roomB.IsAccesibleFromMainRoom)
        {
            roomA.SetAcceibleFromMainRoom();
        }
        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }

    public void SetAcceibleFromMainRoom()
    {
        if (!IsAccesibleFromMainRoom)
        {
            IsAccesibleFromMainRoom = true;
            foreach (var connectedRoom in connectedRooms)
            {
                connectedRoom.IsAccesibleFromMainRoom = true;
            }
        }
    }

    public int CompareTo(Room other)
    {
        return other.roomSize.CompareTo(roomSize);
    }

    public bool IsConnected(Room otherRoom)
    {
        return connectedRooms.Contains(otherRoom);
    }
}

public class Map
{
    public static readonly int WALL = 1;
    public static readonly int FLOOR = 0;

    private int[,] map;
    private int width;
    private int height;
    private List<Coord> _roomTiles;
    private System.Random _random;

    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        map = new int[width, height];
        _roomTiles = new List<Coord>();
        _random = new System.Random(Time.time.ToString().GetHashCode());
    }

    public void SetTile(int x, int y, int value)
    {
        map[x,y] = value;
    }
   
    public int GetTile(int x, int y)
    {
        return map[x, y];
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public List<Coord> GetRoomTiles()
    {
        return _roomTiles;
    }

    public Coord GetRandomRoomTile()
    {
        var i = _random.Next(0, _roomTiles.Count);
        return _roomTiles[i];
    }

    public static Map GenerateBoxMap(int width, int height)
    {
        var map = new Map(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0  || y == height - 1)
                {
                    map.SetTile(x, y, Map.WALL);
                }
                else
                {
                    map.SetTile(x, y, Map.FLOOR);
                }
            }
        }
        map.Process(1, 1);
        return map;
    }

    public static Map GenerateRandomMap(
        int width,
        int height,
        int randomFillPercent, 
        int smoothingPasses, 
        int wallThresholdSize, 
        int roomThresholdSize)
    {
        var map = new Map(width, height);
        map.RandomFillMap(randomFillPercent);
        for (int i = 0; i < smoothingPasses; i++)
        {
            map.Smooth();
        }
        map.Process(wallThresholdSize, roomThresholdSize);
        return map;
    }

    public void Process(int wallThresholdSize, int roomThresholdSize)
    {
        var walls = GetRegions(Map.WALL);
        foreach (var region in walls)
        {
            if (region.Count < wallThresholdSize)
            {
                foreach (var tile in region)
                {
                    map[tile.tileX, tile.tileY] = Map.FLOOR;
                }
            }
        }

        var rooms = GetRegions(Map.FLOOR);
        var survivingRooms = new List<Room>();
        foreach (var region in rooms)
        {
            if (region.Count < roomThresholdSize)
            {
                foreach (var tile in region)
                {
                    map[tile.tileX, tile.tileY] = Map.WALL;
                }
            }
            else
            {
                survivingRooms.Add(new Room(region, map));
            }
        }
        survivingRooms.Sort();
        survivingRooms[0].IsMainRoom = true;
        survivingRooms[0].IsAccesibleFromMainRoom = true;

        _roomTiles = survivingRooms.SelectMany(r => r.tiles).ToList();
        ConnectClosestRooms(survivingRooms);
    }

    private void Smooth()
    {
         for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var wallCount = GetSurroundingWallCount(x, y);

                if (wallCount > 4)
                {
                    map[x,y] = WALL;
                }
                else if (wallCount < 4)
                {
                    map[x,y] = FLOOR;
                }
            }
        }
    }

    private void RandomFillMap(int randomFillPercent)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x,y] = Map.WALL;
                } 
                else 
                {
                    map[x,y] = _random.Next(0, 100) < randomFillPercent 
                        ? Map.WALL 
                        : Map.FLOOR;
                }
            }
        }
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallcount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallcount += map[neighbourX, neighbourY];
                    }
                }
                else 
                {
                    wallcount++;
                }
            }
        }
        return wallcount;
    }

    private List<Coord> GetRegionTiles(int startX, int startY)
    {
        var tiles = new List<Coord>();
        var mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        var queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            var tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (x == tile.tileX || y == tile.tileY))
                    {
                        if (mapFlags[x,y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    private List<List<Coord>> GetRegions(int tileType)
    {
        var regions = new List<List<Coord>>();
        var mapFlags = new int[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    var newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    
                    foreach (var tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    private void ConnectClosestRooms(List<Room> allRooms, bool forceAccesibilityFromMainRoom = false)
    {
        var roomListA = new List<Room>();
        var roomListB = new List<Room>();

        if (forceAccesibilityFromMainRoom)
        {
            foreach (var room in allRooms)
            {
                if (room.IsAccesibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        var bestDistance = 0;
        var bestTileA = new Coord();
        var bestTileB = new Coord();
        var bestRoomA = new Room();
        var bestRoomB = new Room();
        var possibleConnectionFound = false;

        foreach (var roomA in roomListA)
        {
            if (!forceAccesibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach(var roomB in roomListB)
            {
                if (roomA == roomB || roomA.connectedRooms.Contains(roomB)) continue;
                

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        var tileA = roomA.edgeTiles[tileIndexA];
                        var tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));
                        
                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccesibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccesibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccesibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        foreach (var c in GetLine(tileA, tileB))
        {
            DrawCircle(c, 1);
        }
    }

    private void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x*x + y*y <= r*r)
                {
                    var drawX = c.tileX + x;
                    var drawY = c.tileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        map[drawX, drawY] = Map.FLOOR;
                    }
                }
            }
        }
    }

    private List<Coord> GetLine(Coord from, Coord to)
    {
        var line = new List<Coord>();

        var x = from.tileX;
        var y = from.tileY;

        var dx = to.tileX - from.tileX;
        var dy = to.tileY - from.tileY;

        var step = Math.Sign(dx);
        var gradientStep = Math.Sign(dy);

        var longest = Mathf.Abs(dx);
        var shortest = Mathf.Abs(dy);

        var inverted = false;

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        var gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }
}