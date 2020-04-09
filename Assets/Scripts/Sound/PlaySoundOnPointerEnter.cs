using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;

public class PlaySoundOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    [EventRef]
    public string eventName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer enter");
        RuntimeManager.PlayOneShot(eventName);
    }
}
