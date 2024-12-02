using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xiyu.ScriptableObject
{

    [CreateAssetMenu(fileName = "LoaderScriptableObject", menuName = "ScriptableObject/LoaderScriptableObject")]
    public class LoaderSettings : UnityEngine.ScriptableObject
    {
        [SerializeField] private List<LoaderInfoPair> loaderPairs;

        public IReadOnlyDictionary<string, LoaderInfoPair> ToDictionary()
        {
            return loaderPairs.ToDictionary(loaderInfoPair => loaderInfoPair.Name);
        }
    }
}