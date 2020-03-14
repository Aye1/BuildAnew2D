using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Sprite _spriteON;
    [SerializeField] private Sprite _spriteOFF;
#pragma warning restore 0649
    #endregion
    private SpriteRenderer _renderer;

    [SerializeField] private bool _debugIsOn;

    private bool _isOn;
    public bool IsON
    {
        get
        {
            return _isOn;
        }
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                UpdateSprite();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        IsON = _debugIsOn;
    }

    private void UpdateSprite()
    {
        _renderer.sprite = _isOn ? _spriteON : _spriteOFF;
    }
}
