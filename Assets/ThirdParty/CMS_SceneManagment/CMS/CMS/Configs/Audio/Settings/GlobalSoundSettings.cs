using UnityEngine;

namespace CMS.Configs.Audio.Settings
{
 /*   using CMS.Config;
    using CMS.Configs.Audio.Models;
    using CMS.Editor;

    [CreateAssetMenu(fileName = "GlobalSoundSettings", menuName = "Settings/Audio/GlobalSoundSettings", order = 0)]
    public class GlobalSoundSettings : BaseScriptableDrowableItem
    {
        [SerializeField] private GlobalSoundSettingModel settings;

        public GlobalSoundSettingModel GetSettings()
        {
            var result = new GlobalSoundSettingModel().Load();
            if (result == null)
            {
                if (settings == null)
                {
                    return null;
                }
                result = settings.Clone();
                result.Save();
            }
            return result;
        }

#if UNITY_EDITOR

        public override void Draw()
        {
            base.Draw();
            ScriptableGUIUtils.DrawAllField(settings);
        }

        [ContextMenu("SaveCurrentSettings")]
        private void SaveCurrentSettings()
        {
            settings.Save();
        }

        [ContextMenu("PrintStoredSettings")]
        private void PrintStoredSettings()
        {
            Debug.Log(GetSettings());
        }

#endif

    }*/
}