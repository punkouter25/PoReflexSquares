using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Square : MonoBehaviour, IPointerDownHandler
{
    private Image squareImage;
    private Text tapTimeText;
    public bool IsActive { get; private set; }
    private GameManager gameManager;

    void Awake()
    {
        squareImage = GetComponent<Image>();
        gameManager = FindObjectOfType<GameManager>();
        tapTimeText = GetComponentInChildren<Text>();
      // SetColor(Color.blue); // Initialize color to blue
        Debug.Log($"Square Awake. IsActive: {IsActive}");
    }

    //public void SetColor(Color newColor)
    //{
    //    if (squareImage != null)
    //    {
    //        squareImage.color = newColor;
    //    }
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Square OnPointerDown. IsActive: {IsActive}");
        if (IsActive && gameManager != null)
        {
            gameManager.SquareTapped(gameObject.transform.GetSiblingIndex());
        }
    }

    public void ActivateSquare()
    {
        IsActive = true;
        squareImage.color = Color.white;
        Debug.Log("Square Activated");
    }

    public void ResetSquare(bool wasTappedSuccessfully = false, float milliseconds = 0)
    {
        IsActive = false;
        squareImage.color = wasTappedSuccessfully ? Color.green : Color.grey;
        tapTimeText.text = wasTappedSuccessfully ? $"{milliseconds:F0} ms" : "";
        Debug.Log($"Square Reset. Was Tapped Successfully: {wasTappedSuccessfully}, Milliseconds: {milliseconds}");
    }

    //public void ResetSquare()
    //{
    //    IsActive = false;
    //    SetColor(Color.blue); // Reset color to blue
    //    if (tapTimeText != null) tapTimeText.text = ""; // Clear the tap time text
    //    Debug.Log("Square Reset. IsActive set to false.");
    //}
}
