using TMPro;
using UnityEngine;

namespace Xiyu.Game.DrawBoxLine
{
    public class BoxLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        public CapsuleCollider2D CapsuleCollider { get; private set; }
        public TextMeshProUGUI InfoText { get; private set; }

        public void Init(CapsuleCollider2D capsuleCollider2D, TextMeshProUGUI infoText)
        {
            CapsuleCollider = capsuleCollider2D;
            InfoText = infoText;
        }


        public void Draw(Vector2 offset, string content)
        {
            DrawBox(lineRenderer, CapsuleCollider, offset);
            InfoText.text = content;


            InfoText.rectTransform.anchoredPosition = new Vector2
            (CapsuleCollider.transform.position.x + offset.x + CapsuleCollider.offset.x + offset.y - CapsuleCollider.size.x * 0.5F,
                CapsuleCollider.transform.position.y + offset.x + CapsuleCollider.offset.y + offset.y + CapsuleCollider.size.y * 0.5F);
        }


        private static void DrawBox(LineRenderer lineRenderer, CapsuleCollider2D collider, Vector2 offset)
        {
            var current = new Vector3(collider.gameObject.transform.position.x + offset.x, collider.gameObject.transform.position.y + offset.y);

            var topLeft = new Vector3(current.x + collider.offset.x - collider.size.x * 0.5F, current.y + collider.offset.y + collider.size.y * 0.5F);
            var topRight = new Vector3(current.x + collider.offset.x + collider.size.x * 0.5f, current.y + collider.offset.y + collider.size.y * 0.5F);

            var bottomLeft = new Vector3(current.x + collider.offset.x - collider.size.x * 0.5F, current.y + collider.offset.y - collider.size.y * 0.5F);
            var bottomRight = new Vector3(current.x + collider.offset.x + collider.size.x * 0.5F, current.y + collider.offset.y - collider.size.y * 0.5F);

            lineRenderer.positionCount = 5;
            lineRenderer.SetPosition(0, topLeft);
            lineRenderer.SetPosition(1, topRight);
            lineRenderer.SetPosition(2, bottomRight);
            lineRenderer.SetPosition(3, bottomLeft);
            lineRenderer.SetPosition(4, topLeft);
        }
    }
}