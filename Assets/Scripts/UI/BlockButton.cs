using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BlockButton : MonoBehaviour
{
    private Button _button;
    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        _button.interactable = !UIManager.Instance.IsBlocked;
    }
}
