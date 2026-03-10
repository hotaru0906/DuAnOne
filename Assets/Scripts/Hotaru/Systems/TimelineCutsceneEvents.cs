using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class TimelineCutsceneEvents : MonoBehaviour
{
    // Hàm chờ người chơi (ví dụ: chờ player đến vị trí hoặc thực hiện hành động)

    [System.Serializable]
    public class DialogueBlock
    {
        [TextArea(2, 5)]
        public List<string> lines;
        public int cameraIndex; // 0 = Player, 1 = King, 2 = Goddess
    }
    public List<DialogueBlock> dialogues;
    public List<CinemachineVirtualCamera> cameras;
    public GameObject player, boss;
    public GameObject statusUI;
    public GameObject UIGameObject;
    public GameObject box1, box2;
    public PlayableDirector timeline;

    public List<GameObject> npcs; // Danh sách các NPC cần bật

    private int currentIndex = 0;
    private bool waitingForStatus = false;
    public IEnumerator WaitForPlayer(System.Func<bool> condition)
    {
        // Lặp cho đến khi điều kiện thỏa mãn
        while (!condition())
        {
            yield return null;
        }
        // Khi điều kiện đúng, tiếp tục cutscene
        Debug.Log("Player đã sẵn sàng, tiếp tục cutscene.");
    }
    public void DisablePlayerControl()
    {
        // Disable the Player script
        player.GetComponent<Player>().enabled = false;

        // Stop the player's movement
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Set the player's animation to Idle
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Idle");
        }

        // Stop the player's movement sound
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.StopMoveSound();
        }
    }

    public void EnablePlayerControl()
    {
        //player.GetComponent<TestPlayer>().enabled = true;
        player.GetComponent<Player>().enabled = true;
    }
    public void EnableNPCControl()
    {
        foreach (GameObject npc in npcs)
        {
            if (npc != null)
            {
                NPC npcScript = npc.GetComponent<NPC>();
                if (npcScript != null)
                {
                    npcScript.SetCanMove(true);
                }
            }
        }
    }
    public void DisableUI()
    {
        if (statusUI != null)
        {
            statusUI.SetActive(false);
        }
        if (UIGameObject != null)
        {
            UIGameObject.SetActive(false);
        }
    }
    public void EnableUI()
    {
        if (statusUI != null)
        {
            statusUI.SetActive(true);
        }
        if (UIGameObject != null)
        {
            UIGameObject.SetActive(true);
        }
    }

    public void PlayNextDialogue()
    {
        if (currentIndex < dialogues.Count)
        {
            DialogueBlock current = dialogues[currentIndex];
            SwitchCamera(current.cameraIndex);
            DialogueManager.Instance.StartDialogue(current.lines, this);
            currentIndex++;
        }
    }

    public void OnDialogueEnded()
    {
        // sau khi nữ thần nói "Nhấn I để mở trạng thái" thì sẽ chờ tại đây
        if (waitingForStatus == false)
        {
            PlayNextDialogue(); // tiếp tục nếu không phải đoạn đặc biệt
        }
    }

    public void SwitchCamera(int index)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].Priority = (i == index) ? 10 : 0;
        }
    }

    // ← SIGNAL GỌI TỚI ĐÂY
    public void WaitForPlayerPressI()
    {
        timeline.Pause();
        waitingForStatus = true;
        Debug.Log("⏸ Timeline tạm dừng - chờ nhấn I");
    }

    void Update()
    {
        if (waitingForStatus && Input.GetKeyDown(KeyCode.I))
        {
            statusUI.SetActive(true);
            Debug.Log("🟢 Mở UI trạng thái");

            // Giả lập đóng sau vài giây, bạn có thể dùng UI button gọi ResumeCutscene() thật
            Invoke(nameof(ResumeCutscene), 3f);
            waitingForStatus = false;
        }
    }

    public void ResumeCutscene()
    {
        statusUI.SetActive(false);
        Debug.Log("▶️ Tiếp tục Timeline");
        timeline.Resume();
        // Nếu đoạn tiếp theo cần thoại → gọi tiếp
        PlayNextDialogue();
    }
    public void CloseDiaBox()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.CloseDialogueBox();
        }
    }
    public void BattleHitbox()
    {
        if (box1 != null)
        {
            box1.SetActive(true);
        }
        if (box2 != null)
        {
            box2.SetActive(true);
        }
    }
    public void BossActive()
    {
        if (boss != null)
        {
            var b3 = boss.GetComponent<Boss3>();
            if (b3 != null) b3.enabled = true;
            var b6 = boss.GetComponent<Boss6>();
            if (b6 != null) b6.enabled = true;
            var b5 = boss.GetComponent<Boss5>();
            if (b5 != null) b5.enabled = true;
            var b4 = boss.GetComponent<Boss4>();
            if (b4 != null) b4.enabled = true;
            var b1 = boss.GetComponent<Boss1>();
            if (b1 != null) b1.enabled = true;
        }
    }

    public void ChangeBGM(AudioClip newBGM)
    {
        {
            if (BGMController.Instance != null && BGMController.Instance.bgmSource != null)
            {
                // Stop the current BGM
                BGMController.Instance.bgmSource.Stop(); BGMController.Instance.bgmSource.Stop();

                // Set and play the new BGM   // Set and play the new BGM
                BGMController.Instance.bgmSource.clip = newBGM;
                BGMController.Instance.bgmSource.Play(); BGMController.Instance.bgmSource.Play();
            }
            else
            {
                Debug.LogWarning("BGMController or its AudioSource is not set up correctly.");
            }
        }
    }
}
