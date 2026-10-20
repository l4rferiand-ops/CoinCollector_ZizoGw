using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemDetails ItemDetails;
    private GrabableItem itemCollector;
    private bool sudahDiambil;

    void Awake()
    {
        foreach (MonoBehaviour component in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (component is GrabableItem collector)
            {
                itemCollector = collector;
                break;
            }
        }

        if (itemCollector == null)
        {
            Debug.LogError("No active component implementing GrabableItem was found in the scene.", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (sudahDiambil) return;
        if (!other.CompareTag("Player") || ItemDetails == null || itemCollector == null) return;

        sudahDiambil = true;
        itemCollector.AddItem(ItemDetails);

        // Koin tetap dihitung untuk skor / kondisi menang di GameManager
        if (CompareTag("Coin"))
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            }
        }

        Destroy(gameObject);
    }
}
