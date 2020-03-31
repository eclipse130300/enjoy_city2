using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CustomAnimator : MonoBehaviour {

    #region Variables

    [Header("Settings")]
    
    [SerializeField] Animation _animation;
    
    #endregion

    #region Cistom Functions

    public void Play (string animName) {
        _animation.Play(animName);
    }

    public AnimationClip GetAnimationClip(string name) {
        return _animation.GetClip(name);
    }

    #endregion

}
