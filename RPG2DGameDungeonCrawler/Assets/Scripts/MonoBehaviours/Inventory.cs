using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public GameObject slotPrefab;
	public const int numSlots = 3;
	Image[] itemImages = new Image[numSlots];
	Item[] items = new Item[numSlots];
	GameObject[] slots = new GameObject[numSlots];

	public void Start()
	{
		CreateSlots();
	}

	void Update()
	{
		UpdateUI();
	}

	public void UpdateUI()
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] != null)
			{
				Debug.Log(items[i].quantity);
				Slot slotScript = slots[i].GetComponent<Slot>();
				Text quantityText = slotScript.qtyText;
				if(items[i].quantity > 0)
				{
					itemImages[i].enabled = true;
					quantityText.enabled = true;
					quantityText.text = items[i].quantity.ToString();
				}
				else
				{
					quantityText.enabled = false;
					itemImages[i].enabled = false;
				}
			}
		}
	}

	public void CreateSlots()
	{
		if (slotPrefab != null)
		{
			for (int i = 0; i < numSlots; i++)
			{
				GameObject newSlot = Instantiate(slotPrefab);
				newSlot.name = "ItemSlot_" + i;
				newSlot.transform.SetParent(gameObject.transform.GetChild(0).transform); // obiekt podrzedny: InventoryBackground
				slots[i] = newSlot;
				itemImages[i] = newSlot.transform.GetChild(1).GetComponent<Image>(); // obiekt podrzedny: ItemImage
			}
		}
	}

	public int PlayerGold()
	{
		foreach(Item item in items)
		{
			if(item!=null)
			{
				if(item.itemType == ItemType.COIN)
				{
					return item.quantity;
				}
			}
		}
		return 0;
	}

	public void RemoveGold(int amount)
	{
		foreach(Item item in items)
		{
			if(item!=null)
			{
				if(item.itemType == ItemType.COIN)
				{
					Debug.Log(amount);
					Debug.Log(item.quantity);

					item.quantity -= amount;
				}
			}
		}
	}

	public bool AddItem(Item itemToAdd)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] != null && items[i].itemType == itemToAdd.itemType && itemToAdd.stackable == true)
			{
				// Dodanie rzeczy do istniejacego pojemnikaitems
				items[i].quantity += 1;
				Slot slotScript = slots[i].GetComponent<Slot>();
				Text quantityText = slotScript.qtyText;
				quantityText.enabled = true;
				quantityText.text = items[i].quantity.ToString();

				return true;
			}

			if (items[i] == null)
			{
				// Dodanie rzeczy do pustego pojemnika
				// Skopowiowanie rzeczy i dodanie do inwentarza
				items[i] = Instantiate(itemToAdd);
				items[i].quantity = 1;
				itemImages[i].sprite = itemToAdd.sprite;
				itemImages[i].enabled = true;

				return true;
			}
		}

		return false;
	}
}
