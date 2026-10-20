#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menu: Tools > Setup Inventory
// Membuat Canvas inventory + InventoryManager di scene aktif,
// memastikan ada EventSystem, dan memasang komponen Item di semua objek ber-tag "Coin".
public static class InventorySetup
{
    [MenuItem("Tools/Setup Inventory")]
    public static void Setup()
    {
        if (Object.FindFirstObjectByType<InventoryManager>() != null)
        {
            Debug.LogWarning("InventoryManager sudah ada di scene ini, setup dibatalkan.");
            return;
        }

        // ---------- Canvas ----------
        GameObject canvasGO = new GameObject("InventoryCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Setup Inventory");

        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // ---------- Panel ----------
        GameObject panel = new GameObject("InventoryPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(620, 620);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

        // ---------- Container slot (GridLayoutGroup) ----------
        GameObject slots = new GameObject("InventorySlots", typeof(RectTransform), typeof(GridLayoutGroup));
        slots.transform.SetParent(panel.transform, false);
        RectTransform slotsRect = slots.GetComponent<RectTransform>();
        slotsRect.anchorMin = Vector2.zero;
        slotsRect.anchorMax = Vector2.one;
        slotsRect.offsetMin = new Vector2(20, 20);
        slotsRect.offsetMax = new Vector2(-20, -20);

        GridLayoutGroup grid = slots.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(125, 125);
        grid.spacing = new Vector2(10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;
        grid.childAlignment = TextAnchor.MiddleCenter;

        // ---------- InventoryManager ----------
        GameObject managerGO = new GameObject("InventoryManager", typeof(InventoryManager));
        Undo.RegisterCreatedObjectUndo(managerGO, "Setup Inventory");
        SerializedObject managerSO = new SerializedObject(managerGO.GetComponent<InventoryManager>());
        managerSO.FindProperty("InventoryCanvas").objectReferenceValue = canvasGO;
        managerSO.ApplyModifiedProperties();

        // ---------- EventSystem (dibutuhkan untuk klik slot) ----------
        EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject esGO = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(esGO, "Setup Inventory");
        }
        else if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
        {
            StandaloneInputModule oldModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (oldModule != null)
            {
                Undo.DestroyObjectImmediate(oldModule);
            }
            Undo.AddComponent<InputSystemUIInputModule>(eventSystem.gameObject);
        }

        // ---------- Pasang Item di semua koin ----------
        int coinCount = 0;
        ItemDetails coinData = AssetDatabase.LoadAssetAtPath<ItemDetails>("Assets/ScriptableObjects/ItemCoin.asset");
        if (coinData == null)
        {
            Debug.LogWarning("Assets/ScriptableObjects/ItemCoin.asset tidak ditemukan, koin tidak dipasangi Item.");
        }
        else
        {
            try
            {
                foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Coin"))
                {
                    Item item = coin.GetComponent<Item>();
                    if (item == null)
                    {
                        item = Undo.AddComponent<Item>(coin);
                    }

                    SerializedObject itemSO = new SerializedObject(item);
                    itemSO.FindProperty("ItemDetails").objectReferenceValue = coinData;
                    itemSO.ApplyModifiedProperties();
                    coinCount++;
                }
            }
            catch (UnityException e)
            {
                Debug.LogWarning("Tag 'Coin' belum ada: " + e.Message);
            }
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Setup Inventory selesai. Koin yang dipasangi Item: " + coinCount + ". Jangan lupa Save scene (Ctrl+S).");
    }
}
#endif
