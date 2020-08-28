using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHorizontalMover : MonoBehaviour
{
    [SerializeField] float offsetZ;
    [Range(0f,10f)]
    [SerializeField] float lerpSpeed;
    private Camera cameraHor;
    public float diffeneceX;
    public Canvas canvas;

/*    [SerializeField] float movingFOV;*/
/*    private float startingFOV;*/

    private void Awake()
    {
        cameraHor = GetComponent<Camera>();
/*        startingFOV = camera.fieldOfView;*/
    }

    public void MoveTo(Vector3 targetPos, RectTransform cameraBodyScreenRect)
    {
        StopAllCoroutines();

        StartCoroutine(LerpMove(targetPos, cameraBodyScreenRect));
    }

    public Vector3 getOffset(RectTransform bodyRect) {
        Vector3 worldRectPos = cameraHor.ScreenToWorldPoint(new Vector3(bodyRect.position.x, bodyRect.position.y, cameraHor.transform.position.z));
        return new Vector3(transform.position.x - worldRectPos.x, transform.position.y - worldRectPos.y, transform.position.z - worldRectPos.z);
    }

    public void SnapTo(Vector3 targetPos, RectTransform bodyRect)
    {
        transform.position = new Vector3(targetPos.x -  getOffset(bodyRect).x, transform.position.y, offsetZ); 
    }

    IEnumerator LerpMove(Vector3 targetPos, RectTransform desiredBodyRect)
    {
        Vector3 desiredPos = new Vector3(targetPos.x - getOffset(desiredBodyRect).x, transform.position.y, offsetZ);

        diffeneceX = Mathf.Abs(Mathf.Abs(desiredPos.x) - Mathf.Abs(transform.position.x));

        while (diffeneceX >= 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPos, lerpSpeed * Time.deltaTime);
            diffeneceX = Mathf.Abs(Mathf.Abs(desiredPos.x) - Mathf.Abs(transform.position.x));
            yield return null;
        }
        transform.position = desiredPos;
    }
}
