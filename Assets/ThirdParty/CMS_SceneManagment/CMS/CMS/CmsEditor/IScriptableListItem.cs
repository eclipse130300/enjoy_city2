using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CMS.Config
{
    public interface IScriptableListItem
    {

        string ConfigId
        {
            get;
            set;
        }

#if UNITY_EDITOR
        GUIContent ContentForListMinimal();
        GUIContent ContentForListMaximal();
        int IsContains(string searchRequest);
#endif
    }
}
