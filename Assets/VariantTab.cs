using CMS.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VariantTab : MonoBehaviour, IPointerClickHandler
{
    public Image activeIMG;
    public Image tabBackground;

    public VariantGroup group;
    public ItemVariant variant;

    private void Awake()
    {
        activeIMG = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        group.OnVariantSelected(this);
        Messenger.Broadcast(GameEvents.ITEM_VARIANT_CHANGED, variant); //texture as well
    }
}
