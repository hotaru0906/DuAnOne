using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class QuickSceneSwitcher : EditorWindow
{
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private List<SceneInfo> allScenes = new List<SceneInfo>();
    private List<SceneInfo> filteredScenes = new List<SceneInfo>();
    
    [System.Serializable]
    public class SceneInfo
    {
        public string name;
        public string path;
        public string folder;
        
        public SceneInfo(string scenePath)
        {
            path = scenePath;
            name = Path.GetFileNameWithoutExtension(scenePath);
            folder = Path.GetDirectoryName(scenePath).Replace("\\", "/");
        }
    }

    [MenuItem("Tools/Quick Scene Switcher")]
    public static void ShowWindow()
    {
        QuickSceneSwitcher window = GetWindow<QuickSceneSwitcher>("Scene Switcher");
        window.minSize = new Vector2(300, 400);
        window.RefreshSceneList();
    }

    void OnEnable()
    {
        RefreshSceneList();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        // Header
        EditorGUILayout.LabelField("Quick Scene Switcher", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Refresh button and search
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔄 Refresh", GUILayout.Width(80)))
        {
            RefreshSceneList();
        }
        
        EditorGUI.BeginChangeCheck();
        searchFilter = EditorGUILayout.TextField("🔍 Search:", searchFilter);
        if (EditorGUI.EndChangeCheck())
        {
            FilterScenes();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Current scene info
        string currentScene = EditorSceneManager.GetActiveScene().name;
        EditorGUILayout.LabelField($"Current: {currentScene}", EditorStyles.helpBox);
        EditorGUILayout.Space();
        
        // Scene list with scroll
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        if (filteredScenes.Count == 0)
        {
            EditorGUILayout.LabelField("No scenes found!", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            string lastFolder = "";
            foreach (var scene in filteredScenes)
            {
                // Group by folder
                if (scene.folder != lastFolder)
                {
                    if (!string.IsNullOrEmpty(lastFolder))
                        EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField($"📁 {scene.folder}", EditorStyles.miniLabel);
                    lastFolder = scene.folder;
                }
                
                // Scene button
                EditorGUILayout.BeginHorizontal();
                
                // Highlight current scene
                bool isCurrent = scene.name == currentScene;
                if (isCurrent)
                    GUI.backgroundColor = Color.green;
                
                if (GUILayout.Button($"🎬 {scene.name}", GUILayout.Height(25)))
                {
                    OpenScene(scene.path);
                }
                
                if (isCurrent)
                    GUI.backgroundColor = Color.white;
                
                // Quick actions
                if (GUILayout.Button("📋", GUILayout.Width(30), GUILayout.Height(25)))
                {
                    EditorGUIUtility.systemCopyBuffer = scene.path;
                    Debug.Log($"📋 Copied path: {scene.path}");
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        // Footer
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Total scenes: {allScenes.Count} | Filtered: {filteredScenes.Count}", EditorStyles.centeredGreyMiniLabel);
        
        EditorGUILayout.EndVertical();
    }

    void RefreshSceneList()
    {
        allScenes.Clear();
        
        // Find all .unity files in the project
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            allScenes.Add(new SceneInfo(scenePath));
        }
        
        // Sort by folder then by name
        allScenes = allScenes.OrderBy(s => s.folder).ThenBy(s => s.name).ToList();
        
        FilterScenes();
        Debug.Log($"🔄 Found {allScenes.Count} scenes in project");
    }

    void FilterScenes()
    {
        if (string.IsNullOrEmpty(searchFilter))
        {
            filteredScenes = new List<SceneInfo>(allScenes);
        }
        else
        {
            filteredScenes = allScenes.Where(s => 
                s.name.ToLower().Contains(searchFilter.ToLower()) ||
                s.folder.ToLower().Contains(searchFilter.ToLower())
            ).ToList();
        }
    }

    void OpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
            Debug.Log($"🎬 Opened scene: {Path.GetFileNameWithoutExtension(path)}");
        }
    }
}