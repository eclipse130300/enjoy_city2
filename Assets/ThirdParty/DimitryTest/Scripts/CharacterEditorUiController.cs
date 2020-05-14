using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEditorUiController : MonoBehaviour
{
    public MapConfig playerRoom;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDoneButtonTap()
    {
        Loader.Instance.LoadGameScene(playerRoom);
    }
}
