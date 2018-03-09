using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class HitscanTracer : Photon.MonoBehaviour
{
    [SerializeField] float _lifeTime   = 1.0f;
    [SerializeField] int   _fadeFactor = 2;

    LineRenderer _lineRenderer;


    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();    
    }


    [PunRPC]
    public void _NetInitialize(Vector3[] inPoints)
    {
        Debug.Log(inPoints.Length);
        _lineRenderer.positionCount = inPoints.Length;
        _lineRenderer.SetPositions(inPoints);
        Timing.RunCoroutine(_HandleFade());
    }


    IEnumerator<float> _HandleFade()
    {
        float timer = 0.0f;
        while (timer < _lifeTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.InverseLerp(0, _lifeTime, timer);

            SetLineFade(Mathf.Pow(progress, _fadeFactor));
            yield return Timing.WaitForOneFrame;
        }

        Destroy(gameObject);
    }

    void SetLineFade(float inFadeProgress)
    {
        Color fadeColor = _lineRenderer.material.color;
        fadeColor.a = Mathf.InverseLerp(1, 0, inFadeProgress);
        _lineRenderer.material.color = fadeColor;

        Debug.Log(_lineRenderer.material.color.a);
    }
}
