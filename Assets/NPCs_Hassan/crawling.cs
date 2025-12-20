using UnityEngine;

public class CrawlThenUnconscious : MonoBehaviour
{
    public Animator animator;
    public float crawlDuration = 30f;

    private float timer = 0f;
    private bool hasSwitched = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Play crawling animation at start
        animator.Play("Crawl");  // must match the state name in Animator
    }

    void Update()
    {
        if (hasSwitched) return;

        timer += Time.deltaTime;

        if (timer >= crawlDuration)
        {
            hasSwitched = true;

            // Play unconscious animation
            animator.Play("Unconscious");  // must match the state name in Animator
        }
    }
}
