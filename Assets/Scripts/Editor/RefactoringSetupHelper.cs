using UnityEditor;
using UnityEngine;

public static class RefactoringSetupHelper
{
    [MenuItem("Tools/Refactoring/Create GameConfig Asset")]
    public static void CreateGameConfigAsset()
    {
        GameConfig config = ScriptableObject.CreateInstance<GameConfig>();
        
        AssetDatabase.CreateAsset(config, "Assets/Scripts/Data/GameConfig.asset");
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;
        
        Debug.Log("GameConfig asset created at Assets/Scripts/Data/GameConfig.asset");
    }

    [MenuItem("Tools/Refactoring/Show Refactoring Guide")]
    public static void ShowRefactoringGuide()
    {
        string path = "Assets/Scripts/REFACTORING_GUIDE.md";
        Object guideAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
        
        if (guideAsset != null)
        {
            Selection.activeObject = guideAsset;
            EditorUtility.FocusProjectWindow();
        }
        else
        {
            Debug.LogWarning($"Refactoring guide not found at {path}");
        }
    }

    [MenuItem("Tools/Refactoring/Show New Scene Setup Guide")]
    public static void ShowNewSceneGuide()
    {
        string path = "Assets/Scripts/NEW_SCENE_SETUP.md";
        Object guideAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
        
        if (guideAsset != null)
        {
            Selection.activeObject = guideAsset;
            EditorUtility.FocusProjectWindow();
        }
        else
        {
            Debug.LogWarning($"New scene setup guide not found at {path}");
        }
    }

    [MenuItem("Tools/Refactoring/Open Input Actions Setup")]
    public static void ShowInputActionsGuide()
    {
        string path = "Assets/Scripts/INPUT_SYSTEM_SETUP.md";
        Object guideAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
        
        if (guideAsset != null)
        {
            Selection.activeObject = guideAsset;
            EditorUtility.FocusProjectWindow();
        }
        else
        {
            Debug.LogWarning($"Input Actions setup guide not found at {path}");
        }
    }

    [MenuItem("Tools/Refactoring/Setup Scene (Experimental)")]
    public static void AutoSetupScene()
    {
        Debug.Log("Auto-setup feature - Please manually configure according to the Refactoring Guide");
        Debug.Log("This feature helps identify GameObjects that need updating:");
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"✓ Found Player: {player.name}");
            
            if (player.GetComponent<PlayerController>() == null)
            {
                Debug.Log("  → Add PlayerController component");
                Debug.Log("  → Add PlayerInput component");
                Debug.Log("  → Remove old playerController component");
            }
        }
        else
        {
            Debug.LogWarning("✗ Player GameObject not found (needs 'Player' tag)");
        }
        
        GameObject controller = GameObject.Find("Controller");
        if (controller != null)
        {
            Debug.Log($"✓ Found Controller: {controller.name}");
            Debug.Log("  → Add GameManager component");
            Debug.Log("  → Add ItemSpawnManager component");
            Debug.Log("  → Add CollectionManager component");
            Debug.Log("  → Add GameUI component");
            Debug.Log("  → Remove old MainClass, Manager1, Manager2, AnotherManager components");
        }
        else
        {
            Debug.LogWarning("✗ Controller GameObject not found");
        }
        
        Debug.Log("\nPlease see REFACTORING_GUIDE.md for complete setup instructions");
    }
}
