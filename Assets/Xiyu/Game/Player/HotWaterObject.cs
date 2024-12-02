using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Xiyu.Game.Player
{
    public class HotWaterObject : MonoBehaviour
    {
        [SerializeField] private ParticleSystem smokeParticle;


        public event UnityAction<GameObject> OnHotWaterCollisionEnter;

        private ParticleSystem.MainModule _mainModule;


        // public event UnityAction<HotWaterObject> On

        /// <summary>
        /// 最近的烟雾粒子的生命周期 (播放粒子特效后立马等待此时间（秒）确保粒子特效播放完毕（alpha=0）)
        /// </summary>
        public float PreferredLifeTime => _mainModule.duration + _mainModule.startLifetime.constant;


        public float Duration => _mainModule.duration;


        private bool _isHiding;

        private void Awake()
        {
            _mainModule = smokeParticle.main;
        }


        private void OnEnable()
        {
            smokeParticle.Stop();
        }


        public void Initialized(float duration = -1)
        {
            if (duration > 0)
                _mainModule.duration = duration;
            transform.localScale = Vector3.one;
            smokeParticle.Play();
        }


        public async UniTaskVoid HideForget(float seconds, Action onComplete)
        {
            if (_isHiding)
            {
                return;
            }

            _isHiding = true;

            await UniTask.WaitForSeconds(seconds);
            onComplete?.Invoke();
            _isHiding = false;
        }

        private void OnDisable()
        {
            OnHotWaterCollisionEnter = null;
        }

        private void OnParticleCollision(GameObject other)
        {
            OnHotWaterCollisionEnter?.Invoke(other);
        }

    }
}