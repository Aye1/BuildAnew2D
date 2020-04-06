using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInfo : MonoBehaviour
{
    private Color constructibleColor;
    private Color inconstructibleColor;
    public TerrainTile dataTile;
    // Start is called before the first frame update
    void Start()
    {
        constructibleColor = new Color(0, 0, 255, 255);
        inconstructibleColor = new Color(0, 255, 0, 255);
        GetComponent<SpriteRenderer>().color = inconstructibleColor;
    }

    public void SetTerrainConstructible()
    {
        GetComponent<SpriteRenderer>().color = constructibleColor;
    }

    public void SetTerrainInconstructible()
    {
        GetComponent<SpriteRenderer>().color = inconstructibleColor;
    }
}
