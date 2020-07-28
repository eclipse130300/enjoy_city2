using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class MaterialPooler : MonoBehaviourSingleton<MaterialPooler>
{
    public Material[] materials;
    public int growthSize = 1;
    public int maxPoolSize = 10;

    public MaterialPool[] materialPools;
    int handyIndex;

    private void Start()
    {
        InitializePools();
    }

    void InitializePools()
    {
        materialPools = new MaterialPool[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            handyIndex = i;
            materialPools[i] = new MaterialPool(growthSize, MaterialFact, maxPoolSize);
        }
    }

    public Material GetRandomBulletMaterial()
    {
        handyIndex = UnityEngine.Random.Range(0, materialPools.Length);

        return materialPools[handyIndex].GetObject();
    }

    public Material MaterialFact()
    {
        return new Material(materials[handyIndex]);
    }
}
