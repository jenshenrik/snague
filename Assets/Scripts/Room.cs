using System;
using System.Collections.Generic;

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
