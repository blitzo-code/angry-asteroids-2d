#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using AngryAsteroids2D.Source.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AngryAsteroids2D.Source.Core
{
    public class AppDelegate : MonoBehaviour
    {
        static AppDelegate INSTANCE;
        
        void Awake()
        {
            if (INSTANCE)
            {
                HierarchyUtils.LogDuplicatedSingletonError(GetType().Name, gameObject);
                return;
            }
            INSTANCE = this;
            
            Init();
        }

        void Init()
        {
            StartCoroutine(LoadGameplayScene());
        }

        IEnumerator LoadGameplayScene()
        {
            var asyncLoadOperation = SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Additive);
            while (!asyncLoadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
