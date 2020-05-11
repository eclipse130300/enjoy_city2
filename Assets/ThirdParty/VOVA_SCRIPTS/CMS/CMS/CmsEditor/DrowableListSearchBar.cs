using CMS.Config;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace CMS.Editor
{
    public class DrowableListSearchBarView
    {

        List<int[]> searchResult = new List<int[]>();
        List<int> endSearchResultList = new List<int>();
        int searchIndex = 0;
        string searchRequest = "";
        string oldSearchRequest = "";

        public List<int> DrawSearchBar<T>(List<T> list, int width = 200) where T : IScriptableListItem
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("?");
            if (searchResult.Count > 0)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                if (searchRequest.Length > 0)
                {
                    GUI.backgroundColor = Color.yellow;
                }
            }
            searchRequest = GUILayout.TextField(searchRequest, GUILayout.Width(width));
            if (oldSearchRequest != searchRequest)
            {
                searchResult.Clear();
                endSearchResultList.Clear();

                oldSearchRequest = searchRequest;

                for (int i = 0; i < list.Count; i++)
                {
                    int contains = list[i].IsContains(searchRequest);
                    if (contains > -1)
                    {
                        searchResult.Add(new int[] { i, contains });
                    }

                }
                // searchResult.Sort(delegate (int[] a, int[] b)
                // {
                //return a[1] > b[1] ? 1 : a[1] == b[1]? 0 : - 1;
                // });
                if (searchResult.Count > 0)
                {
                    searchIndex = 0;

                }

                searchResult.ForEach(delegate (int[] element)
                {
                    endSearchResultList.Add(element[0]);
                });

            }
            if (GUILayout.Button("Clear"))
            {
                searchRequest = "";
                oldSearchRequest = "";
                searchIndex = 0;
                searchResult.Clear();
                endSearchResultList.Clear();
            }

            GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();
            return endSearchResultList;
        }
    }
}
#endif
