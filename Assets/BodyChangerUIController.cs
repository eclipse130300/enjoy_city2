using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyChangerUIController : MonoBehaviour
{
    [SerializeField] CameraHorizontalMover cameraMover;

    [SerializeField] GameObject wallPartPref;
    [SerializeField] int padding;
    public List<GameObject> spawnedWalls = new List<GameObject>();
    public List<GameObject> spawnedBodies = new List<GameObject>();

    public int previewingIndex;
    public int bodyConfigsAmount;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        bodyConfigsAmount = ScriptableList<BodyConfig>.instance.list.Count;
        var floor = wallPartPref.transform.Find("Floor");
        var offsetX = floor.transform.localScale.x;
        Vector3 spawnPosition = Vector3.zero;

        //spawn wall parts (+padding*2 to make it look like endless wall)
        for (int i = 0; i < bodyConfigsAmount + padding * 2; i++)
        {
            GameObject newWall = Instantiate(wallPartPref, spawnPosition, Quaternion.identity);
            spawnPosition.x += offsetX;
            spawnedWalls.Add(newWall);
        }

        Debug.Log(bodyConfigsAmount);

        for (int i = padding; i < spawnedWalls.Count - padding; i++)
        {
            var placeholderTransform = spawnedWalls[i].transform.Find("CharPlaceholder");
            var bodyPref = ScriptableList<BodyConfig>.instance.list[i - 1].body_prefab;
            GameObject newBody = Instantiate(bodyPref, placeholderTransform.position, placeholderTransform.rotation, placeholderTransform);
            spawnedBodies.Add(newBody);
        }

        previewingIndex = 0;
        cameraMover.SnapTo(spawnedBodies[previewingIndex].transform.position.x);
    }

    public void ChangeCharBody(bool isForward)
    {
/*        float nextBodyPosX = isForward == true ?
            spawnedBodies[previewingIndex++ % bodyConfigsAmount].transform.position.x :
            spawnedBodies[previewingIndex-- % bodyConfigsAmount].transform.position.x;*/

        previewingIndex = isForward == true ?
            previewingIndex+1:
            previewingIndex-1;


        previewingIndex = Mathf.Clamp(previewingIndex, 0, spawnedBodies.Count - 1);
        Debug.Log(previewingIndex);

        cameraMover.MoveTo(spawnedBodies[previewingIndex].transform.position.x);
    }

    private bool inBounds(int index, int count)
    {
        return (index >= 0) && (index < count);
    }
}

