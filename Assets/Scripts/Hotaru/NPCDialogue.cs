using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public List<string> dialogueLines; // Các câu thoại của NPC

    [Header("GameObject to Activate")]
    public GameObject objectToActivate; // GameObject sẽ được bật sau khi hội thoại kết thúc

    private bool isPlayerInRange = false; // Kiểm tra nếu player đang trong vùng tương tác

    void Update()
    {
        // Kiểm tra nếu player nhấn phím "E" để bắt đầu đối thoại
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueLines, this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.CloseDialogueBox();
            }
        }
    }

    // Optional: Gọi khi đối thoại kết thúc
    public void OnDialogueEnded()
    {
        Debug.Log("Dialogue with NPC ended.");

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true); // Bật GameObject
            Debug.Log($"Activated GameObject: {objectToActivate.name}");
        }
    }
}
