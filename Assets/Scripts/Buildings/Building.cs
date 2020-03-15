using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Building : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Sprite _spriteON;
    [SerializeField] private Sprite _spriteOFF;
#pragma warning restore 0649
    #endregion
    private SpriteRenderer _renderer;

    public StructureTile dataTile;

    // This object does not store data, but it's the only way to acceed it from the editor
    // Thus, the ability to turn on/off the building is done here
    [SerializeField] private bool _debugIsOn;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sortingOrder = 10;
    }

    private void Update()
    {
        dataTile.IsOn = _debugIsOn;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        _renderer.sprite = dataTile.IsOn ? _spriteON : _spriteOFF;
    }
}
