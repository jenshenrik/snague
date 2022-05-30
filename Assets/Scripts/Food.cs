using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    public void SetPosition(Coord tile)
    {
        this.transform.position = new Vector3(
            tile.tileX + .5f,
            tile.tileY + .5f,
            0
        );
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }
}
