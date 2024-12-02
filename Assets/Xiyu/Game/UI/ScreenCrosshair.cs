using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Xiyu.Game.Player;
using Random = UnityEngine.Random;

namespace Xiyu.Game.UI
{
    public class ScreenCrosshair : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRendererX, lineRendererY;

        [SerializeField] private InputAction pointInputAction;
        [Space] [SerializeField] private float defaultLineWidth;
        [SerializeField] private float defaultUpDateFrequency;
        [SerializeField] private Vector2 defaultTargetOffset;
        [SerializeField] private Color defaultLineColor;

        private Camera _mainCamera;

        public void Run(InputAction pointInput, float? lineWidth = null, float? upDateFrequency = null, Vector2? targetOffset = null, Color? lineColor = null)
        {
            pointInputAction = pointInput;
            lineRendererX.widthMultiplier = lineWidth ?? defaultLineWidth;
            lineRendererY.widthMultiplier = lineWidth ?? defaultLineWidth;
            _mainCamera = Camera.main;

            UpdateCrosshairLine(upDateFrequency ?? defaultUpDateFrequency, targetOffset ?? defaultTargetOffset, lineColor ?? defaultLineColor).Forget();
        }


        private async UniTaskVoid UpdateCrosshairLine(float upDateFrequency, Vector2 targetOffset, Color lineColor)
        {
            lineRendererX.colorGradient = new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(lineColor, 0),
                    new GradientColorKey(lineColor, 1)
                }
            };

            lineRendererY.colorGradient = new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(lineColor, 0),
                    new GradientColorKey(lineColor, 1)
                }
            };

            while (!destroyCancellationToken.IsCancellationRequested)
            {
                var screenToWorldPoint = _mainCamera.ScreenToWorldPoint(pointInputAction.ReadValue<Vector2>());


                lineRendererX.transform.position = new Vector3(0, screenToWorldPoint.y + Random.Range(targetOffset.x, targetOffset.y), 0);

                lineRendererY.transform.position = new Vector3(screenToWorldPoint.x + Random.Range(targetOffset.x, targetOffset.y), 0, 0);

                await UniTask.WaitForSeconds(upDateFrequency);
            }
        }
    }
}