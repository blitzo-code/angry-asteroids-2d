
using UnityEngine;

namespace AngryAsteroids2D.Source.Utils
{
    public static class LogUtils 
    {
        public static void LogCouldNotInsertEntityError(string entityName, string systemName)
        {
            Debug.LogError($"Could not insert entity {entityName} on system {systemName}");
        }

        public static void LogCouldNotFindEntityError(string entityName, string systemName)
        {
            Debug.LogError($"Could not find entity {entityName} on system {systemName} entity collection");
        }
        
        public static void LogPoolIsFullError()
        {
            Debug.LogError("The pool is full, can't create more objects");
        }

        public static void CouldNotFindObjectOnPoolError(string gameObjectId)
        {
            Debug.LogError($"Could not find game object {gameObjectId} on the pool");
        }
        
        public static void LogDuplicatedSingletonError(string duplicatedSingletonName, GameObject duplicatedSingletonObject)
        {
            Debug.LogError($"There is already an {duplicatedSingletonName} on the scene");
            duplicatedSingletonObject.name = "<REMOVE_ME>" + duplicatedSingletonObject.name;
            duplicatedSingletonObject.SetActive(false);
        }
    }
}
