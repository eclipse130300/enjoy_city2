using CMS.Config;
using System.Collections.Generic;
using UnityEngine;
namespace CMS.Editor
{
    public class DrawableListViewWithSearchBar : DrawableListView
    {
#if UNITY_EDITOR
        DrowableListSearchBarView searchBar = new DrowableListSearchBarView();


        public IScriptableListItem DrawSellectableList<T>(List<T> list, T selectedItem, int width) where T : IScriptableListItem
        {
            GUILayout.BeginVertical();
            List<int> searchResult = searchBar.DrawSearchBar<T>(list, width - 100);
            IScriptableListItem sellected = base.DrawSellectableList<T>(list, selectedItem, searchResult, width);
            GUILayout.EndVertical();
            return sellected;
        }
#endif
    }

}
