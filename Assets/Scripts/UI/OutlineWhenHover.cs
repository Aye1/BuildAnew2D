using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineWhenHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Material _outlineMaterial;

    private void Start()
    {
        _outlineMaterial = GetComponent<Renderer>().material;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_outlineMaterial != null)
        {
            _outlineMaterial.SetFloat("_OutlineActive", 1.0f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_outlineMaterial != null)
        {
            _outlineMaterial.SetFloat("_OutlineActive", 0.0f);
        }
    }
}
