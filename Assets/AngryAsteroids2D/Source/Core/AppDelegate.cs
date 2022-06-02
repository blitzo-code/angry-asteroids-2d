using System.Collections;
using AngryAsteroids2D.Source.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AngryAsteroids2D.Source.Core
{
    public class AppDelegate : MonoBehaviour
    {
        bool _isLoading;
        
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && !_isLoading)
            {
                StartCoroutine(LoadGameplayScene());
            }
        }

        IEnumerator LoadGameplayScene()
        {
            _isLoading = true;
            
            var asyncLoadOperation = SceneManager.LoadSceneAsync("GameplayScene");
            while (!asyncLoadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
