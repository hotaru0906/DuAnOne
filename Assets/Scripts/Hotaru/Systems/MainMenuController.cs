using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public List<Text> menuOptions; // Thứ tự: NewGame, LoadGame, Quit
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private int selectedIndex = 0;

    void Start()
    {
        UpdateSelection();
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleMouseHover();
    }

    void HandleKeyboardInput()
    {
        bool changed = false;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuOptions.Count) % menuOptions.Count;
            changed = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuOptions.Count;
            changed = true;
        }

        if (changed)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteAction(selectedIndex);
        }
    }

    void HandleMouseHover()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            Text option = menuOptions[i];
            // Kiểm tra nếu chuột nằm trên option này
            if (RectTransformUtility.RectangleContainsScreenPoint(option.rectTransform, Input.mousePosition))
            {
                if (selectedIndex != i)
                {
                    selectedIndex = i;
                    UpdateSelection();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    ExecuteAction(i);
                }
            }
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            menuOptions[i].color = (i == selectedIndex) ? highlightColor : normalColor;
        }
    }

    void ExecuteAction(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("New Game");
                SceneController sceneController = FindObjectOfType<SceneController>();
                if (sceneController != null)
                {
                    sceneController.ChangeScene();
                }
                break;
            case 1:
                Debug.Log("Load Game");
                
                break;
            case 2:
                Debug.Log("Quit Game");
                Application.Quit();
                break;
        }
    }
}
