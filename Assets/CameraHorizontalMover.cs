using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHorizontalMover : MonoBehaviour
{
    [SerializeField] float offsetZ;
    [SerializeField] float OffsetX; //tweak - hardcode :(
    [Range(0f,1f)]
    [SerializeField] float speed;
    [SerializeField] float movingFOV;

    private const int referenceResolutionWidth = 1300; 

    private Camera camera;
    private float startingFOV;

    private float diffeneceX;
    private RectTransform bodyRect;

    Vector3 test;
    Vector3 test2;

    public Canvas canvas;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        startingFOV = camera.fieldOfView;

        float screenRelation = (float)Screen.width / referenceResolutionWidth;
        OffsetX = OffsetX * screenRelation;
    }

    public void MoveTo(Vector3 targetPos, RectTransform cameraBodyScreenPoint)
    {
        StopAllCoroutines();
        /*var cameraOffsetPos = new Vector3(targetPos.x, transform.position.y, offsetZ);*/
        var cameraOffsetPos = new Vector3(targetPos.x - OffsetX, transform.position.y, offsetZ);


        Vector3 cameraMoveDirection = cameraOffsetPos.x > transform.position.x ? transform.right : -transform.right;


        /*StartCoroutine(LerpMove(targetPos, cameraMoveDirection, cameraBodyScreenPoint));*/
        /*StartCoroutine(LerpMove(cameraOffsetPos));*/
    }

    public void SnapTo(Vector3 targetPos, RectTransform bodyRect)
    {
        transform.position = new Vector3(targetPos.x, transform.position.y, offsetZ);

        Vector2 currentBodyPos = camera.WorldToScreenPoint(targetPos);

        float desiredBodyscreenPosX = Screen.width + bodyRect.anchoredPosition.x; //+ cause rect is anchored to the right bottom point
        Debug.Log("BODY DESIRED RECT :" + bodyRect.anchoredPosition);
        Debug.Log(desiredBodyscreenPosX);
        diffeneceX = Mathf.Abs(Mathf.Abs(desiredBodyscreenPosX) - Mathf.Abs(currentBodyPos.x));
        Debug.Log(diffeneceX);
        /*
                OffsetX = (float)diffeneceX / Screen.width;*/
        Vector3 newPos = new Vector3(currentBodyPos.x + diffeneceX, currentBodyPos.y, 0);
        newPos = camera.ScreenToWorldPoint(newPos);
        Debug.Log("NEWPOS : " + camera.previousViewProjectionMatrix * newPos);
        newPos = camera.previousViewProjectionMatrix * newPos;

        transform.position = new Vector3(newPos.x, transform.position.y, offsetZ);
        /*        targetPos.y = transform.position.y;
                float side = (targetPos - transform.position).magnitude;
                Debug.Log(side);

                Vector2 pixelPoint = RectTransformUtility.PixelAdjustPoint(bodyRect.rect.position, bodyRect.transform, canvas);

                Ray ray = camera.ScreenPointToRay(pixelPoint);
                Debug.Log(bodyRect.rect.position);
                RaycastHit hit;

                float hypotenuse = 0f;
                if (Physics.Raycast(ray, out hit));
                {
                    Vector3 hitpoint = hit.point;
                    hitpoint.y = transform.position.y;
                    test = hitpoint;
                    hypotenuse = (hitpoint - transform.position).magnitude;
                }
                Debug.Log(hypotenuse);

                OffsetX = Mathf.Sqrt(hypotenuse * hypotenuse - side * side);

                transform.position = new Vector3(targetPos.x - OffsetX, transform.position.y, offsetZ);*/
    }

/*    IEnumerator LerpMove(Vector3 targetPos)
    {
        Debug.Log(diffeneceX);
        diffeneceX = Mathf.Abs(Mathf.Abs(targetPos.x) - Mathf.Abs(transform.position.x));
        while (diffeneceX >= 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed);
            diffeneceX = Mathf.Abs(Mathf.Abs(targetPos.x) - Mathf.Abs(transform.position.x));
            yield return null;
        }
        transform.position = targetPos;
    }*/

    //moves via canvasUI
    IEnumerator LerpMove(Vector3 bodyPosition, Vector3 cameraMoveDirection, RectTransform bodyPoint)
    {

        //transform world body pos into screen pos
        Vector2 screenBodyPos = camera.WorldToScreenPoint(bodyPosition);

        //if point on the screen point is IN UI rectangle, stop moving camera
        while (!RectTransformUtility.RectangleContainsScreenPoint(bodyPoint, screenBodyPos))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + cameraMoveDirection, speed);
            screenBodyPos = camera.WorldToScreenPoint(bodyPosition);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, test);
        Gizmos.DrawSphere(test2, 1f);
    }

    private void OnDestroy()
    {
        
    }
}
