using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EnergyIndicator : MonoBehaviour
{
    private bool _isOn;
    public bool IsOn
    {
        get { return _isOn; }
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                UpdateSprite();
            }
        }
    }

#pragma warning disable 0649
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private Sprite spriteOn;
#pragma warning restore 0649

    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void UpdateSprite()
    {
        _image.sprite = _isOn ? spriteOn : spriteOff;
    }
}
