using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ModuleUpgrade : MonoBehaviour
{
    private bool _isOn = false;
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

    private Sprite _spriteOff;
    private Sprite _spriteOn;
    private AbstractModuleScriptable _abstractModule;
    private StructureTile _tile;
    private Image _image;
    private Button _button;
    public delegate void ClickDelegate(ModuleUpgrade sender);

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
    public void InitModule(AbstractModuleScriptable abstractModuleScriptable, StructureTile tile)
    {
        _button = GetComponent<Button>();
        _image = _button.image;
        _spriteOff = abstractModuleScriptable.spriteOff;
        _spriteOn = abstractModuleScriptable.spriteOn;
        _abstractModule = abstractModuleScriptable;
        _tile = tile;
        IsOn = tile.IsModuleActive(abstractModuleScriptable);
        UpdateSprite();
        if(IsOn == false)
        {
            _button.onClick.AddListener(() => ActivateModule());
        }
    }
    private void UpdateSprite()
    {
        _image.sprite = _isOn ? _spriteOn : _spriteOff;
    }

    private void ActivateModule()
    {
        IsOn = true;
         _tile.AddModule(_abstractModule);
        _button.onClick.RemoveAllListeners();
    }
}
