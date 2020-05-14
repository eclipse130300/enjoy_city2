using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CMS.Config
{
    public interface IGUIDrawable
    {
#if UNITY_EDITOR
        void Draw();
#endif
    }
}
