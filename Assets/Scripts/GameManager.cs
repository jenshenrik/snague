using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Food _food;
    [SerializeField]
    private SnakeMovement _snake;

    [Header("Map")]
    [SerializeField]
    private MapGenerator _mapGenerator;
    [SerializeField]
    private MapRenderer _mapRenderer;

    private int level;
    private int health;
    private int targetHealth = 3;
    private Map _map;
    private readonly Vector3 initialPosition = new Vector3(10.5f, 10.5f, 0f);
    private Follow _mainCameraFollow;

    private void Awake()
    {
        _mainCameraFollow = Camera.main.GetComponent<Follow>();
    }

    private void Start() 
    {
        // _map = _mapGenerator.GenerateMap();
        // _map.GenerateRandomMap(35, 3, 50, 50);
        // _map.Process(1, 1);
        // _map = Map.GenerateBoxMap(20, 30);
        ResetGame();
    }

    private void SetupMap()
    {
         _mapRenderer.DrawTilemap(_map);
        var wallTilemap = _mapRenderer.GetWallTilemap();
        _mainCameraFollow.SetBounds(wallTilemap.localBounds.min, wallTilemap.localBounds.max);

        _snake.SetPosition(_map.GetRandomRoomTile());
        Camera.main.transform.position = new Vector3(
            _snake.transform.position.x,
            _snake.transform.position.y,
            Camera.main.transform.position.z
        );

        _food.SetPosition(_map.GetRandomRoomTile());
    }

    public void EatFood()
    {
        health++;
        if (health == targetHealth)
        {
            ProgressLevel();
        }
        else
        {
            _food.SetPosition(_map.GetRandomRoomTile());
            _snake.Grow();
        }
    }

    private void ProgressLevel()
    {
        _snake.ResetSnake();
        level++;
        health = 0;
        if (level == 1)
        {
            _map = Map.GenerateBoxMap(30, 20);
            targetHealth = 3;
        }
        else if (level == 2)
        {
            _map = Map.GenerateBoxMap(45, 30);
            targetHealth = 5;
        }
        // else
        // {
        //     // do procgen shit
        // }
        SetupMap();
        
    }

    public void Collide()
    {
        health--;
        if (health == 0)
        {
            // game over
            ResetGame();
        }
        else
        {
            _snake.ResetSnake();
            _snake.Stop();
            _snake.SetPosition(_map.GetRandomRoomTile());
            _food.SetPosition(_map.GetRandomRoomTile());
        }
    }

    private void ResetGame()
    {
        level = 0;
        ProgressLevel();
        SetupMap();
    }
}
