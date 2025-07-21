using UnityEngine;

public class ObjectActive : MonoBehaviour
{
    public MonoBehaviour objectsToDeactivate;
    public bool hasBeenActivated = false; 

    void Awake()
    {
        if (objectsToDeactivate != null)
            objectsToDeactivate.enabled = false;
    }

    void OnBecameVisible()
    {
        if (objectsToDeactivate != null)
        {
            objectsToDeactivate.enabled = true;
            hasBeenActivated = true;
            Debug.Log("Enemy đã hiện trong camera — kích hoạt AI.");
        }
    }

    void OnBecameInvisible()
    {
        if (objectsToDeactivate != null)
        {
            objectsToDeactivate.enabled = false;
            Debug.Log("Enemy đã rời khỏi camera — vô hiệu hóa AI.");
        }
    }
}
