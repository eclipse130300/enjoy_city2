using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SocialGTA {

    public class ChooseCharacterController : MonoBehaviour {

        #region Variables

        [Header("Settings")]
        [SerializeField] int _mMenuSceneID;
        [SerializeField] CharactersData _charactersData;
        [SerializeField] SceneLoaderManager _loaderManager;
        [SerializeField] GameObject[] _characterModels;

        [Space(10), Header("UI Components")]
        [SerializeField] TMP_Text _nameText;
        [SerializeField] TMP_Text _discriptionText;
        [SerializeField] Image _avatarImage;

        private int _characterID;

        #endregion

        #region Standart Functions

        private void Start () {
            OnClickChangeCharacter(0);
        }

        #endregion

        #region Custom Functions

        /// <summary>
        /// Button index of -1 to 1
        /// </summary>
        /// <param name="bttnID"></param>
        public void OnClickChangeCharacter (int bttnID) {
            _characterID = SaveProfileSettings.Profile?.characterIndex ?? 0;
            _characterID += bttnID;
            _characterID = Mathf.Clamp(_characterID, 0, _charactersData.Characters.Length - 1);
            
            var profile = SaveProfileSettings.Profile ?? new ProfileModel {
                characterIndex = 0
            };

            profile.characterIndex = _characterID;
            SaveProfileSettings.Profile = profile;
            var character = _charactersData.Characters[_characterID];

            // Scene Settings
            for (int i = 0; i < _characterModels.Length; ++i)
                _characterModels[i].SetActive(false);
            _characterModels[_characterID].SetActive(true);

            // Update UI
            _avatarImage.sprite = character.Avatar;
            _nameText.text = character.Name;
            _nameText.color = character.NameColor;

            string discription = @"<color=#FFC900>Name:</color> <color=#FFFFFF>{0}</color>
<color=#FFC900>Age:</color> <color=#FFFFFF>{1}</color>
<color=#FFC900>Hobby:</color> <color=#FFFFFF>{2}</color>";

            _discriptionText.text = string.Format(discription, character.FullName, character.Age, character.Hobby);
        }

        public void OnClickChooseCharacter () {
            var profile = SaveProfileSettings.Profile;
            profile.characterIndex = _characterID;
            profile.isChooseCharacter = true;
            SaveProfileSettings.Profile = profile;
        }

        public void OnClickLoadMainMenu () {
            _loaderManager.LoadAsyncScene(_mMenuSceneID);
        }

        #endregion
    }
}
