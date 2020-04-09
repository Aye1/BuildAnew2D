using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class PlaySoundOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    [EventRef]
    public string eventName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(eventName);
    }
}
