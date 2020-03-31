using UnityEngine;

namespace SocialGTA {

    public class MainMenuController : MonoBehaviour {

        #region Variables

        [Header("Settings")]
        [SerializeField] int _cCharacterSceneID;
        [SerializeField] string _serverMapName;

        private SceneLoaderManager _loaderManager;

        #endregion

        #region Standart Functions

        private void Start () {
            _loaderManager = SceneLoaderManager.Instance;
        }

        #endregion

        #region Custom Functions

        public void OnClickOpenChooseCharacterScene () {
            _loaderManager.LoadAsyncScene(_cCharacterSceneID);
        }

        public void OnClickConnectionInServer () {
            _loaderManager.ConnectionInServer();
        }

        public void OnClickCreateANewServer () {
            _loaderManager.StartServer(_serverMapName);
        }

        #endregion

    }
}
