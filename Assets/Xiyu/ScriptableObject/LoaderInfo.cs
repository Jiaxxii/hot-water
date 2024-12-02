using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Xiyu.ScriptableObject
{
    /// <summary>
    /// 需要加载的资源信息，主要包含图片的引用和偏移信息
    /// </summary>
    [Serializable]
    public struct LoaderInfo
    {
        [SerializeField] private UnityEngine.AddressableAssets.AssetReferenceSprite[] referenceSprites;
        [SerializeField] private TextAsset offsetJson;

        private IReadOnlyDictionary<string, UnityEngine.AddressableAssets.AssetReferenceSprite> _buffer;


        /// <summary>
        /// 根据资源的“SubObjectName”来获取对应的资源引用
        /// <para>*SubObjectName需要再引用资源后手动设置</para>
        /// </summary>
        public IReadOnlyDictionary<string, UnityEngine.AddressableAssets.AssetReferenceSprite> AssetReferenceSpriteDictionary =>
            _buffer ??= referenceSprites.ToDictionary(ars => ars.SubObjectName);

        /// <summary>
        /// 资源的json偏移信息（它可能是空的）
        /// </summary>
        [CanBeNull] public TextAsset OffsetJson => offsetJson;


        /// <summary>
        /// 获取随机的资源引用 (一般只对“body”使用)
        /// </summary>
        /// <returns></returns>
        public UnityEngine.AddressableAssets.AssetReferenceSprite GetRandomAssetReferenceSprite()
        {
            return referenceSprites[UnityEngine.Random.Range(0, referenceSprites.Length)];
        }
    }
}