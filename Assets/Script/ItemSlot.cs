using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item Data
    public string itemName;
    public int itemQuantity;
    public Sprite itemIcon;
    public bool isFull;

    // Item Slot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private int maxStackSize = 99;

    // Selected Item
    [SerializeField] public GameObject selectedPanel;
    public bool isItemSelected;

    public InventoryManager inventoryManager;

    public void Start()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    public void AddItem(ItemDetails item)
    {
        itemName = item.ItemName;
        // itemQuantity = item.Quantity;
        itemIcon = item.ItemIcon;
        AddQuantity(item.Quantity);
        // isFull = true;

        // quantityText.text = itemQuantity.ToString();
        // quantityText.enabled = true;
        // itemImage.sprite = itemIcon;
    }

    // Check conditions for stacking items in the same slot
    public bool CanStackWith(string otherItemName)
    {
        return !isFull && itemName == otherItemName;
    }

    public void AddQuantity(int amount)
    {
        itemQuantity += amount;
        isFull = itemQuantity >= maxStackSize;
        
        quantityText.text = itemQuantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LeftClick();
        }
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick();
        }
    }
    private void LeftClick()
    {
        Debug.Log("Left Clicked on ");
        inventoryManager.DesselectAllSlot();
        selectedPanel.SetActive(true);
        isItemSelected = true;
    }
    private void RightClick()
    {
        
        Debug.Log("Right Clicked on ");
    }
}
