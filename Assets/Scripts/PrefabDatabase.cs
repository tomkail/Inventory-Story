using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System;
using UnityEditor;
#endif   

public class PrefabDatabase : MonoSingleton<PrefabDatabase> {
    public Level levelPrefab;
    public ItemView itemViewPrefab;
    public ItemDraggableGhostView itemDraggableGhostViewPrefab;

    public AssetDictionary<Level> levels;
    public AssetDictionary<AudioClip> musicTracks;
    #if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void Init() {
        Instance.AutoPopulate();
    }

    [ContextMenu("Populate")]
    void AutoPopulate() {
        Instance.levels = CreatePrefabDictionaryFromLabel<Level>("level");
        Instance.musicTracks = CreatePrefabDictionaryFromLabel<AudioClip>("Music");
    }

    public static AssetDictionary<T> CreatePrefabDictionaryFromLabel<T>(string label, Func<bool, T> validator = null) where T : Object {
        var filter = $"l:{label}";
        return CreatePrefabDictionaryFromFilter<T>(filter, null, validator);
    }

    public static AssetDictionary<T> CreatePrefabDictionaryFromFilter<T>(string filter, string[] searchInFolders = null, Func<bool, T> validator = null) where T : Object {
        var guids = searchInFolders == null ? AssetDatabase.FindAssets(filter) : AssetDatabase.FindAssets(filter, searchInFolders);
        var dictionary = new AssetDictionary<T>();
        foreach (string guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<T>(path);
            if(prefab != null && (validator == null || validator(prefab)))
                dictionary.Add(prefab.name, prefab);
        }
        return dictionary;
    }
    #endif
    
    [System.Serializable]
    public class AssetDictionary<T> : DictionaryWrapper<string, T> where T : Object {}

    [System.Serializable]
    public class DictionaryWrapper<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [System.Serializable]
        public class KeyValue {
            public TKey key;
            public TValue value;
        }

        [SerializeField]
        private List<KeyValue> m_Data = new List<KeyValue>();

        public void OnAfterDeserialize() {
            Clear();
            foreach(var kv  in m_Data)
                Add(kv.key, kv.value);
        }

        public void OnBeforeSerialize() {
            m_Data.Clear();
            foreach (var kv in this)
                m_Data.Add(new KeyValue { key = kv.Key, value = kv.Value });
        }
    }
}