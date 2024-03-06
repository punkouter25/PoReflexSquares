using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Square : MonoBehaviour, IPointerDownHandler
{
    private Image squareImage;
    private Text tapTimeText;
    public bool IsActive { get; set; }
    private GameManager gameManager;

    private void Awake()
    {
        squareImage = GetComponent<Image>();
        gameManager = FindObjectOfType<GameManager>(); // This could be expensive. Consider passing a reference to GameManager to each Square instead.
        tapTimeText = GetComponentInChildren<Text>();
        Debug.Log($"Square Awake. IsActive: {IsActive}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Square OnPointerDown. IsActive: {IsActive}");
        // Here you would check for IsActive before calling GameManager
        if (gameManager != null)
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
        squareImage.color = wasTappedSuccessfully ? Color.green : Color.black;
        tapTimeText.text = wasTappedSuccessfully ? $"{milliseconds:F0} ms" : "";
        Debug.Log($"Square Reset. Was Tapped Successfully: {wasTappedSuccessfully}, Milliseconds: {milliseconds}");
    }
}
