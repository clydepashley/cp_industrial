using UnityEngine;

public class StaggeredAnimatorPlayback : MonoBehaviour
{
    [Header("Targets")]

    [Tooltip("Animators to stagger.")]
    [SerializeField] private Animator[] animators;

    [Header("Animation Settings")]

    [Tooltip("Name of the Animator state to play.")]
    [SerializeField] private string animationStateName = "Idle";

    [Tooltip("Animator layer index.")]
    [SerializeField] private int layer = 0;

    [Tooltip("Play automatically on Awake.")]
    [SerializeField] private bool playOnAwake = true;

    private void Awake()
    {
        if (playOnAwake)
        {
            PlayStaggered();
        }
    }

    /// <summary>
    /// Plays the animation on all animators, staggered evenly across normalized time.
    /// </summary>
    public void PlayStaggered()
    {

        if (animators == null || animators.Length == 0)
            return;

        int count = animators.Length;

        for (int i = 0; i < count; i++)
        {
            Animator animator = animators[i];
            if (animator == null)
                continue;

            float normalizedTime = (float)i / count;

            animator.Play(animationStateName, layer, normalizedTime);
            animator.speed = 1f;
        }
    }
}
