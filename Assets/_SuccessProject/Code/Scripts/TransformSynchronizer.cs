using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSynchronizer : MonoBehaviour
{
    public enum UpdateMod { 
        fixedUpdate,
        update,
        lateUpdate

    }
    public bool isMaster = false;
    public bool addInRuntime = true;
    public UpdateMod updateMod = UpdateMod.lateUpdate;
    public string key = "";
    private static Dictionary<string, TransformSynchronizer> objects = new Dictionary<string, TransformSynchronizer>();
    // Start is called before the first frame update
    void Start()
    {
        var parent = transform.parent;
        int id = GetComponentInParent<Animator>().GetInstanceID();

        while (parent.parent != null)
        {
            parent = parent.parent;

        }
        if (key == "")
        {
            key = id + gameObject.name;
        }
        if (addInRuntime)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                TransformSynchronizer suynchronizer = transform.GetChild(i).gameObject.AddComponent<TransformSynchronizer>();
                suynchronizer.isMaster = isMaster;
                suynchronizer.addInRuntime = addInRuntime;
                suynchronizer.updateMod = updateMod;
            }
        }
       
       
        if (isMaster)
        {
            objects[key] = this;
        }
        
    }
    private void OnDestroy()
    {
        if (isMaster)
        {
            objects.Remove(key);
        }
    }
    void FixedUpdate()
    {
        if (updateMod == UpdateMod.fixedUpdate && !isMaster)
        {
            UpdateValues();
        }
    }
    void Update()
    {
        if (updateMod == UpdateMod.update && !isMaster)
        {
            UpdateValues();
        }
    }
    void LateUpdate()
    {
        if (updateMod == UpdateMod.lateUpdate && !isMaster)
        {
            UpdateValues();
        }
    }
    void UpdateValues() {
        if (objects.ContainsKey(key) && objects[key] != null)
        {
            transform.position = objects[key].transform.position;
            transform.rotation = objects[key].transform.rotation;
            transform.localScale = objects[key].transform.localScale;
        }

    }
}
