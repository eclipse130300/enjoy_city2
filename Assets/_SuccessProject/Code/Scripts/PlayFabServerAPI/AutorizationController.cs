using UnityEngine;

namespace SocialGTA {

    public class AutorizationController : MonoBehaviour {

        #region Variables

        [Header("Componenst")]
        [SerializeField] int regSceneID;
        [SerializeField] int mMenuSceneID;
        [SerializeField] int cCharacterSceneID;
        [SerializeField] SceneLoaderManager loaderManager;

        #endregion

        #region Standart Functions

        private void Awake () {
            if(SaveProfileSettings.Profile == null) {
                // Loading registration menu
                loaderManager.LoadAsyncScene(regSceneID);
            } else {
                if (SaveProfileSettings.Profile.isChooseCharacter) {
                    // Loading main menu
                    loaderManager.LoadAsyncScene(mMenuSceneID);
                } else {
                    // Choose Character
                    loaderManager.LoadAsyncScene(cCharacterSceneID);
                }
            }
        }

        #endregion

    }
}
