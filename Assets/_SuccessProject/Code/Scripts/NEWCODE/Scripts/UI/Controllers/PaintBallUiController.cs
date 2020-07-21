using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallUiController : MonoBehaviour
{
    [SerializeField] FixedButton shootButton;
    [SerializeField] FixedButton reloadButton;
    [SerializeField] FixedButton SuperShotButton;
    [SerializeField] FixedButton powerUpButton;

    Camera playerCamera;

    private void Awake()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if(shootButton.Pressed)
        {
            Quaternion cameraOrientation = GetCameraOrientation();
            Messenger.Broadcast(GameEvents.SHOOT_PRESSED, cameraOrientation);
        }
    }

    private Quaternion GetCameraOrientation()
    {
        /*        Vector2 randomCirclePoint = Random.insideUnitCircle;
                Vector2 randomScreenPoint = crossHair.rect.position + randomCirclePoint; // * playerInput
        */
        return playerCamera.transform.rotation;
    }
}
