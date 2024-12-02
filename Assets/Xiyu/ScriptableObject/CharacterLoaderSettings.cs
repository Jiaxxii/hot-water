using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xiyu.ScriptableObject
{
    [CreateAssetMenu(fileName = "CharacterLoaderSettings", menuName = "ScriptableObject/CharacterLoaderSettings")]
    public class CharacterLoaderSettings : UnityEngine.ScriptableObject, IEnumerable<CharacterInfo>
    {
        [SerializeField] private List<CharacterInfo> characterReferences;

        public IEnumerator<CharacterInfo> GetEnumerator()
        {
            foreach (var assetReferenceGameObject in characterReferences)
            {
                yield return assetReferenceGameObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}