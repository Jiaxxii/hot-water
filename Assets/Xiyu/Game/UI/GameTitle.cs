using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.Game.DrawBoxLine;

namespace Xiyu.Game.UI
{
    public class GameTitle : MonoBehaviour
    {
        [Serializable]
        private struct Line : IEnumerable<char>
        {
            [SerializeField] private string lineContent;
            [SerializeField] private float endDelaySeconds;
            [SerializeField] private float characterDelaySeconds;

            public float EndDelaySeconds => endDelaySeconds;

            public Line(string lineContent, float endDelaySeconds, float characterDelaySeconds)
            {
                this.lineContent = lineContent;
                this.endDelaySeconds = endDelaySeconds;
                this.characterDelaySeconds = characterDelaySeconds;
            }

            public float CharacterDelaySeconds => characterDelaySeconds;

            public IEnumerator<char> GetEnumerator()
            {
                foreach (var c in lineContent) yield return c;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Header("引用")] [SerializeField] private Image consolePanel;
        [SerializeField] private TextMeshProUGUI consoleText;

        [Header("音效")] [SerializeField] private AudioClip newLineClip;
        [SerializeField] private AudioClip switchPanelClip;


        [SerializeField] private float whiteDurationSeconds = 1;
        [SerializeField] private float closePanelDurationSeconds = 1;


        [SerializeField] private List<Line> contentLines;

        private void Awake()
        {
            PlayerPrefs.SetInt("OPEN GAME COUNT", PlayerPrefs.HasKey("OPEN GAME COUNT") ? PlayerPrefs.GetInt("OPEN GAME COUNT") + 1 : 1);
#if !UNITY_EDITOR
            if (SystemInfo.operatingSystem.StartsWith("Windows"))
            {
                consoleText.fontSize = 15;
            }
#endif
        }


        private async void Start()
        {
            var stringBuilder = new StringBuilder(1024);
            contentLines.Add(new Line($"OPEN GAME COUNT - {PlayerPrefs.GetInt("OPEN GAME COUNT", 1)}", 1, 0.01F));

            foreach (var line in contentLines)
            {
                AudioSource.PlayClipAtPoint(newLineClip, Vector3.zero);
                foreach (var character in line)
                {
                    consoleText.text = stringBuilder.Append(character).ToString();
                    await UniTask.WaitForSeconds(line.CharacterDelaySeconds);
                }

                consoleText.text = stringBuilder.AppendLine().ToString();
                await UniTask.WaitForSeconds(line.EndDelaySeconds);
            }


            AudioSource.PlayClipAtPoint(switchPanelClip, Vector3.zero);
            consoleText.DOColor(Color.white, whiteDurationSeconds);
            await consolePanel.DOColor(Color.white, whiteDurationSeconds).AsyncWaitForCompletion().AsUniTask();
            consoleText.gameObject.SetActive(false);

            var drawBoxControl = FindObjectOfType<DrawBoxControl>();
            drawBoxControl.Run();
            FindObjectOfType<ScreenCrosshair>().Run(Player.PlayerControl.InputSystem.UI.Point, upDateFrequency: drawBoxControl.UpdateFrequency);

            consolePanel.DOFade(0, closePanelDurationSeconds).OnComplete(() =>
            {
                contentLines = null;
                Destroy(consolePanel.gameObject);
                Destroy(this);
            });
        }
    }
}