using System;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace CMS.Configs.Audio.Models
{
    [Serializable]
    public class SoundSettingsModel
    {
        public AudioClip audioClip = null;
        public AudioMixerGroup mixerGroup = null;
        public SoundPriorityType priority = SoundPriorityType.NORMAL;
        [Range(0f, 1f), Space(5)] public float volume = 1f;
        [Range(-3f, 3f), Space(5)] public float pitch = 1f;
        public bool isLoop = false;
        public bool isSingleton = false;

        public SoundSettingsModel Clone()
        {
            return (SoundSettingsModel) MemberwiseClone();
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine("SoundSettingsModel")
                .Append("audioClip : ").AppendLine(audioClip == null ? "NULL" : audioClip.name)
                .Append("mixerGroup : ").AppendLine(mixerGroup == null ? "NULL" : mixerGroup.name)
                .AppendLine("priority : " + priority)
                .AppendLine("volume : " + volume)
                .AppendLine("pitch : " + pitch)
                .AppendLine("isLoop : " + isLoop)
                .AppendLine("isSingleton : " + isSingleton)
                .ToString();
        }
    }
}