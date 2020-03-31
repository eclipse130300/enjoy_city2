using UnityEngine;
using System.Collections;

namespace SocialGTA.Network {

    public class PlayerSkinsManager : Bolt.EntityEventListener<IPlayerState> {

        [SerializeField] CharacterSkinModel[] _models;

        public override void Attached() {
            state.SetAnimator(_models[0].animator);

            if (entity.IsOwner) {
                for (var i = 0; i < state.SkinArray.Length; ++i)
                    state.SkinArray[i].SkinID = i;

                state.SkinActivateIndex = -1;
            }

            state.AddCallback("SkinActivateIndex", ChangeSkinHandler);

            if (entity.IsOwner) {
                var profile = SaveProfileSettings.Profile ?? new ProfileModel { characterIndex = 0, isChooseCharacter = true };
                state.SkinActivateIndex = profile.characterIndex;
            }
        }

        private void ChangeSkinHandler() {
            for (int i = 0; i < _models.Length; ++i)
                _models[i].obj.SetActive(false);

            if (state.SkinActivateIndex >= 0) {
                int skinID = state.SkinActivateIndex;
                _models[skinID].obj.SetActive(true);
                state.SetAnimator(_models[skinID].animator);
                Debug.Log("Skin ID: " + skinID);
            }
        }
    }
}
