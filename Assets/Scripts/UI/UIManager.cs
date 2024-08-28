using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Operation
{
    Set,
    Add,
    Subtract
}

namespace UI
{
    [System.Serializable]
    public struct UIAsset
    {
        public string name;
        public UnityEngine.Object asset;
    }

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<UIAsset> assets = new List<UIAsset>();
        [SerializeField] private bool CacheOnStart; 
        private Dictionary<string, UIAsset> cachedAssets = new Dictionary<string, UIAsset>();

        private void Start()
        {
            if (CacheOnStart)
            {
                for (int i = 0; i < assets.Count; i++)
                {
                    FindAssetByName(assets[i].name); 
                }
            }
        }
        public UIAsset FindAssetByName(string name)
        {
            if (cachedAssets.TryGetValue(name, out UIAsset cachedAsset))
            {
                return cachedAsset;
            }

            UIAsset? foundAsset = assets.Find(x => x.name == name);
            if (foundAsset.HasValue)
            {
                cachedAssets[name] = foundAsset.Value;
                return foundAsset.Value;
            }

            ErrorHandler.LoadErrorScene($"Couldn't find UI asset {name}", ErrorHandler.ErrorType.Error); 
            return default;
        }

        #region Sliders
        public Slider ModifySlider(string name, float amount, Operation operation = Operation.Set)
        {
            Slider staminaMeter = (Slider)FindAssetByName(name).asset;

            switch (operation)
            {
                case Operation.Set:
                    staminaMeter.value = amount;
                    break;
                case Operation.Add:
                    staminaMeter.value += amount;
                    break;
                case Operation.Subtract:
                    staminaMeter.value -= amount;
                    break;
            }

            return staminaMeter;
        }

        public float ModifySlider_Value(string name, float amount, Operation operation = Operation.Set)
        {
            return ModifySlider(name, amount, operation).value;
        }
        #endregion

        #region General 
        public bool ToggleObject(string name, bool value, bool tog)
        {
            GameObject obj = (GameObject)FindAssetByName(name).asset;
            if (tog)
            {
                obj.SetActive(!obj.activeSelf);
                return obj.activeSelf; 
            }

            obj.SetActive(value);
            return value; 
        }
        #endregion

        public void ClearCache()
        {
            cachedAssets.Clear();
        }

        public void RemoveFromCache(string name)
        {
            if (cachedAssets.TryGetValue(name, out UIAsset cachedAsset))
            {
                cachedAssets.Remove(name);
            }
        }
    }
}
