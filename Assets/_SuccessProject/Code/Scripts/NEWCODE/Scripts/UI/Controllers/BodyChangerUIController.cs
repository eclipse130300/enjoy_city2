using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodyChangerUIController : MonoBehaviour
{
    [SerializeField] CameraHorizontalMover cameraMover;
    [SerializeField] RectTransform canvasBodyRect;

    [SerializeField] GameObject wallPartPref;
    [SerializeField] int padding;
    public List<GameObject> spawnedWalls = new List<GameObject>();
    public List<GameObject> spawnedPreviewBodies = new List<GameObject>();
    public List<BodyConfig> allBodyConfigs = new List<BodyConfig>();

    public int previewingIndex;
    public int bodyConfigsAmount;

    [SerializeField] MapConfig charEditorConfig;

    //test 

    public List<BodyConfig> configList;

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

/*        Debug.Log(bodyConfigsAmount);*/

        for (int i = padding; i < spawnedWalls.Count - padding; i++)
        {
            var placeholderTransform = spawnedWalls[i].transform.Find("CharPlaceholder");
            allBodyConfigs.Add(ScriptableList<BodyConfig>.instance.list[i - 1]);


            /*var bodyPref = ScriptableList<BodyConfig>.instance.list[i - 1].preview_body_prefab;*/
            var bodyPref = allBodyConfigs[i-padding].preview_body_prefab;


            GameObject newBody = Instantiate(bodyPref, placeholderTransform.position, placeholderTransform.rotation, placeholderTransform);
            spawnedPreviewBodies.Add(newBody);
        }

        previewingIndex = 0;
        cameraMover.SnapTo(spawnedPreviewBodies[previewingIndex].transform.position, canvasBodyRect);
    }

    public void ChangeCharBody(bool isForward)
    {
/*        float nextBodyPosX = isForward == true ?
            spawnedBodies[previewingIndex++ % bodyConfigsAmount].transform.position.x :
            spawnedBodies[previewingIndex-- % bodyConfigsAmount].transform.position.x;*/

        previewingIndex = isForward == true ?
            previewingIndex+1:
            previewingIndex-1;


        previewingIndex = Mathf.Clamp(previewingIndex, 0, spawnedPreviewBodies.Count - 1);


        cameraMover.MoveTo(spawnedPreviewBodies[previewingIndex].transform.position, canvasBodyRect);
    }

    public void OnDoneButtonTap()
    {
        //find config with current body
        /*List<BodyConfig>*/
        BodyConfig bodyConf = allBodyConfigs[previewingIndex]; /*ScriptableList<BodyConfig>.instance.list.Where(x => x.preview_body_prefab == spawnedPreviewBodies[previewingIndex]).FirstOrDefault();*/
/*        Debug.Log(bodyConf.gender.ToString());*/
        //add this config to savemanager
        SaveManager.Instance.SaveBody(bodyConf);

        Loader.Instance.LoadGameScene(charEditorConfig);
    }
}

