using UnityEngine;

namespace Patte_pe_patta.Utility
{
    public static class ResponsiveSpriteRenderer
    {
        /// <summary>
        /// Scales the SpriteRenderer to fill the entire screen area (both width and height),
        /// based on the main orthographic camera's visible world dimensions.
        /// </summary>
        /// <param name="renderer">The SpriteRenderer to scale.</param>
        public static void FitToFullScreen(SpriteRenderer renderer)
        {
            Camera mainCam = Camera.main;

            // Return early if camera or renderer is missing
            if (mainCam == null || renderer == null)
                return;

            // Get visible world height and width based on camera's orthographic size
            float worldHeight = mainCam.orthographicSize * 2f;
            float worldWidth = worldHeight * mainCam.aspect;

            // Scale the sprite proportionally to fit the entire screen area
            renderer.transform.localScale = new Vector3(
                worldWidth / renderer.sprite.bounds.size.x,
                worldHeight / renderer.sprite.bounds.size.y,
                1f
            );
        }

        /// <summary>
        /// Scales the SpriteRenderer so its height matches the safe area height of the screen,
        /// preserving aspect ratio by applying uniform scale.
        /// </summary>
        /// <param name="renderer">The SpriteRenderer to scale.</param>
        public static void FitToSafeAreaHeight(SpriteRenderer renderer)
        {
            // Get the safe area of the screen (area excluding notches, status bars, etc.)
            Rect safeArea = Screen.safeArea;

            // Total screen height in pixels
            float screenHeight = Screen.height;

            // Normalized height of safe area (0 to 1)
            float safeAreaNormalizedHeight = safeArea.height / screenHeight;

            // Total visible vertical world height from the orthographic camera
            float worldHeight = Camera.main.orthographicSize * 2f;

            // Convert normalized safe area height to world units
            float safeWorldHeight = worldHeight * safeAreaNormalizedHeight;

            // Get the sprite's original width in pixels (unaffected by current transform scale)
            float spritePixelsWide = renderer.sprite.rect.width;

            // Convert pixel dimensions to world units using pixels per unit
            float unitsPerPixel = 1f / renderer.sprite.pixelsPerUnit;
            float originalSpriteWidth = spritePixelsWide * unitsPerPixel;

            // Compute scale factor to match sprite's width with the safe area height
            // Uniform scaling is applied to maintain aspect ratio
            Vector3 scale = renderer.transform.localScale;
            scale.x = safeWorldHeight / originalSpriteWidth;
            scale.y = safeWorldHeight / originalSpriteWidth;

            // Apply the calculated scale
            renderer.transform.localScale = scale;
        }
    }
}