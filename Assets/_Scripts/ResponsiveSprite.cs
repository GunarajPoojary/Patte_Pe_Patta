using UnityEngine;

public class ResponsiveSprite : MonoBehaviour
{
    // Reference to the SpriteRenderer you want to scale
    [SerializeField] private SpriteRenderer _renderer;

    // Main camera reference
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;

        // Ensure the camera is orthographic before proceeding
        if (_cam != null && _cam.orthographic)
            FitToSafeAreaWidth();
        else
            Debug.LogWarning("Camera is not set to Orthographic");
    }

    void FitToSafeAreaWidth()
    {
        // Get the safe area of the screen (excludes notches, navigation bars, etc.)
        Rect safeArea = Screen.safeArea;

        // Get the total screen height in pixels
        float screenHeight = Screen.height;

        // Calculate the normalized (0 to 1) height of the safe area relative to the screen
        float safeAreaNormalizedHeight = safeArea.height / screenHeight;

        // Get the total vertical world height visible by the orthographic camera
        float worldHeight = _cam.orthographicSize * 2f;

        // Calculate how tall the safe area is in world units
        float safeWorldHeight = worldHeight * safeAreaNormalizedHeight;

        // Get the original sprite width in pixels (unaffected by scale)
        float spritePixelsWide = _renderer.sprite.rect.width;

        // Convert pixels to world units using the sprite's pixelsPerUnit setting
        float unitsPerPixel = 1f / _renderer.sprite.pixelsPerUnit;
        float originalSpriteWidth = spritePixelsWide * unitsPerPixel;

        // Compute scale factor to match sprite width with the safe area height
        Vector3 scale = _renderer.transform.localScale;
        scale.x = safeWorldHeight / originalSpriteWidth;
        scale.y = safeWorldHeight / originalSpriteWidth; // Uniform scaling to maintain aspect ratio
        _renderer.transform.localScale = scale;
    }
}