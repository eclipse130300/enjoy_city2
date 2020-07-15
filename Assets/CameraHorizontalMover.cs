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
    public Vector3 getOffset(RectTransform bodyRect) {
        Vector3 vect = camera.ScreenToWorldPoint(new Vector3(bodyRect.position.x, bodyRect.position.y, camera.transform.position.z));
        return new Vector3(transform.position.x - vect.x, transform.position.y - vect.y, transform.position.z - vect.z);
    }
    // private void getOffset(RectTransform bodyRect)
    public void SnapTo(Vector3 targetPos, RectTransform bodyRect)
    {
        transform.position = new Vector3(targetPos.x -  getOffset(bodyRect).x, transform.position.y, offsetZ); 
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
