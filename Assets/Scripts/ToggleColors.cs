using UnityEngine;
using UnityEngine.UI;

public class ToggleColors : MonoBehaviour
{
    private Color onColor = new Color(0x7A / 255f, 0xCF / 255f, 0xE1 / 255f, 1f);
    private Color offColor = Color.white;
    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void Start()
    {
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(bool isOn)
    {
        toggle.image.color = isOn ? onColor : offColor;
    }
}
