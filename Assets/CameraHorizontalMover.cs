using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHorizontalMover : MonoBehaviour
{
    [SerializeField] float offsetZ;
    [Range(0f,1f)]
    [SerializeField] float speed;
    [SerializeField] float movingFOV;

    private Camera camera;
    private float startingFOV;

    //test
    public float diffeneceX;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        startingFOV = camera.fieldOfView;
    }

    public void MoveTo(float targetPosX)
    {
        StopAllCoroutines();
        var targetPos = new Vector3(targetPosX, transform.position.y, offsetZ);
        StartCoroutine(LerpMove(targetPos));
    }

    public void SnapTo(float targetPosX)
    {
        transform.position = new Vector3(targetPosX, transform.position.y, offsetZ);
    }

    IEnumerator LerpMove(Vector3 targetPos)
    {
        /*var*/ 
        Debug.Log(diffeneceX);
        diffeneceX = Mathf.Abs(Mathf.Abs(targetPos.x) - Mathf.Abs(transform.position.x));
/*        camera.fieldOfView = movingFOV;*/
        while (diffeneceX >= 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed);
            diffeneceX = Mathf.Abs(Mathf.Abs(targetPos.x) - Mathf.Abs(transform.position.x));
/*            Debug.Log(diffeneceX);*/
            yield return null;
        }
        Debug.Log("Finished!");
        transform.position = targetPos;
/*        camera.fieldOfView = startingFOV;*/
    }

    private void OnDestroy()
    {
        
    }
}
