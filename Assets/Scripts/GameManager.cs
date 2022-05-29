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
        _map = Map.GenerateBoxMap(60, 40);
        _mapRenderer.DrawTilemap(_map);
        var wallTilemap = _mapRenderer.GetWallTilemap();
        // Camera.main.transform.position = new Vector3(
        //     _map.GetWidth() / 2,
        //     _map.GetHeight() / 2,
        //     Camera.main.transform.position.z
        // );
        _mainCameraFollow.SetBounds(wallTilemap.localBounds.min, wallTilemap.localBounds.max);
        
        ResetGame();
    }

    public void EatFood()
    {
        _food.SetPosition(_map.GetRandomRoomTile());
        _snake.Grow();
    }

    public void Collide()
    {
        _snake.ResetSnake();
        ResetGame();
    }

    public void ResetGame()
    {
        level = 1;
        _food.SetPosition(_map.GetRandomRoomTile());
        _snake.SetPosition(_map.GetRandomRoomTile());
        Camera.main.transform.position = new Vector3(
            _snake.transform.position.x,
            _snake.transform.position.y,
            Camera.main.transform.position.z
        );
    }
}
