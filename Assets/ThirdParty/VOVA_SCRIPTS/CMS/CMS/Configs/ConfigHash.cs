using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
namespace CMS.Config
{
    [System.Serializable]
    public class ConfigHash : ScriptableObject
    {
        [SerializeField]
        public List<IntStringString> hashes = new List<IntStringString>();

    }
}

