using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Xiyu.Game.InitializeOffsetSystem;
using Xiyu.ScriptableObject;

namespace Xiyu.Game
{
    public sealed class CharacterGeneration
    {
        private CharacterGeneration()
        {
        }

        private LoaderInfoPair _loaderInfoPair;
        private IReadOnlyDictionary<string, Offset> _bodyOffsetsMap;
        private AsyncOperationHandle<GameObject> _asyncOperationHandle;

        public Character Character { get; private set; }

        private CharacterGeneration Init(LoaderInfoPair loaderInfoPair)
        {
            _loaderInfoPair = loaderInfoPair;
            _bodyOffsetsMap = LoadOffsets(loaderInfoPair.BodyLoaderInfo.OffsetJson);
            return this;
        }

        public async UniTask TryWaitDisplay()
        {
            if (!_asyncOperationHandle.IsDone)
            {
                await _asyncOperationHandle;
            }
        }

        public async UniTask<(Sprite body, Sprite face, Offset offset)> GetRandomBody(string faceSpriteName)
        {
            var body = await LoadBodySpriteAsync(_loaderInfoPair.BodyLoaderInfo.GetRandomAssetReferenceSprite().SubObjectName);
            var face = await LoadFaceSpriteAsync(faceSpriteName);
            return (body.Item1, face, body.Item2);
        }

        public async UniTask<(Sprite, Offset)> LoadBodySpriteAsync(string bodySpriteName)
        {
            if (!_loaderInfoPair.BodyLoaderInfo.AssetReferenceSpriteDictionary.TryGetValue(bodySpriteName, out var assetReferenceSprite))
            {
                throw new KeyNotFoundException($"No reference found for \"{bodySpriteName}\" in \"{_loaderInfoPair.Name}\"");
            }

            var offset = Offset.None;
            if (_bodyOffsetsMap != null && !_bodyOffsetsMap.TryGetValue(bodySpriteName, out offset))
            {
                offset = _bodyOffsetsMap.FirstOrDefault(b => b.Value.FileName == bodySpriteName).Value;
            }

            if (assetReferenceSprite.Asset != null)
            {
                return ((Sprite)assetReferenceSprite.Asset, offset);
            }

            var sprite = await assetReferenceSprite.LoadAssetAsync<Sprite>();
            return (sprite, offset);
        }

        public async UniTask<Sprite> LoadFaceSpriteAsync(string faceSpriteName)
        {
            if (!_loaderInfoPair.FaceLoaderInfo.AssetReferenceSpriteDictionary.TryGetValue(faceSpriteName, out var assetReferenceSprite))
            {
                throw new KeyNotFoundException($"No reference found for \"{faceSpriteName}\" in \"{_loaderInfoPair.Name}\"");
            }

            if (assetReferenceSprite.Asset != null)
            {
                return (Sprite)assetReferenceSprite.Asset;
            }

            return await assetReferenceSprite.LoadAssetAsync<Sprite>();
        }

        private static IReadOnlyDictionary<string, Offset> LoadOffsets([CanBeNull] TextAsset offsetJson)
        {
            return offsetJson != null && !string.IsNullOrEmpty(offsetJson?.text)
                ? JsonConvert.DeserializeObject<Dictionary<string, Offset>>(offsetJson.text)
                : null;
        }

        public static async UniTask<CharacterGeneration> Create(LoaderInfoPair loaderInfoPair, AssetReferenceGameObject characterReference, Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            var characterGeneration = new CharacterGeneration
            {
                _asyncOperationHandle = characterReference.InstantiateAsync(position, rotation, parent)
            };

            characterGeneration.Character = (await characterGeneration._asyncOperationHandle).GetComponent<Character>();
            return characterGeneration.Init(loaderInfoPair);
        }
    }
}