using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    [SerializeField] private Sprite _spriteON;
    [SerializeField] private Sprite _spriteOFF;
    private SpriteRenderer _renderer;

    public bool DebugIsOn;

    private bool _isOn;
    public bool IsON
    {
        get
        {
            return _isOn;
        }
        set
        {
            if(_isOn != value)
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
        IsON = DebugIsOn;
    }

    private void UpdateSprite()
    {
        _renderer.sprite = _isOn ? _spriteON : _spriteOFF;
    }
}
