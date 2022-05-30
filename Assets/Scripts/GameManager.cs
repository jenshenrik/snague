using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Food _food;
    [SerializeField]
    private Sprite _foodRegularSprite;
    [SerializeField]
    private Sprite _foodLastSprite;
    [SerializeField]
    private SnakeMovement _snake;
    [SerializeField]
    private HealthBar _healthBar;

    [Header("Map")]
    [SerializeField]
    private MapRenderer _mapRenderer;

    private int _level;
    private int _health;
    private int _targetHealth = 3;
    private Map _map;
    private Follow _mainCameraFollow;

    private void Awake()
    {
        _mainCameraFollow = Camera.main.GetComponent<Follow>();
    }

    private void Start() 
    {
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
        _food.SetSprite(_foodRegularSprite);
    }

    public void EatFood()
    {
        _health++;
        _healthBar.SetHealth(_health);
        if (_health == _targetHealth)
        {
            ProgressLevel();
        }
        else
        {
            _food.SetPosition(_map.GetRandomRoomTile());
            if (_health == _targetHealth - 1)
            {
                _food.SetSprite(_foodLastSprite);
            }
            _snake.Grow();
        }
    }

    private void ProgressLevel()
    {
        _snake.ResetSnake();
        _level++;
        _health = 0;
        if (_level == 1)
        {
            _map = Map.GenerateBoxMap(30, 20);
            _targetHealth = 3;
        }
        else if (_level == 2)
        {
            _map = Map.GenerateBoxMap(45, 30);
            _targetHealth = 5;
        }
        else
        {
            _map = Map.GenerateRandomMap(45, 30, GetFillPercentage(_level), 3, 8, 10);
            _targetHealth++;
        }
        _healthBar.SetHealth(_health);
        _healthBar.SetMaxHealth(_targetHealth);
        SetupMap();
        
    }

    private int GetFillPercentage(int level)
    {
        return 40 + level % 3;
    }

    public void Collide()
    {
        _health--;
        _healthBar.SetHealth(_health);
        if (_health < 0)
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
            _food.SetSprite(_foodRegularSprite);
        }
    }

    private void ResetGame()
    {
        _level = 0;
        ProgressLevel();
        SetupMap();
    }
}
