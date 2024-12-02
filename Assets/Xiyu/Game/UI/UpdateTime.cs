using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Xiyu.Game.UI
{
    public class UpdateTime : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private string timeFormat = "yyyy-MM-dd HH:mm:ss";
        [SerializeField] private float updateFrequencySecond = 0.2F;

        private void Start()
        {
            UpdateTimeTextForget().Forget();
        }

        private async UniTaskVoid UpdateTimeTextForget()
        {
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                textMeshProUGUI.text = System.DateTime.Now.ToString(timeFormat);
                await UniTask.WaitForSeconds(updateFrequencySecond);
            }
        }
    }
}