using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowPassword : MonoBehaviour
{
    private TMP_InputField inputField;
    private Toggle toggle;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        toggle = transform.Find("ShowPass").GetComponent<Toggle>();

        // Set the initial content type based on the toggle state
        if (toggle.isOn)
        {
            inputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
        }
    }

    public void OnToggleValueChanged()
    {
        // Change the content type based on the toggle state
        if (toggle.isOn)
        {
            inputField.contentType = TMP_InputField.ContentType.Standard;
            inputField.ForceLabelUpdate();
        }
        else
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
            inputField.ForceLabelUpdate();
        }
    }
}
