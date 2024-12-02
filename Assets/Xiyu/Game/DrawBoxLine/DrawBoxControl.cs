using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Xiyu.Game.DrawBoxLine
{
    public class DrawBoxControl : MonoBehaviour
    {
        [SerializeField] private RectTransform textInfoParent;

        [Space] [Header("检测对象")] [SerializeField]
        private int countCheck = 5;

        [SerializeField] private float checkIntervalSeconds = 1F;


        [Space] [Header("更新绘制")] [SerializeField]
        private Vector2 drawBoxOffsetXRange;

        [SerializeField] private Vector2 drawBoxOffsetYRange;
        [SerializeField] private float updateFrequency = 0.1F;

        public float UpdateFrequency => updateFrequency;

        private record DrawObject(TextMeshProUGUI InfoText, BoxLine BoxLine)
        {
            public TextMeshProUGUI InfoText { get; } = InfoText;
            public BoxLine BoxLine { get; } = BoxLine;
        }

        private readonly Dictionary<int, DrawObject> _drawObjects = new();


        // private void Start()
        // {
        //     CheckObjectForget(countCheck, checkIntervalSeconds).Forget();
        //     UpDateDraw().Forget();
        // }

        public void Run()
        {
            CheckObjectForget(countCheck, checkIntervalSeconds).Forget();
            UpDateDraw().Forget();
        }


        private async UniTaskVoid CheckObjectForget(int checkCount, float checkIntervalTime)
        {
            while (checkCount-- > 0 || destroyCancellationToken.IsCancellationRequested)
            {
                foreach (var capsuleCollider2D in FindObjectsOfType<CapsuleCollider2D>().Where(cc => !_drawObjects.ContainsKey(cc.gameObject.GetInstanceID())))
                {
                    var drawObject = CreateFunc();

                    drawObject.BoxLine.transform.SetParent(transform);
                    drawObject.BoxLine.Init(capsuleCollider2D, drawObject.InfoText);

                    _drawObjects.Add(capsuleCollider2D.gameObject.GetInstanceID(), drawObject);
                }

                await UniTask.WaitForSeconds(checkIntervalTime);
            }
        }


        private async UniTaskVoid UpDateDraw()
        {
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                foreach (var drawObject in _drawObjects.Select(d => d.Value))
                {
                    var drawOffset = new Vector2(Random.Range(drawBoxOffsetXRange.x, drawBoxOffsetXRange.y), Random.Range(drawBoxOffsetYRange.x, drawBoxOffsetYRange.y));

                    var content = new Vector2(drawObject.BoxLine.CapsuleCollider.transform.position.x, drawObject.BoxLine.CapsuleCollider.transform.position.y);
                    drawObject.BoxLine.Draw(drawOffset, content.ToString());
                }

                await UniTask.WaitForSeconds(updateFrequency);
            }
        }


        private DrawObject CreateFunc()
        {
            var boxLine = Instantiate(Resources.Load<BoxLine>("DrawBox"), Vector3.zero, Quaternion.identity);
            var infoText = Instantiate(Resources.Load<TextMeshProUGUI>("DrawInfoText"), textInfoParent);

            return new DrawObject(infoText, boxLine);
        }
    }
}