using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TerrainInfo : MonoBehaviour
{
    public Sprite initialSprite;
    public Sprite displayAreaSprite;
    public TerrainTile dataTile;

    private bool _isConstructible = false;
    private Color _currentColor;
    private readonly Color _invisibleInfo = new Color(0, 0, 0, 0);
    private readonly Color _constructibleColor = new Color(0, 0, 255, 100);
    private readonly Color _whiteColor = new Color(25,255,255,255);

#pragma warning disable 0649
    [SerializeField] private SpriteRenderer _warningFloodSprite;
#pragma warning restore 0649

    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _warningFloodSprite.gameObject.SetActive(false);
        TacticalViewManager.OnShowConstructibleView += ShowConstructibleView;
        TacticalViewManager.OnHideConstructibleView += HideConstructibleView;
    }

    void OnDestroy()
    {
        TacticalViewManager.OnShowConstructibleView -= ShowConstructibleView;
        TacticalViewManager.OnHideConstructibleView -= HideConstructibleView;
        Destroy(gameObject);
        _renderer = null;
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
    }

    public void HideFloodableInfo()
    {
        _warningFloodSprite.gameObject.SetActive(false);   
    }

    public void ShowConstructibleView()
    {
        if(_isConstructible)
        {
            _renderer.sprite = initialSprite;
            _renderer.color = _constructibleColor;
            _currentColor = _constructibleColor;
        }
    }
    public void ShowInsideAreaColor()
    {
        _renderer.sprite = displayAreaSprite;
        _renderer.color = _whiteColor;

    }
    public void HideInsideAreaColor()
    {
        _renderer.sprite = initialSprite;
        _renderer.color = _currentColor;
    }

        public void HideConstructibleView()
    {
        _renderer.color = _invisibleInfo;
        _currentColor = _invisibleInfo;
    }
}
