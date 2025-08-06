using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public List<string> dialogueLines; // Các câu thoại của NPC

    [Header("GameObject to Activate")]
    public GameObject objectToActivate; // GameObject sẽ được bật sau khi hội thoại kết thúc
    public GameObject UIobject; // GameObject UI sẽ được bật khi bắt đầu đối thoại

    private bool isPlayerInRange = false; // Kiểm tra nếu player đang trong vùng tương tác
    private bool isDialogueActive = false; // Kiểm tra nếu đối thoại đang diễn ra

    void Update()
    {
        // Chỉ bắt đầu đối thoại nếu chưa bắt đầu
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
            isDialogueActive = true; // Đánh dấu đối thoại đang diễn ra ngay khi nhấn E
            StartDialogue();
            Invoke(nameof(ActivateObject), 2f);
        }
    }

    void StartDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            isDialogueActive = true; // Đánh dấu đối thoại đang diễn ra
            DialogueManager.Instance.StartDialogue(dialogueLines, this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            UIobject.SetActive(true); // Bật GameObject UI khi player vào vùng tương tác
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            isDialogueActive = false; // Đánh dấu đối thoại đã kết thúc
            UIobject.SetActive(false); // Tắt GameObject UI khi player rời khỏi vùng tương tác
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.CloseDialogueBox();
                DialogueManager.Instance.StopSound();
            }
            objectToActivate.SetActive(false); // Tắt GameObject nếu player rời khỏi vùng tương tác
            CancelInvoke(nameof(ActivateObject)); // Hủy Invoke nếu player rời khỏi vùng
        }
    }

    // Optional: Gọi khi đối thoại kết thúc
    public void OnDialogueEnded()
    {
        Debug.Log("Dialogue with NPC ended.");

        isDialogueActive = false; // Đánh dấu đối thoại đã kết thúc

        if (objectToActivate != null)
        {
            Debug.Log($"objectToActivate is assigned: {objectToActivate.name}");
            Debug.Log($"objectToActivate active state before: {objectToActivate.activeSelf}");

            // Kích hoạt GameObject sau 2 giây
            
        }
        else
        {
            Debug.LogWarning("objectToActivate is not set in the Inspector.");
        }
    }

    private void ActivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"objectToActivate active state after: {objectToActivate.activeSelf}");
        }
    }
}
