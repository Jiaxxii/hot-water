using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Xiyu.Game.InitializeOffsetSystem;
using Random = UnityEngine.Random;

namespace Xiyu.Game
{
    public enum InjuryType
    {
        Empyrosis
    }

    public interface IBeInjured
    {
        UniTaskVoid Hurt(InjuryType injuryType, object sender);
    }

    public class Character : MonoBehaviour, IBeInjured
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private string characterName;


        [Space] [Header("受伤设置")] [SerializeField]
        private float empyrosisJumpForce = 100;

        [SerializeField] private float empyrosisCollisionForce = 50F;

        [SerializeField] private float hurtQuitTime = 3f;

        [Space] [Header("受伤音效")] [SerializeField]
        private AudioSource audioSource;

        [SerializeField] private AudioClip[] hurtAudioClips;

        [Space] [SerializeField] private SpriteRenderer bodySpriteRenderer;
        [SerializeField] private UnityEngine.CapsuleCollider2D bodyCollider;
        [SerializeField] private SpriteRenderer faceSpriteRenderer;

        public CharacterGeneration CharacterGeneration { get; private set; }


        private bool _isHurt;

        public string CharacterName => characterName;


        private void Start()
        {
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public async UniTask Init(string faceSpriteName, CharacterGeneration characterGeneration)
        {
            CharacterGeneration = characterGeneration;
            var (body, face, offset) = await CharacterGeneration.GetRandomBody(faceSpriteName);
            SetBody(body, face, offset);
        }

        public void SetBody(Sprite body, Sprite face, Offset offset)
        {
            bodySpriteRenderer.sprite = body;
            faceSpriteRenderer.sprite = face;

            if (string.IsNullOrEmpty(offset.FileName)) return;

            faceSpriteRenderer.transform.position -= new Vector3(offset.PositionOffset.x, offset.PositionOffset.y, 0);
            bodyCollider.offset = offset.CapsuleCollider2D.Offset;
            bodyCollider.size = offset.CapsuleCollider2D.Size;
        }


        public async UniTaskVoid Hurt(InjuryType injuryType, object sender)
        {
            if (injuryType != InjuryType.Empyrosis || _isHurt) return;

            _isHurt = true;

            if (hurtAudioClips is { Length: > 0 })
            {
                audioSource.clip = hurtAudioClips[Random.Range(0, hurtAudioClips.Length)];
                audioSource.Play();
            }

            if (sender is Vector3 senderPosition)
                rb.AddForce((transform.position.x > senderPosition.x ? Vector2.right : Vector2.left) * empyrosisCollisionForce, ForceMode2D.Impulse);
            else
                rb.AddForce(Vector2.up * empyrosisJumpForce, ForceMode2D.Impulse);

            faceSpriteRenderer.sprite = await CharacterGeneration.LoadFaceSpriteAsync($"{characterName}_face_1");

            await UniTask.WaitForSeconds(hurtQuitTime);

            faceSpriteRenderer.sprite = await CharacterGeneration.LoadFaceSpriteAsync($"{characterName}_face_0");

            _isHurt = false;
        }
    }
}