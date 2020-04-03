using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TurnManager.OnTurnStart += OnTurnStarted;
    }

    public void OnTurnStarted()
    {
        StartCoroutine("NextDay");
    }
    IEnumerator NextDay()
    {
        Transform transform = gameObject.GetComponent<Transform>();
        Vector3 initialPosition = transform.position;
        for (float angle = 0f; angle < 360f; angle += 20f)
        {
            transform.RotateAround(Vector3.zero, Vector3.back, 20);
            yield return new WaitForSeconds(.1f);
        }
        transform.position = initialPosition;
    }
}
