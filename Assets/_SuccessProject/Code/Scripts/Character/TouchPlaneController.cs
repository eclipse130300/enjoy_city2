using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPlaneController : MonoBehaviour, IDragHandler, IPointerUpHandler {

    [HideInInspector] public Vector2 Direction;

    public void OnDrag(PointerEventData eventData) {
        Direction = eventData.delta;
    }

    public void OnPointerUp(PointerEventData eventData) {
        Direction = Vector2.zero;
    }
}
