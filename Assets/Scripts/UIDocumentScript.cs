using UnityEngine;
using UnityEngine.UIElements;

public class RoundedCard : MonoBehaviour
{
    public UIDocument doc;

    void OnEnable()
    {
        var root = doc.rootVisualElement;

        // Make root fill the screen
        root.style.flexDirection = FlexDirection.Row;
        root.style.justifyContent = Justify.Center;
        root.style.alignItems = Align.Center;
        root.style.width = Length.Percent(100);
        root.style.height = Length.Percent(100);
        root.style.backgroundColor = new Color(0xF0 / 255f, 0xF9 / 255f, 0xFF / 255f); // #f0f9ff

        // Rounded rectangle
        var rect = new VisualElement();
        rect.style.width = 300;
        rect.style.height = 160;
        rect.style.backgroundColor = new Color(0.90f, 0.96f, 1f); // light blue
        rect.style.borderTopLeftRadius = 16;
        rect.style.borderTopRightRadius = 16;
        rect.style.borderBottomLeftRadius = 16;
        rect.style.borderBottomRightRadius = 16;
        rect.style.borderLeftWidth = 3;
        rect.style.borderRightWidth = 3;
        rect.style.borderTopWidth = 3;
        rect.style.borderBottomWidth = 3;
        rect.style.borderLeftColor = new Color(0.16f, 0.48f, 1f);
        rect.style.borderRightColor = rect.style.borderLeftColor;
        rect.style.borderTopColor = rect.style.borderLeftColor;
        rect.style.borderBottomColor = rect.style.borderLeftColor;

        // Optional: make rectangle scale with screen size (mobile-friendly)
        rect.style.minWidth = 200;
        rect.style.maxWidth = 400;
        rect.style.minHeight = 120;
        rect.style.maxHeight = 240;

        root.Add(rect);
    }
}
