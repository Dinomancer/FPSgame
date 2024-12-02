using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    [SerializeField] private TMP_Text colorDisplayText;
    [SerializeField] private RawImage colorIndicatorImage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateColorDisplay(string color)
    {
        if (colorDisplayText != null)
        {
            colorDisplayText.text = "Current Color: " + color;
        }

        if (colorIndicatorImage != null)
        {
            Color newColor = Color.white;
            switch (color.ToLower())
            {
                case "red":
                    newColor = Color.red;
                    break;
                case "green":
                    newColor = Color.green;
                    break;
                case "blue":
                    newColor = Color.blue;
                    break;
            }
            colorIndicatorImage.color = newColor;
        }
    }
}
