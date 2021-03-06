﻿using Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MaterialPooler : MonoBehaviourSingleton<MaterialPooler>
{
    public Material[] materials;
    public int growthSize = 1;
    public int maxPoolSize = 10;

    public float singleHitExistence = 2f;

    public Dictionary<Material, MaterialPool> bulletHitPools = new Dictionary<Material, MaterialPool>();

    int handyIndex;

    public List<BulletMaterialsInfo> existingMaterials = new List<BulletMaterialsInfo>();

    public HittablesController hittablesController;

    private void Awake()
    {
        if(hittablesController == null)
        {
            hittablesController = FindObjectOfType<HittablesController>();
        }
    }

    private void Start()
    {
        InitializePools();
    }

    void InitializePools()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            handyIndex = i;
            var newMatPool = new MaterialPool(growthSize, MaterialFact, maxPoolSize);
            bulletHitPools.Add(materials[i], newMatPool);
        }
    }

    public Material GetRandomBulletMaterial(Renderer renderer, Hittable hittable, Vector3 collisionPoint)
    {
        handyIndex = UnityEngine.Random.Range(0, materials.Length);
        var randomMat = materials[handyIndex];
        var newMaterial = bulletHitPools[randomMat].GetObject();

        existingMaterials.Add(new BulletMaterialsInfo(newMaterial, renderer, singleHitExistence, handyIndex, collisionPoint, hittable));

        return newMaterial;

    }

    public Material MaterialFact()
    {
        return new Material(materials[handyIndex]);
    }

    private void Update()
    { 
        for (int i = 0; i < existingMaterials.Count; i++)
        {
            if(existingMaterials[i].DecrementTime(Time.deltaTime))
            {
                ReturnMaterialToPool(existingMaterials[i]);
            }
        }
    }

    void ReturnMaterialToPool(BulletMaterialsInfo info)
    {

        var objMaterials = info.renderer.sharedMaterials.ToList();
    //    objMaterials.Remove(info.material);
        for (int i=0; i< objMaterials.Count; i++)
        {
            if(objMaterials[i] == info.material)
            {
                objMaterials.Remove(info.material);
                i--;
/*               Debug.Log("Materials are equal");*/
            }
        }

        /*        info.renderer.sharedMaterials = (objMaterials).ToArray();*/

        info.hittable.Clear(info.hitPos);

       bulletHitPools[GetMaterial(info.poolId)].PutObject(info.material);
        existingMaterials.Remove(info);
    }

    Material GetMaterial(int poolIndex)
    {
        return materials[poolIndex];
    }
}
