using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.ScriptableObject;

namespace Xiyu.Game
{
    public class CharacterController : MonoBehaviour
    {
        private async void Start()
        {
            
#if !UNITY_EDITOR
            if (SystemInfo.operatingSystem.StartsWith("Windows"))
            {
                Cursor.visible = false;
            }
#endif


            var characterLoaderSettings = (CharacterLoaderSettings)await Resources.LoadAsync<CharacterLoaderSettings>("Settings/CharacterLoaderSettings");
            var loaderInfoDictionary = ((LoaderSettings)await Resources.LoadAsync<LoaderSettings>("Settings/LoaderScriptableObject")).ToDictionary();

            foreach (var characterInfo in characterLoaderSettings)
            {
                if (!loaderInfoDictionary.TryGetValue(characterInfo.Name, out var loaderInfoPair))
                {
                    throw new KeyNotFoundException($"配置文件(Settings/LoaderScriptableObject.asset)中没有找到名为\"{characterInfo.Name}\"的LoaderInfoPair(初始化偏移信息)");
                }

                // 角色生成器
                var characterGeneration = await CharacterGeneration.Create(loaderInfoPair, characterInfo.ReferenceGameObject, Vector3.zero, Quaternion.identity);

                // 初始化角色
                await characterGeneration.Character.Init(characterGeneration.Character.CharacterName + "_face_0", characterGeneration);
            }
        }
    }
}