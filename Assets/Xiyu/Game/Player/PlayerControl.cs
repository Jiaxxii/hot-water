using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Xiyu.Game.Player
{
    public class PlayerControl : MonoBehaviour
    {
        public static PlayerInputSystem InputSystem { get; private set; }


        private Camera _mainCamera;


        private ObjectPool<HotWaterObject> _hotWaterObjectPool;

        [SerializeField] private Transform firePoint;
        [SerializeField] private float tiltAngle = 25F;
        [SerializeField] private float tiltDuration = 0.5F;


        private bool _isFiring;


        private void Awake()
        {
            InputSystem = new PlayerInputSystem();
            _mainCamera = Camera.main;

            Application.targetFrameRate = 60;


            InputSystem.UI.Point.performed += TrackMovement;
            InputSystem.Player.Fire.performed += OnFireEventHandle;

            _hotWaterObjectPool = new ObjectPool<HotWaterObject>(CreateFunc, OnGetAction, water => water.gameObject.SetActive(false));

            return;

            void OnGetAction(HotWaterObject waterObject)
            {
                waterObject.gameObject.SetActive(true);
                waterObject.OnHotWaterCollisionEnter += coll => OnHotWaterCollisionEnterEventHandle(waterObject, coll);
            }

            HotWaterObject CreateFunc()
            {
                var water = Instantiate(Resources.Load<HotWaterObject>("Hot water"), firePoint.transform.position, Quaternion.identity);
                water.transform.SetParent(firePoint);

                return water;
            }
        }


        private void OnEnable()
        {
            InputSystem.Enable();
            // Cursor.visible = false;
            // Cursor.lockState = CursorLockMode.Confined;
        }

        private void OnDisable()
        {
            InputSystem.Disable();
            // Cursor.visible = true;
        }


        private void OnFireEventHandle(InputAction.CallbackContext context)
        {
            FireSustainedForget().Forget();
        }


        private async UniTaskVoid FireSustainedForget(float duration = -1)
        {
            if (_isFiring) return;

            _isFiring = true;

            await transform.DOLocalRotate(new Vector3(0, 0, tiltAngle), tiltDuration)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion().AsUniTask();


            // 此处触发 hotWaterObject 的 OnEnable 事件
            var hotWaterObject = _hotWaterObjectPool.Get();

            // 初始化（这里会开始播放粒子动画）
            hotWaterObject.Initialized(duration);

            // 立刻进行回收计时
            hotWaterObject.HideForget(hotWaterObject.PreferredLifeTime, () => _hotWaterObjectPool.Release(hotWaterObject)).Forget();

            // 等待发射时间
            await UniTask.WaitForSeconds(hotWaterObject.Duration);

            await transform.DOLocalRotate(Vector3.zero, tiltDuration)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion().AsUniTask();

            _isFiring = false;
        }


        // 当热水与其他物体碰撞时 （粒子系统碰撞）
        private void OnHotWaterCollisionEnterEventHandle(HotWaterObject waterObject, GameObject other)
        {
            if (!other.TryGetComponent(typeof(IBeInjured), out var component))
            {
                return;
            }

            var beInjured = (IBeInjured)component;
            beInjured.Hurt(InjuryType.Empyrosis, null);
        }


        private Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            var screenToWorldPoint = _mainCamera.ScreenToWorldPoint(screenPoint);
            screenToWorldPoint.z = transform.position.z;
            return screenToWorldPoint;
        }

        private void TrackMovement(InputAction.CallbackContext callbackContext)
        {
            var readValue = InputSystem.UI.Point.ReadValue<Vector2>();

            transform.position = ScreenToWorldPoint(readValue);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(typeof(IBeInjured), out var component))
            {
                return;
            }

            var beInjured = (IBeInjured)component;
            beInjured.Hurt(InjuryType.Empyrosis, transform.position);
        }
    }
}