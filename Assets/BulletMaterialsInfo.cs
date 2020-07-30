using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletMaterialsInfo
{
    public Material material;
    public Renderer renderer;
    public float existenceTime;
    public int poolId;

    public BulletMaterialsInfo(Material material, Renderer renderer, float existanceTime, int poolId)
        {
        this.material = material;
        this.renderer = renderer;
        this.existenceTime = existanceTime;
        this.poolId = poolId;
        }

    public bool DecrementTime(float delta)
    {
        existenceTime = Mathf.Max(existenceTime - delta, 0f);
        return existenceTime == 0f;
    }
}
