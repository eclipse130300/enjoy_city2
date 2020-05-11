using UnityEngine;

namespace CMS.Configs.Audio.Settings
{
    using CMS.Configs.Audio.Models;

    [CreateAssetMenu(fileName = "SoundSettings", menuName = "Settings/Audio/SoundSettings", order = 1)]
    public class SoundSettings : ScriptableObject
    {
        [SerializeField] private SoundSettingsModel settings;

        public SoundSettingsModel GetSettings()
        {
            return settings.Clone();
        }

#if UNITY_EDITOR

        [SerializeField, Space(10), TextArea(2, 5)] private string description = "";

        private void OnValidate()
        {
            if (settings.audioClip == null)
            {
                Debug.LogWarning(name + " | settings.audioClip field is missing!");
            }
        }

#endif

    }
}