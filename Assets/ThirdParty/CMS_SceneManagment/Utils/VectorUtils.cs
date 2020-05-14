using UnityEngine;

namespace Utils
{
    public static class VectorUtils
    {
        public static Vector2 RotateVector2(this Vector2 v, float angle)
        {
            Vector2 result;

            float degree = angle * Mathf.Deg2Rad;

            float degreeSin = Mathf.Sin(degree);
            float degreeCos = Mathf.Cos(degree);

            result = new Vector2(v.x * degreeCos - v.y * degreeSin, v.x * degreeSin - v.y * degreeCos);

            return result;
        }

        public static Vector3 RotateVector3AlongYAxis(this Vector3 v, float angle)
        {
            Vector3 result;

            float degree = angle * Mathf.Deg2Rad;

            float degreeSin = Mathf.Sin(degree);
            float degreeCos = Mathf.Cos(degree);

            result = new Vector3(v.x * degreeCos - v.z * degreeSin, v.y, v.x * degreeSin - v.z * degreeCos);

            return result;
        }

        public static Vector3 Land(this Vector3 v)
        {
            RaycastHit hit;
            if (Physics.Raycast(v + Vector3.up * 1000, Vector3.down, out hit, 10000))
            {
                return new Vector3(v.x, hit.point.y, v.z);
            }

            return v;
        }

        public static Vector3 Land(this Vector3 v, int layer)
        {
            RaycastHit hit;
            if (Physics.Raycast(v + Vector3.up * 1000, Vector3.down, out hit, 10000,  layer))
            {
                return new Vector3(v.x, hit.point.y, v.z);
            }

            return v;
        }

	    public static float Distance2d(this Vector3 a, Vector3 b)
	    {
		    float dx = a.x - b.x;
		    float dz = a.z - b.z;
		    return Mathf.Sqrt(dx * dx + dz * dz);
	    }

        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 XYZ(this Vector2 v, float z = 0.0f)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 XZY(this Vector2 v, float y = 0.0f)
        {
            return new Vector3(v.x, y, v.y);
        }

        public static bool OnLayer(this Vector3 v, int Layer)
        {
            RaycastHit hit;
            if (Physics.Raycast(v + Vector3.up * 1000, Vector3.down, out hit, 10000))
            {
                return hit.collider.gameObject.layer == Layer;
            }
            return false;
        }
    }
}