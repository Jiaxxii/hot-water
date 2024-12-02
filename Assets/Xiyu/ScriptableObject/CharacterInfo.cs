using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Xiyu.ScriptableObject
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "ScriptableObject/CharacterInfo")]
    public class CharacterInfo : UnityEngine.ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private AssetReferenceGameObject referenceGameObject;

        public string Name => name;
        public AssetReferenceGameObject ReferenceGameObject => referenceGameObject;
    }
}