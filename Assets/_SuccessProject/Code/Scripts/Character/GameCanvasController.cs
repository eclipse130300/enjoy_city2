using System.Collections;
using UnityEngine;

namespace SocialGTA {

    public class GameCanvasController : BoltSingletonPrefab<GameCanvasController> {

        #region Variables

        [Header("Settings")]
        [SerializeField] string _showClip;
        [SerializeField] string _hideClip;
        [SerializeField] Joystick _joystick;
        [SerializeField] CustomAnimator _pauseAnim;
        [SerializeField] SceneLoaderManager _loaderManager;
        [SerializeField] TouchPlaneController touchPlane;

        private bool _isJumping;

        public float Vertical => _joystick.Vertical;

        public float Horizontal => _joystick.Horizontal;

        public Vector2 Direction => _joystick.Direction;

        private bool _isJump;
        public bool IsJump { get { return _isJump; } }
        public Vector2 TouchDirecition => touchPlane.Direction;

        #endregion

        #region Standart Functions

        private void Awake() {
            DontDestroyOnLoad(this);
        }

        #endregion

        #region Custom Functions

        #region Pause Menu

        public void OnClickOpenMenu() {
            _pauseAnim.gameObject.SetActive(true);
            _pauseAnim.Play(_showClip);
        }

        public void OnClickCloseMenu() {
            StartCoroutine(ClosePauseAnim());
        }

        private IEnumerator ClosePauseAnim() {
            _pauseAnim.Play(_hideClip);

            yield return new WaitForSeconds(_pauseAnim.GetAnimationClip(_hideClip).length);

            _pauseAnim.gameObject.SetActive(false);

            yield break;
        }

        public void OnClickLoadMainMenu() {
            _loaderManager.DiscconnectFromServer();
        }

        #endregion

        public void OnDownAndUpJump(bool isJump) {
            var i = 0;
            int b = i;

            if (isJump && !_isJumping) {
                StartCoroutine(Jump(isJump));
            }
            if (!isJump) {
                _isJump = isJump;
                _isJumping = false;
                StopAllCoroutines();
            }

        }

        private IEnumerator Jump(bool isJump) {
            _isJump = isJump;
            _isJumping = true;

            yield return new WaitForSeconds(0.01f);
            _isJumping = false;
            _isJump = !isJump;

            yield break;
        }

        #endregion

    }
}
