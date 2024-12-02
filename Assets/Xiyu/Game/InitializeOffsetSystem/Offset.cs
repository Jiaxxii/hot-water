using UnityEngine;

namespace Xiyu.Game.InitializeOffsetSystem
{
    [System.Serializable]
    public readonly struct Offset
    {
        public Offset(string fileName, Vector2 positionOffset, CapsuleCollider2D capsuleCollider2D)
        {
            FileName = fileName;
            PositionOffset = positionOffset;
            CapsuleCollider2D = capsuleCollider2D;
        }

        public string FileName { get; }
        public Vector2 PositionOffset { get; }
        public CapsuleCollider2D CapsuleCollider2D { get; }


        public static Offset None => new(string.Empty, Vector2.zero, CapsuleCollider2D.None);
    }
}