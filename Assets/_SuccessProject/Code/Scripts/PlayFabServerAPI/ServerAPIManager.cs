using PlayFab;
using PlayFab.ClientModels;
using System;

namespace SocialGTA {

    public class ServerAPIManager {

        #region Custom Functions

        public static void Login (string _username, string _password, Action<LoginResult> _successfulCallback, Action<PlayFabError> _errorCallback = null) {
            var request = new LoginWithPlayFabRequest();

            request.Username = _username;
            request.Password = _password;

            PlayFabClientAPI.LoginWithPlayFab(request, _successfulCallback, error => {
                _errorCallback?.Invoke(error);
            });
        }

        public static void Registrate (string _username, string _email, string _passworld, Action<RegisterPlayFabUserResult> _successfulCallback, Action<PlayFabError> _errorCallback = null) {
            var request = new RegisterPlayFabUserRequest();

            request.TitleId = PlayFabSettings.TitleId;
            request.Email = _email;
            request.Username = _username;
            request.Password = _passworld;

            PlayFabClientAPI.RegisterPlayFabUser(request, _successfulCallback, error => {
                _errorCallback?.Invoke(error);
            });
        }

        /// <summary>
        /// Get account information from server
        /// </summary>
        /// <param name="_successufulCallback"></param>
        /// <param name="_errorCallback"></param>
        public static void GetAccountInfo (Action<GetAccountInfoResult> _successufulCallback, Action<PlayFabError> _errorCallback = null) {
            var request = new GetAccountInfoRequest();

            PlayFabClientAPI.GetAccountInfo(request, _successufulCallback, error => {
                _errorCallback?.Invoke(error);
            });
        }

        #endregion
    }
}


