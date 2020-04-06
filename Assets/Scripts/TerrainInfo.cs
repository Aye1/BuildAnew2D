using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInfo : MonoBehaviour
{
    public Color constructibleColor = new Color(0, 0, 255, 100);
    public Color floodWarningColor = new Color(255, 0, 0, 100);
    private Color invisibleInfo = new Color(0, 0, 0, 0);
    public TerrainTile dataTile;

    public void SetTerrainConstructible()
    {
        GetComponent<SpriteRenderer>().color = constructibleColor;
    }

    public void SetTerrainInconstructible()
    {
        ResetTerrainInfo();
    }

    public void SetTerrainFloodable()
    {
        GetComponent<SpriteRenderer>().color = floodWarningColor;
    }

    public void ResetTerrainInfo()
    {
        GetComponent<SpriteRenderer>().color = invisibleInfo;
    }
}
