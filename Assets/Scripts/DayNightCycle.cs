using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Color _dayColor;
    public Color _dawnColor;
    public Color _nightColor;
    public float _maxIntensity =2.0f;
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
        Light2D light = gameObject.GetComponent<Light2D>();
        light.color = Color.yellow;
        for (float angle = 0f; angle < 360f; angle += 1f)
        {
            float angleRatio = angle/360.0f;
            float rotationAngle = 1.0f;
            Color lerpedColor = Color.yellow;
            if (angle >= 0 && angle < 45)
            {
                angleRatio = angle / 45.0f;
                lerpedColor = Color.Lerp(_dayColor, _dawnColor, angleRatio);
            }
            else if(angle >= 45 && angle < 90)
            {
                angleRatio = (angle-45) / 45.0f;
                lerpedColor = Color.Lerp(_dawnColor, _nightColor, angleRatio);
                light.intensity = Mathf.Lerp(_maxIntensity, 0.5f, angleRatio);

            }
            else if (angle >= 90 && angle < 270)
            {
                angleRatio = (angle-90) / 45.0f;
                angle += 10;
                rotationAngle = 10.0f;
                lerpedColor = Color.blue;
                light.intensity = 0.5f;
            }
            else if (angle >= 270 && angle < 315)
            {
                angleRatio = (angle-270) / 45.0f;
                lerpedColor = Color.Lerp(_nightColor, _dawnColor, angleRatio);
                light.intensity = Mathf.LerpAngle(0.5f, _maxIntensity, angleRatio);

            }
            else if (angle >= 315 && angle < 360)
            {
                angleRatio = (angle-315) / 45.0f;
                lerpedColor = Color.Lerp(_dawnColor, _dayColor, angleRatio);
                light.intensity = 1.0f;
            }
            light.color = lerpedColor;
            transform.RotateAround(Vector3.zero, new Vector3(0,0.5f,-1.0f), rotationAngle);
            yield return new WaitForSeconds(.01f);
        }
        transform.position = initialPosition;
    }
}
