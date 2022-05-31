
using UnityEngine;

namespace AngryAsteroids2D.Source.Utils
{
    public static class LogUtils 
    {
        public static void LogKeyNotFoundError(string keyValueString)
        {
            Debug.LogError($"The key {keyValueString} was not found in the dictionary");
        }

        public static void LogComponentNotFoundError(string gameObjectName, string componentName)
        {
            Debug.LogError($"Could not found component {componentName} on {gameObjectName}");
        }

        public static void LogCouldNotInsertEntityError(string entityName, string systemName)
        {
            Debug.LogError($"Could not insert entity {entityName} on system {systemName}");
        }

        public static void LogCouldNotFindEntityError(string entityName, string systemName)
        {
            Debug.LogError($"Could not find entity {entityName} on system {systemName} entity collection");
        }

        public static void LogCouldNotCreateTypeError(string typeName)
        {
            Debug.LogError($"There is no construction defined for type {typeName}");
        }

        public static void LogPoolIsFullError()
        {
            Debug.LogError("The pool is full, can't create more objects");
        }

        public static void CouldNotFindObjectOnPoolError(string gameObjectId)
        {
            Debug.LogError($"Could not find game object {gameObjectId} on the pool");
        }
    }
}
