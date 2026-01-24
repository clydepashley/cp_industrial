using UnityEngine;
using UnityEngine.Splines;

public class Splines_AnimateHeirarchyWithOffset : MonoBehaviour
{
    [SerializeField] internal float timeOffset = 0.02f;
    [SerializeField] internal SplineAnimate[] splineAnimators;
    [SerializeField] internal float normalizedTime;

    private void Update()
    {
        for (int i = 0; i < splineAnimators.Length; i++)
        {
            float offsetTime = normalizedTime - i * timeOffset;
            offsetTime = Mathf.Clamp01(offsetTime);
            SplineAnimate sAnimator = splineAnimators[i];
            sAnimator.NormalizedTime = offsetTime;
        }
    }
}
