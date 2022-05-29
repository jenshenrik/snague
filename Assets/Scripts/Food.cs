using UnityEngine;

public class Food : MonoBehaviour
{
    public void SetPosition(Coord tile)
    {
        this.transform.position = new Vector3(
            tile.tileX + .5f,
            tile.tileY + .5f,
            0
        );
    }
}
