using UnityEngine;

namespace Core.Utils
{
    public static class SpriteUtils
    {
        public static void FlipSprite(Transform transform, ref bool isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
