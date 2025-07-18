using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class TimelineCutsceneEvents : MonoBehaviour
{
    [System.Serializable]
    public class DialogueBlock
    {
        [TextArea(2, 5)]
        public List<string> lines;
        public int cameraIndex; // 0 = Player, 1 = King, 2 = Goddess
    }
    public List<DialogueBlock> dialogues;
    public List<CinemachineVirtualCamera> cameras;
    public GameObject player;
    public GameObject statusUI; // UI trạng thái
    public PlayableDirector timeline;

    private int currentIndex = 0;
    private bool waitingForStatus = false;

    public void DisablePlayerControl()
    {
        player.GetComponent<TestPlayer>().enabled = false;
    }

    public void EnablePlayerControl()
    {
        player.GetComponent<TestPlayer>().enabled = true;
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
    
}
