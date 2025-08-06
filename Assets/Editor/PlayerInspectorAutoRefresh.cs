using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerInspectorAutoRefresh : Editor
{
    public override void OnInspectorGUI()
    {
        // Force Unity to update the Inspector every frame in Play Mode
        if (Application.isPlaying)
        {
            Repaint();
        }
        base.OnInspectorGUI();
    }
}
