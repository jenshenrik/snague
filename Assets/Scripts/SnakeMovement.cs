using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    private Vector2 _direction;
    private Rigidbody2D _myRigidbody2D;
    private GameManager _gameManager;
    private readonly Vector2 initialDirection = new Vector2(0f, 0f);

    [SerializeField]
    private Transform segmentPrefab;
    private List<Transform> _segments = new List<Transform>();

    private bool hasInput;

    private void Awake()
    {
        _myRigidbody2D = GetComponent<Rigidbody2D>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        ResetSnake();
    }

    private void Update()
    {
        // only detect direction change if no input was made since last move
        if (!hasInput)
        {
            if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
            {
                _direction = Vector2.up;
                // register that input was made
                hasInput = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
            {
                _direction = Vector2.down;
                // register that input was made
                hasInput = true;
            }
            else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
            {
                _direction = Vector2.right;
                // register that input was made
                hasInput = true;
            }
            else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
            {
                _direction = Vector2.left;
                // register that input was made
                hasInput = true;
            }
            
        }
    }

    private void FixedUpdate()
    {
        MoveSegments();
        MoveHead();
        // reset input so that new one can be entered
        hasInput = false;
    }

    private void MoveHead()
    {
        _myRigidbody2D.transform.position += new Vector3(
            Mathf.Round(_direction.x),
            Mathf.Round(_direction.y),
            0
        );
    }

    private void MoveSegments()
    {
        // move tail segments by moving each to the position of 
        // its predecessor starting from the end of the tail
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Wall" || other.tag == "Player")
        {
            _gameManager.Collide();
        }
        else if (other.tag == "Food")
        {
            _gameManager.EatFood();
        }
    }

    public void ResetSnake()
    {
        hasInput = false;
        _direction = initialDirection;
        Stop();

        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);
    }

    public void Grow()
    {
        var segment = Instantiate(segmentPrefab);
        segment.GetComponent<SpriteRenderer>().sortingOrder = 5;
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);
    }

    public void Stop()
    {
        _direction = initialDirection;
    }

    public void SetPosition(Coord tile)
    {
        this._myRigidbody2D.transform.position = new Vector3(tile.tileX + .5f, tile.tileY + .5f, 0);
    }
}
