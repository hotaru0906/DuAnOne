using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DevController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("Forest");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SceneManager.LoadScene("tower1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SceneManager.LoadScene("tower2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SceneManager.LoadScene("dungeon");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SceneManager.LoadScene("dungeon 1");
        }
    }
}