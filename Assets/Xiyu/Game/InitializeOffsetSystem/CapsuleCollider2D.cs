using System;
using UnityEngine;

namespace Xiyu.Game.InitializeOffsetSystem
{
    [Serializable]
    public readonly struct CapsuleCollider2D
    {
        public CapsuleCollider2D(Vector2 offset, Vector2 size)
        {
            Offset = offset;
            Size = size;
        }

        public Vector2 Offset { get; }
        public Vector2 Size { get; }


        public static CapsuleCollider2D None => new(Vector2.zero, Vector2.zero);
    }
}