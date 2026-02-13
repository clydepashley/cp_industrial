using UnityEngine;

public static class ProximityUtils
{
    /// <summary>
    /// Calculates a proximity factor based on distance, inner radius, and fade size.
    /// Returns 1 if distance <= innerRadius,
    /// Returns 0 if distance >= innerRadius + fadeSize,
    /// Lerps linearly from 1 to 0 between innerRadius and innerRadius + fadeSize.
    /// </summary>
    public static float ProximityFade(float distance, float innerRadius, float fadeSize)
    {
        if (distance <= innerRadius)
            return 1f;
        if (distance >= innerRadius + fadeSize)
            return 0f;

        float t = (distance - innerRadius) / fadeSize;
        return 1f - t; // lerp from 1 to 0
    }

    public static float InverseProximityFade(float distance, float innerRadius, float fadeSize)
    {
        if (distance <= innerRadius)
            return 0f;
        if (distance >= innerRadius + fadeSize)
            return 1f;

        float t = (distance - innerRadius) / fadeSize;
        return t; // lerp from 0 to 1
    }

    public static float SmoothProximityFade(float distance, float innerRadius, float fadeSize)
    {
        if (distance <= innerRadius)
            return 1f;
        if (distance >= innerRadius + fadeSize)
            return 0f;

        float t = (distance - innerRadius) / fadeSize;
        return Mathf.SmoothStep(1f, 0f, t);
    }

    public static float ProximityStep(float distance, float innerRadius)
    {
        return distance <= innerRadius ? 1f : 0f;
    }

    public static float InverseSquareProximity(float distance, float maxDistance)
    {
        if (distance >= maxDistance)
            return 0f;

        float normalizedDistance = distance / maxDistance;
        return 1f / (normalizedDistance * normalizedDistance + 1f); // +1 to avoid division by zero
    }
}