using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public TimelineCutsceneEvents timelineEvents;
    public GameObject timelineObject; // Reference to the Timeline GameObject

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timelineEvents.timeline.Play(); // chạy Timeline
            gameObject.SetActive(false);    // chỉ chạy 1 lần
            timelineObject.SetActive(true); // bật Timeline GameObject
        }
    }
}
