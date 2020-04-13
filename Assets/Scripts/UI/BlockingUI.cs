using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockingUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static List<BlockingUI> _blockingUIs;
    public static bool IsBlocked
    {
        get
        {
            return _blockingUIs.Count > 0;
        }
    }

    private void Awake()
    {
        if(_blockingUIs == null)
        {
            _blockingUIs = new List<BlockingUI>();
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_blockingUIs.Contains(this))
        {
            _blockingUIs.Add(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _blockingUIs.Remove(this);
    }
}
