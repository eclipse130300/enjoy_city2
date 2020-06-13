
using CMS.Config;
using UnityEngine;

namespace SocialGTA {

    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        MapConfig cityScene;
        [SerializeField]
        MapConfig createCharacterScene;
        [SerializeField]
        MapConfig registerScene;


        AutorizationController autorization = new AutorizationController();
        private void Start()
        {
            
        }
        public void OnPlayClick() {
          // autorization.LogOut();
            autorization.Login();
            if (autorization.isAutorized)
                Loader.Instance.LoadGameScene(cityScene);
            else
                Loader.Instance.LoadGameScene(registerScene);

        }

    }
    
}
