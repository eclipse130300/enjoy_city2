using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class MaterialPool : Pool<Material>
{

    public MaterialPool(int growthSize = 1, Func<Material> factory = null, int capacity = int.MaxValue) : base(growthSize, factory, capacity)
    {
    }
}
