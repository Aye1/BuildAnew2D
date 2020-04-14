using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
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

    [SerializeField] private SpriteRenderer _warningFloodSprite;

    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _warningFloodSprite.gameObject.SetActive(false);
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
        _warningFloodSprite.gameObject.SetActive(true);
        //_renderer.color = floodWarningColor;
        //_currentColor = floodWarningColor;
    }

    public void ResetTerrainInfo()
    {
        _warningFloodSprite.gameObject.SetActive(false);
        _renderer.color = invisibleInfo;
        _currentColor = invisibleInfo;
    }

    public void ShowConstructibleView()
    {
        if(_isConstructible)
        {
            _renderer.color = constructibleColor;
            _currentColor = constructibleColor;
        }
    }
    public void ShowInsideAreaColor()
    {
        _renderer.sprite = displayAreaSprite;
        _renderer.color = new Color(25,255,255,255);

    }
    public void HideInsideAreaColor()
    {
        _renderer.sprite = initialSprite;
        _renderer.color = _currentColor;
    }

        public void HideConstructibleView()
    {
        ResetTerrainInfo();
    }
}
