namespace RunTime.Extensions
{
    using UnityEngine;

namespace RunTime.Utilities
{
    /// <summary>
    /// Thread-Safe olmayan, Unity Main Thread odaklı, Lazy-Loaded Singleton yapısı.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        #region Private Fields
        
        // Önbelleklenmiş instance
        private static T _instance;
        
        // Oyunun kapanıp kapanmadığını kontrol eden flag
        private static bool _isQuitting = false;
        
        #endregion

        #region Public Properties

        public static T Instance
        {
            get
            {
                // 1. Oyun kapanıyorsa yeni obje yaratma, null dön.
                // Bu, "SomeManager.Instance.Save()" gibi çağrıların oyun kapanırken hata vermesini önler.
                if (_isQuitting)
                {
                    Debug.LogWarning($"[MonoSingleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }

                // 2. Instance zaten varsa hemen dön (En hızlı yol)
                if (_instance != null) return _instance;

                // 3. Sahnede var mı diye ara (Pahalı işlem, sadece 1 kere çalışır)
                _instance = FindObjectOfType<T>();
                
                if (_instance != null) return _instance;

                // 4. Sahnede yoksa yeni yarat (Lazy Instantiation)
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name; // Objenin adı class adı olur
                _instance = obj.AddComponent<T>();
                    
                Debug.Log($"[MonoSingleton] Auto-created instance of {typeof(T)}");

                return _instance;
            }
        }
        
        #endregion

        #region Lifecycle Methods

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;

            // Eğer instance zaten set edilmişse ve bu o değilse, bu bir kopyadır. Yok et.
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // İlk atama
            _instance = this as T;
            
            // Eğer Singleton'ın sahne geçişlerinde ölmemesini istiyorsan:
            // DontDestroyOnLoad(gameObject); 
            // Not: Bunu opsiyonel bıraktım, her singleton kalıcı olmak zorunda değildir.
        }
        
        // Oyun kapanırken flag'i kaldır
        protected virtual void OnApplicationQuit()
        {
            //_isQuitting = true;
            _instance = null;
        }

        #endregion
    }
}
}