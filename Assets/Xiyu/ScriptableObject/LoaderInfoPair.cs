using System;
using UnityEngine;

namespace Xiyu.ScriptableObject
{
    /// <summary>
    /// 这是一个ScriptableObject，用于表示一个立绘的加载（初始化）信息
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "LoaderScriptableObject", menuName = "ScriptableObject/LoaderInfoPair")]
    public class LoaderInfoPair : UnityEngine.ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private LoaderInfo bodyLoaderInfo;
        [SerializeField] private LoaderInfo faceLoaderInfo;


        /// <summary>
        /// 立绘名称
        /// </summary>
        public string Name => name;

        /// <summary>
        /// 身体部分的加载信息
        /// </summary>
        public LoaderInfo BodyLoaderInfo => bodyLoaderInfo;

        /// <summary>
        /// 脸部的加载信息
        /// </summary>
        public LoaderInfo FaceLoaderInfo => faceLoaderInfo;
    }
}