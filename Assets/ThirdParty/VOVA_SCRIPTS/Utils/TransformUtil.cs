using UnityEngine;

public static class TransformUtil
{
    public static void ResetTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = new Vector3(1, 1, 1);
    }
}
