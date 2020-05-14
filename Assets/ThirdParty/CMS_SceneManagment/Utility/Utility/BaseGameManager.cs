using System.Collections;
using UnityEngine;
using Utils;

namespace Utils
{
    public abstract class BaseGameManager<T> : MonoBehaviour where T : BaseGameManager<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        protected bool  _ready;

        private float   _initProgress;

        private Coroutine _internalInit;
        private Coroutine _startupInit;

        public bool IsReady
        {
            get { return _ready; }
        }

        public float InitProgress
        {
            get { return _initProgress; }

            set
            {
                _initProgress = value;
                _initProgress = Mathf.Clamp01(_initProgress);
            }
        }

        protected virtual void Awake()
        {
            SingletonUtils.SetSingleton((T) this, ref _instance);
            Init();
        }

        protected virtual void OnDestroy()
        {
            if (_startupInit != null)
                StopCoroutine(_startupInit);

            if (_internalInit != null)
                StopCoroutine(_internalInit);

            _instance = null;
            
        }

        public void Init()
        {
            _internalInit = StartCoroutine(InternalInit());
        }

        public IEnumerator WaitFor()
        {
            while (!_ready)
            {
                yield return null;
            }
        }

        protected virtual IEnumerator Startup()
        {
            yield return null;
        }

        private IEnumerator InternalInit()
        {
            if (_startupInit != null)
                StopCoroutine(_startupInit);
            _startupInit = StartCoroutine(Startup());

            yield return _startupInit;
            _ready = true;
        }
    }
}
