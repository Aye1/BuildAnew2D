using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInfo : MonoBehaviour
{
    public Color constructibleColor = new Color(0, 0, 255, 100);
    public Color floodWarningColor = new Color(255, 0, 0, 100);
    private Color invisibleInfo = new Color(0, 0, 0, 0);
    public TerrainTile dataTile;
    private bool _isConstructible = false;

    void Start()
    {
        TacticalViewManager.OnShowConstructibleView += ShowConstructibleView;
        TacticalViewManager.OnHideConstructibleView += HideConstructibleView;
    }

    public void SetTerrainConstructible()
    {
        _isConstructible = true;
        if(TacticalViewManager.Instance.IsConstructibleViewVisible())
        {
            ShowConstructibleView();
        }
    }

    public void SetTerrainInconstructible()
    {
        _isConstructible = false;
        HideConstructibleView();
    }

    public void SetTerrainFloodable()
    {
        GetComponent<SpriteRenderer>().color = floodWarningColor;
    }

    public void ResetTerrainInfo()
    {
        GetComponent<SpriteRenderer>().color = invisibleInfo;
    }

    public void ShowConstructibleView()
    {
        if(_isConstructible)
        {
            GetComponent<SpriteRenderer>().color = constructibleColor;
        }
    }

    public void HideConstructibleView()
    {
        ResetTerrainInfo();
    }
}
