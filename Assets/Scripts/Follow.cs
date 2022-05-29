using UnityEngine;

public class Follow : MonoBehaviour
{

    [SerializeField]
    private Transform follow;

    private Vector3 _minBounds;
    private Vector3 _maxBounds;
    private Camera _camera;

    private void Awake() 
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        var topBoundary = _maxBounds.y - GetVerticalExtent();
        var bottomBoundary = _minBounds.y + GetVerticalExtent();
        var leftBoundary = _minBounds.x + GetHorizontalExtent();
        var rightBoundary = _maxBounds.x - GetHorizontalExtent();

        var x = Mathf.Clamp(follow.position.x, leftBoundary, rightBoundary);
        var y = Mathf.Clamp(follow.position.y, bottomBoundary, topBoundary);
        this.transform.position = new Vector3(x, y, this.transform.position.z);
    }

    public void SetBounds(Vector3 minBounds, Vector3 maxBounds)
    {
        _minBounds = minBounds;
        _maxBounds = maxBounds;
    }

    private float GetVerticalExtent()
    {
        return _camera.orthographicSize;
    }

    private float GetHorizontalExtent()
    {
        return _camera.aspect * GetVerticalExtent();
    }
}
