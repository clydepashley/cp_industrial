using UnityEngine;
using UnityEngine.Splines;

[DisallowMultipleComponent]
public class Splines_AnimateNormalizedTimeWrapper : MonoBehaviour
{
    public SplineAnimate splineAnimate;

    [SerializeField, Range(0f, 1f)]
    public float _normalizedTime;

    private float _lastNormalizedTime = -1f;

    private void Awake()
    {
        if (splineAnimate == null)
            splineAnimate = GetComponent<SplineAnimate>();
    }

    private void Update()
    {
        if (_lastNormalizedTime != _normalizedTime)
        {
            splineAnimate.NormalizedTime = _normalizedTime;
            _lastNormalizedTime = _normalizedTime;
        }
    }
}