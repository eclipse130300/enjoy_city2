using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Tab : MonoBehaviour , IPointerClickHandler
{
    public TabGroup tabGroup;
    public TextMeshProUGUI tmPro;

    public Image icon;
    [HideInInspector] public Image background;

    void Awake()
    {
        tabGroup.Subscribe(this);
        background = GetComponent<Image>();
        tmPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
}
