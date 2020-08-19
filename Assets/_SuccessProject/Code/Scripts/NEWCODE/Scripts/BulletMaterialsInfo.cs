using Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletMaterialsInfo
{
    public Material material;
    public Renderer renderer;
    public float existenceTime;
    public Vector3 hitPos;
    public Hittable hittable;
    public int poolId;

    public BulletMaterialsInfo(Material material, Renderer renderer, float existanceTime, int poolId, Vector3 hitDataPos, Hittable hittable)
        {
        this.material = material;
        this.renderer = renderer;
        this.existenceTime = existanceTime;
        this.poolId = poolId;
        this.hitPos = hitDataPos;
        this.hittable = hittable;
        }

    public bool DecrementTime(float delta)
    {
        existenceTime = Mathf.Max(existenceTime - delta, 0f);
        return existenceTime == 0f;
    }
}
