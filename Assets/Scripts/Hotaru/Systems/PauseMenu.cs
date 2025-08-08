using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    public GameObject pauseMenu; // Menu tạm dừng
    public GameObject player; // Tham chiếu đến đối tượng Player

    void Update()
    {
        // Kiểm tra nếu nhấn phím Escape để tạm dừng hoặc tiếp tục trò chơi
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        player.GetComponent<Player>().enabled = false; // Tắt điều khiển của Player
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        player.GetComponent<Player>().enabled = true; // Bật điều khiển của Player
    }
}
