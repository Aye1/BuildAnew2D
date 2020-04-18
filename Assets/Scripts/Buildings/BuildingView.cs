using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BuildingView : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [Header("Editor bindings")]
    [SerializeField] private Sprite _spriteON;
    [SerializeField] private Sprite _spriteOFF;
    // This object does not store data, but it's the only way to acceed it from the editor
    // Thus, the ability to turn on/off the building is done here
    [Header("Debug controls")]
    [SerializeField] private bool _forceDebugIsOn;
#pragma warning restore 0649
    #endregion
    private SpriteRenderer _renderer;
    private Animator _animator;

    public StructureTile dataTile;

    public virtual void SpecificUpdate() { }

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sortingOrder = 10;
        _animator = GetComponent<Animator>();
    }
    public void DestroyBuilding()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        if(_forceDebugIsOn)
        {
            dataTile.IsOn = _forceDebugIsOn;
        }
        if(_animator != null)
        {
            _animator.SetBool("IsOn", dataTile.IsOn);
        }
        SpecificUpdate();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        _renderer.sprite = dataTile.IsOn ? _spriteON : _spriteOFF;
    }

    public void WarnDestruction()
    {
        _renderer.color = Color.red;
    }
    public void DisableWarningDestruction()
    {
        _renderer.color = Color.white;
    }

    public void UpgradeBuilding()
    {
        _renderer.color = Color.green;
    }
}
