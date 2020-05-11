using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FileThread : CustomThread
{
    public override void InsertData<T>(T value)
    {
        throw new System.NotImplementedException();
    }
}
