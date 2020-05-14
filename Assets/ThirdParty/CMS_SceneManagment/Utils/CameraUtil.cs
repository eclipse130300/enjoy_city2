using UnityEngine;

namespace Utils
{
    public static class CameraUtil
    {
        // TODO(Rostislav Pogosian 12.12.2018) : Something is wrong here. If camera is looking towards z axis and down y axis objects near top of the viewport get culled when they shouldn't.
        // 


        // NOTE(Rostislav Pogosian 12.12.2018:
        // THIS IS NOT UNITY PLANES!
        // Vector4 plane is defined by xy being the center of the position and zw being the size!

        /// <summary>
        /// Buildis frustum planes based from give projection view matrix.
        /// Planes array must be allocated prior to call and it's length should not be less than 6.
        /// </summary>
        /// <param name="pv">Projection View matrix</param>
        /// <param name="planes">Frustum planes</param>
        public static void MakeFrustrumPlanes(Matrix4x4 pv, Vector4[] planes)
        {
            float t;

            // TODO(Rostislav Pogosian 12.12.2018): Get matrix values directly [mm0...mmX], do not use index operator!

            //Right plane

            planes[0].x = pv[3] - pv[0];
            planes[0].y = pv[7] - pv[4];
            planes[0].z = pv[11] - pv[8];
            planes[0].w = pv[15] - pv[12];

            t = Mathf.Sqrt(planes[0].x * planes[0].x + planes[0].y * planes[0].y + planes[0].z * planes[0].z);

            planes[0].x /= t;
            planes[0].y /= t;
            planes[0].z /= t;
            planes[0].w /= t;

            //Left plane

            planes[1].x = pv[3] + pv[0];
            planes[1].y = pv[7] + pv[4];
            planes[1].z = pv[11] + pv[8];
            planes[1].w = pv[15] + pv[12];

            t = Mathf.Sqrt(planes[1].x * planes[1].x + planes[1].y * planes[1].y + planes[1].z * planes[1].z);
            planes[1].x /= t;
            planes[1].y /= t;
            planes[1].z /= t;
            planes[1].w /= t;

            //Bottom plane

            planes[2].x = pv[3] + pv[1];
            planes[2].y = pv[7] + pv[5];
            planes[2].z = pv[11] + pv[9];
            planes[2].w = pv[15] + pv[13];

            t = Mathf.Sqrt(planes[2].x * planes[2].x + planes[2].y * planes[2].y + planes[2].z * planes[2].z);
            planes[2].x /= t;
            planes[2].y /= t;
            planes[2].z /= t;
            planes[2].w /= t;

            //Top plane

            planes[3].x = pv[3] - pv[1];
            planes[3].y = pv[7] - pv[5];
            planes[3].z = pv[11] - pv[9];
            planes[3].w = pv[15] - pv[13];

            t = Mathf.Sqrt(planes[3].x * planes[3].x + planes[3].y * planes[3].y + planes[3].z * planes[3].z);

            planes[3].x /= t;
            planes[3].y /= t;
            planes[3].y /= t;
            planes[3].w /= t;

            //Far plane

            planes[4].x = pv[3] - pv[2];
            planes[4].y = pv[7] - pv[6];
            planes[4].z = pv[11] - pv[10];
            planes[4].w = pv[15] - pv[14];

            t = Mathf.Sqrt(planes[4].x * planes[4].x + planes[4].y * planes[4].y + planes[4].z * planes[4].z);

            planes[4].x /= t;
            planes[4].y /= t;
            planes[4].z /= t;
            planes[4].w /= t;

            //Near plane

            planes[5].x = pv[3] + pv[2];
            planes[5].y = pv[7] + pv[6];
            planes[5].z = pv[11] + pv[10];
            planes[5].w = pv[15] + pv[14];

            t = Mathf.Sqrt(planes[5].x * planes[5].x + planes[5].y * planes[5].y + planes[5].z * planes[5].z);

            planes[5].x /= t;
            planes[5].y /= t;
            planes[5].z /= t;
            planes[5].w /= t;
        }
        
        /// <summary>
        /// NOTE(Rostislav Pogosian 12.12.2018): Kinda funny function. Basically it just ignores y axis of the planes. 
        /// Might not work properly thoguh... But seems ok.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <param name="frustumPlanes"></param>
        /// <returns></returns>
        public static bool SphereInFrustum(float x, float z, float radius, Vector4[] frustumPlanes)
        {
            for (int i = 0; i < 6; ++i)
            {
                Vector4 plane = frustumPlanes[i];

                if (plane.x * x + plane.z * z + plane.w <= -radius)
                {
                    return (false);
                }
            }

            return true;
        }

        public static bool SphereInFrustum(Vector3 at, float radius, Vector4[] frustumPlanes)
        {
            for (int i = 0; i < 6; ++i)
            {
                Vector4 plane = frustumPlanes[i];

                if (plane.x * at.x + plane.y * at.y + plane.z * at.z + plane.w <= -radius)
                {
                    return (false);
                }
            }

            return true;
        }
    }
}