using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CMS.Config;

namespace SocialGTA {

    public class RegistrationController : MonoBehaviour {

        #region Variables

        [Header("UI Components")]
        [SerializeField] TMP_InputField userNameIF;
        [SerializeField] TMP_InputField emailIF;
        [SerializeField] TMP_InputField passworldIF;
        [SerializeField] Button regBttn;

        [Space(5), Header("Other Componenst")]
        [SerializeField] MapConfig createCharacter;
      //  [SerializeField] SceneLoaderManager loaderManager;

        string errorMessage;
        string playFabErrorMessage;

        #endregion

        #region Standart Functions

        private void Start () {
            CheckError("");
        }

        #endregion

        #region Custom Functions

        public void OnClickRegistration () {
            Loader.Instance.LoadGameScene(createCharacter);
            SaveProfileSettings.Profile = new ProfileModel
            {
                UserName = userNameIF.text,
                Email = emailIF.text,
                Passworld = passworldIF.text
            };
            ServerAPIManager.Registrate(userNameIF.text, emailIF.text, passworldIF.text, result => {
                SaveProfileSettings.Profile = new ProfileModel
                {
                    UserName = userNameIF.text,
                    Email = emailIF.text,
                    Passworld = passworldIF.text
                };
              
                // loaderManager.LoadAsyncScene(cCharacterSceneID);

            }, error => {
                playFabErrorMessage = error.ErrorMessage;
            });
        }

        public void CheckError (string val) {
            errorMessage = "";

            if (passworldIF.text.Length < 6)
                errorMessage += "Пароль должен иметь хотябы 6 знаков." + "/n";

            if (playFabErrorMessage?.Length == 0)
                errorMessage += playFabErrorMessage + "/n";

            regBttn.interactable = userNameIF.text.Length > 3 && emailIF.text.Length > 4 && passworldIF.text.Length > 5;
        }

        #endregion
    }
}
