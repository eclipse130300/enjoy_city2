using UnityEngine;

namespace SocialGTA {

    public class AutorizationController {

        #region Variables
        public ProfileModel profile;

        #endregion

        public AutorizationController() {
           


        }
        public void Login() {
            profile = SaveProfileSettings.Profile;
          
        }
        public void LogOut()
        {
            SaveProfileSettings.Profile = new ProfileModel();
        }
        public bool isAutorized {
            get {
                return profile!= null && profile.Email != "" && profile.Passworld != "" && profile.UserName != "";
            }
        
        }

    }
}
