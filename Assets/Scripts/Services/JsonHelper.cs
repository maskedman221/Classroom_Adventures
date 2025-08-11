
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrapped = $"{{\"Items\":{json}}}";
        return JsonUtility.FromJson<Wrapper<T>>(wrapped).Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}