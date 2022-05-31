using UnityEngine;

namespace AngryAsteroids2D.Source.Utils
{
    public static class HierarchyUtils 
    {
        public static void LogDuplicatedSingletonError(string duplicatedSingletonName, GameObject duplicatedSingletonObject)
        {
            Debug.LogError($"There is already an {duplicatedSingletonName} on the scene");
            duplicatedSingletonObject.name = "<REMOVE_ME>" + duplicatedSingletonObject.name;
            duplicatedSingletonObject.SetActive(false);
        }
    }
}
