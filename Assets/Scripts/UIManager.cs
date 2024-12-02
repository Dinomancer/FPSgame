using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    [SerializeField] private TMP_Text colorDisplayText;
    [SerializeField] private RawImage colorIndicatorImage;

    private readonly Color redColor = new Color(
        0xE7 / 255f,  // 231/255
        0x52 / 255f,  // 82/255
        0x62 / 255f   // 98/255
    );

    private readonly Color greenColor = new Color(
        0x4B / 255f,  // 75/255
        0xD9 / 255f,  // 217/255
        0x54 / 255f   // 84/255
    );

    private readonly Color blueColor = new Color(
        0x4D / 255f,  // 77/255
        0x5D / 255f,  // 93/255
        0xDB / 255f   // 219/255
    );

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
                    colorIndicatorImage.color = redColor;
                    break;
                case "green":
                    colorIndicatorImage.color = greenColor;
                    break;
                case "blue":
                    colorIndicatorImage.color = blueColor;
                    break;
            }
        }
    }
}
