using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInfo : MonoBehaviour
{
    public Color constructibleColor = new Color(0, 0, 255, 100);
    public Sprite initialSprite;
    public Sprite displayAreaSprite;
    public Color floodWarningColor = new Color(255, 0, 0, 100);
    private Color invisibleInfo = new Color(0, 0, 0, 0);
    public TerrainTile dataTile;
    private bool _isConstructible = false;
    private Color _currentColor;

    void Start()
    {
        TacticalViewManager.OnShowConstructibleView += ShowConstructibleView;
        TacticalViewManager.OnHideConstructibleView += HideConstructibleView;
    }

    public void DestroyTerrainInfo()
    {
        TacticalViewManager.OnShowConstructibleView -= ShowConstructibleView;
        TacticalViewManager.OnHideConstructibleView -= HideConstructibleView;
        Destroy(gameObject);
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
        _currentColor = floodWarningColor;
    }

    public void ResetTerrainInfo()
    {
        GetComponent<SpriteRenderer>().color = invisibleInfo;
        _currentColor = invisibleInfo;
    }

    public void ShowConstructibleView()
    {
        if(_isConstructible)
        {
            GetComponent<SpriteRenderer>().color = constructibleColor;
            _currentColor = constructibleColor;
        }
    }
    public void ShowInsideAreaColor()
    {
        GetComponent<SpriteRenderer>().sprite = displayAreaSprite;
        GetComponent<SpriteRenderer>().color = new Color(25,255,255,255);

    }
    public void HideInsideAreaColor()
    {
        GetComponent<SpriteRenderer>().sprite = initialSprite;
        GetComponent<SpriteRenderer>().color = _currentColor;
    }

        public void HideConstructibleView()
    {
        ResetTerrainInfo();
    }
}
