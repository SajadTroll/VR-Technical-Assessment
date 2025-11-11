using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SceneSetupAutomation : EditorWindow
{
    private string sceneName = "RefactoredDemo";
    private GameObject itemPrefab;
    private GameConfig gameConfig;
    private InputActionAsset inputActions;

    [MenuItem("Tools/Refactoring/Create New Scene Setup")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupAutomation>("Scene Setup Automation");
    }

    private void OnGUI()
    {
        GUILayout.Label("Automated Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This tool will create a new scene and set up all GameObjects with the refactored scripts.\n\n" +
            "Required assets (you must create these first):\n" +
            "1. GameConfig asset\n" +
            "2. PlayerInputActions asset\n" +
            "3. Item Prefab (optional - will create if missing)",
            MessageType.Info
        );

        GUILayout.Space(10);

        sceneName = EditorGUILayout.TextField("Scene Name:", sceneName);
        gameConfig = (GameConfig)EditorGUILayout.ObjectField("Game Config:", gameConfig, typeof(GameConfig), false);
        inputActions = (InputActionAsset)EditorGUILayout.ObjectField("Input Actions:", inputActions, typeof(InputActionAsset), false);
        itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab (Optional):", itemPrefab, typeof(GameObject), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create GameConfig Asset", GUILayout.Height(30)))
        {
            CreateGameConfig();
        }

        GUILayout.Space(5);

        EditorGUI.BeginDisabledGroup(gameConfig == null);
        if (GUILayout.Button("Setup New Scene", GUILayout.Height(40)))
        {
            SetupScene();
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);

        if (GUILayout.Button("Show Setup Guide", GUILayout.Height(30)))
        {
            ShowSetupGuide();
        }
    }

    private void CreateGameConfig()
    {
        GameConfig config = ScriptableObject.CreateInstance<GameConfig>();
        string path = "Assets/Scripts/Data/GameConfig.asset";

        if (!AssetDatabase.IsValidFolder("Assets/Scripts/Data"))
        {
            AssetDatabase.CreateFolder("Assets/Scripts", "Data");
        }

        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        gameConfig = config;
        EditorUtility.DisplayDialog("Success", "GameConfig asset created at:\n" + path, "OK");
        EditorGUIUtility.PingObject(config);
    }

    private void SetupScene()
    {
        if (gameConfig == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign GameConfig asset first!", "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog("Create New Scene",
            $"This will create a new scene '{sceneName}' with all refactored components.\n\nContinue?",
            "Yes", "Cancel"))
        {
            return;
        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        newScene.name = sceneName;

        ClearDefaultObjects();

        GameObject player = CreatePlayer();
        GameObject controller = CreateController();
        GameObject canvas = CreateCanvas();
        GameObject ground = CreateGround();

        if (itemPrefab == null)
        {
            itemPrefab = CreateItemPrefab();
        }

        WireReferences(player, controller, canvas);

        string scenePath = $"Assets/Scenes/{sceneName}.unity";
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        EditorSceneManager.SaveScene(newScene, scenePath);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success!",
            $"Scene '{sceneName}' created successfully!\n\n" +
            "Scene saved at: " + scenePath + "\n\n" +
            "Press Play to test!",
            "OK");

        Selection.activeGameObject = player;
    }

    private void ClearDefaultObjects()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null) DestroyImmediate(mainCamera);

        GameObject directionalLight = GameObject.Find("Directional Light");
        if (directionalLight != null) DestroyImmediate(directionalLight);
    }

    private GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0, 1, 0);

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        PlayerController playerController = player.AddComponent<PlayerController>();

        if (inputActions != null)
        {
            PlayerInput playerInput = player.AddComponent<PlayerInput>();
            playerInput.actions = inputActions;
            playerInput.defaultActionMap = "Player";
        }
        else
        {
            Debug.LogWarning("PlayerInputActions not assigned. You'll need to add PlayerInput component manually.");
        }

        GameObject camera = new GameObject("Main Camera");
        camera.transform.SetParent(player.transform);
        camera.transform.localPosition = new Vector3(0, 12, -15);
        camera.transform.localRotation = Quaternion.Euler(30, 0, 0);
        camera.tag = "MainCamera";
        camera.AddComponent<Camera>();

        CreatePlayerLights(player);

        return player;
    }

    private void CreatePlayerLights(GameObject player)
    {
        GameObject lightHolder1 = new GameObject("LightHolder1");
        lightHolder1.transform.SetParent(player.transform);
        lightHolder1.transform.localPosition = new Vector3(5, 3, 5);

        GameObject spotLight1 = new GameObject("Spot Light");
        spotLight1.transform.SetParent(lightHolder1.transform);
        spotLight1.transform.localPosition = Vector3.zero;
        spotLight1.transform.localRotation = Quaternion.Euler(45, -45, 0);
        Light light1 = spotLight1.AddComponent<Light>();
        light1.type = LightType.Spot;
        light1.range = 30;
        light1.spotAngle = 60;
        light1.intensity = 1;

        GameObject lightHolder2 = new GameObject("LightHolder2");
        lightHolder2.transform.SetParent(player.transform);
        lightHolder2.transform.localPosition = new Vector3(-5, 3, -5);

        GameObject spotLight2 = new GameObject("Spot Light");
        spotLight2.transform.SetParent(lightHolder2.transform);
        spotLight2.transform.localPosition = Vector3.zero;
        spotLight2.transform.localRotation = Quaternion.Euler(45, 135, 0);
        Light light2 = spotLight2.AddComponent<Light>();
        light2.type = LightType.Spot;
        light2.range = 30;
        light2.spotAngle = 60;
        light2.intensity = 1;
    }

    private GameObject CreateController()
    {
        GameObject controller = new GameObject("Controller");
        controller.transform.position = Vector3.zero;

        controller.AddComponent<GameManager>();
        controller.AddComponent<ItemSpawnManager>();
        controller.AddComponent<CollectionManager>();
        controller.AddComponent<GameUI>();

        return controller;
    }

    private GameObject CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        GameObject textGO = new GameObject("InfoText");
        textGO.transform.SetParent(canvasGO.transform);

        RectTransform rectTransform = textGO.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(20, -20);
        rectTransform.sizeDelta = new Vector2(400, 60);

        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "Collected: 0/50 Avg: 0.0";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.TopLeft;

        return canvasGO;
    }

    private GameObject CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(5, 1, 5);

        return ground;
    }

    private GameObject CreateItemPrefab()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = "CollectibleItem";
        item.tag = "Item";

        item.AddComponent<CollectibleItem>();

        string prefabPath = "Assets/Prefabs/CollectibleItem.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(item, prefabPath);
        DestroyImmediate(item);

        Debug.Log("Item prefab created at: " + prefabPath);
        return prefab;
    }

    private void WireReferences(GameObject player, GameObject controller, GameObject canvas)
    {
        GameManager gameManager = controller.GetComponent<GameManager>();
        ItemSpawnManager spawnManager = controller.GetComponent<ItemSpawnManager>();
        CollectionManager collectionManager = controller.GetComponent<CollectionManager>();
        GameUI gameUI = controller.GetComponent<GameUI>();

        SerializedObject gmSO = new SerializedObject(gameManager);
        gmSO.FindProperty("spawnManager").objectReferenceValue = spawnManager;
        gmSO.FindProperty("collectionManager").objectReferenceValue = collectionManager;
        gmSO.ApplyModifiedProperties();

        SerializedObject smSO = new SerializedObject(spawnManager);
        smSO.FindProperty("itemPrefab").objectReferenceValue = itemPrefab;
        smSO.FindProperty("config").objectReferenceValue = gameConfig;
        smSO.FindProperty("playerTransform").objectReferenceValue = player.transform;

        Light[] lights = player.GetComponentsInChildren<Light>();
        SerializedProperty lightsProp = smSO.FindProperty("sceneLights");
        lightsProp.arraySize = lights.Length;
        for (int i = 0; i < lights.Length; i++)
        {
            lightsProp.GetArrayElementAtIndex(i).objectReferenceValue = lights[i];
        }
        smSO.ApplyModifiedProperties();

        TextMeshProUGUI infoText = canvas.GetComponentInChildren<TextMeshProUGUI>();
        SerializedObject guiSO = new SerializedObject(gameUI);
        guiSO.FindProperty("infoText").objectReferenceValue = infoText;
        guiSO.FindProperty("playerTransform").objectReferenceValue = player.transform;
        guiSO.FindProperty("spawnManager").objectReferenceValue = spawnManager;
        guiSO.FindProperty("collectionManager").objectReferenceValue = collectionManager;
        guiSO.FindProperty("maxItems").intValue = 50;
        guiSO.ApplyModifiedProperties();

        Debug.Log("All references wired successfully!");
    }

    private void ShowSetupGuide()
    {
        string guidePath = "Assets/Scripts/NEW_SCENE_SETUP.md";
        TextAsset guide = AssetDatabase.LoadAssetAtPath<TextAsset>(guidePath);
        if (guide != null)
        {
            EditorGUIUtility.PingObject(guide);
            Selection.activeObject = guide;
        }
        else
        {
            EditorUtility.DisplayDialog("Guide Not Found",
                "Could not find NEW_SCENE_SETUP.md\n\nPlease check Assets/Scripts/ folder.",
                "OK");
        }
    }
}
