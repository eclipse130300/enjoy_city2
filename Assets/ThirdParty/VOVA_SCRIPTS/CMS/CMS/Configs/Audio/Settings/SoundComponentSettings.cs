using UnityEngine;

namespace CMS.Configs.Audio.Settings
{
/*    using CMS.Configs.Audio.Models;

    [CreateAssetMenu(fileName = "SoundComponentSettings", menuName = "Settings/Audio/SoundComponentSettings", order = 3)]
    public class SoundComponentSettings : ScriptableObject
    {
        [SerializeField] protected SoundSettings[] soundSettings;
        [SerializeField] protected SoundComponentSettingsModel componentSettings;

        public SoundComponentSettingsModel GetSettings()
        {
            SoundSettingsModel[] sounds = GetSoundSetiings();
            if (sounds.IsNullOrEmpty())
            {
                return null;
            }
            SoundComponentSettingsModel result = componentSettings.Clone();
            result.sounds = sounds;
            return result;
        }

        private SoundSettingsModel[] GetSoundSetiings()
        {
            if (soundSettings.IsNullOrEmpty())
            {
                return null;
            }
            int length = soundSettings.Length;
            var result = new SoundSettingsModel[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = soundSettings[i].GetSettings();
            }
            return result;
        }

#if UNITY_EDITOR

        [ContextMenu("PrintSettings")]
        private void PrintSettings()
        {
            Debug.Log(GetSettings());
        }

        private void OnValidate()
        {
            if (soundSettings.IsNullOrEmpty())
            {
                Debug.LogWarning(name + " | soundSettings field IsNullOrEmpty!");
            }
        }

#endif

    }*/
}