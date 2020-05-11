using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;


namespace Utils
{
    public class ProgressBar : MonoBehaviour
    {
/* Old GUI progress bar
        private float percent = 1f;
        public Texture2D bgImage;
        public Texture2D fgImage;
        public Transform OwnerTransform;

        public float barWidth=50;
        public float barHeight=8;
      
        void OnGUI()
        {
            if (percent < 1)
            {
                GUI.depth = 100;
                Vector2 targetPos;
                targetPos = Camera.main.WorldToScreenPoint(OwnerTransform.position);

                GUI.DrawTexture(new Rect(targetPos.x, Screen.height - targetPos.y, barWidth, barHeight), bgImage);
                GUI.DrawTexture(new Rect(targetPos.x, Screen.height - targetPos.y, barWidth*percent, barHeight), fgImage);
            }
        }

        public void SetPercent(float _percent)
        {
            percent = _percent;
            OnHealthChanged(_percent);
        }

*/
        #region PUBLIC_REFERENCES
        public RectTransform targetCanvas;
        public RectTransform healthBar;
        public Image healthBarImage;
        public Transform objectToFollow;
        #endregion
        #region PUBLIC_METHODS
/*
        public void SetHealthBarData(Transform targetTransform, RectTransform healthBarPanel)
        {
            this.targetCanvas = healthBarPanel;
            healthBar = GetComponent<RectTransform>();
            objectToFollow = targetTransform;
            RepositionHealthBar();
            healthBar.gameObject.SetActive(true);
        }
*/
        public void SetPercent(float healthFill)
        {
            healthBarImage.fillAmount = healthFill;
        }
        #endregion
        #region UNITY_CALLBACKS
        void Update()
        {
            RepositionHealthBar();
        }
        #endregion
        #region PRIVATE_METHODS
        private void RepositionHealthBar()
        {
            if (!objectToFollow)
                return;
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectToFollow.position);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)));
            //now you can set the position of the ui element
            healthBar.anchoredPosition = WorldObject_ScreenPosition;
        }
        #endregion

 

    }

}