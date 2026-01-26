using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text interactText;

    void Start()
    {
        interactText.text = "";
    }

    public void SetInteract(string prompt)
    {
        if (interactText)
            interactText.text = prompt;
    }
}
