using UnityEngine;

namespace SocialGTA {

    public class SaveProfileSettings {

        static string SAVEPROFILE_KEY = "Save_profile_key";

        public static ProfileModel Profile {
            get{

                if (PlayerPrefs.HasKey(SAVEPROFILE_KEY)) {
                    return JsonUtility.FromJson<ProfileModel>(PlayerPrefs.GetString(SAVEPROFILE_KEY));
                }

                return null;
            }
            set{
                PlayerPrefs.SetString(SAVEPROFILE_KEY, JsonUtility.ToJson(value));
                PlayerPrefs.Save();
            }
        }

    }
}
