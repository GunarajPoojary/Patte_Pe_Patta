using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bgRenderer;
    private Camera _mainCam;

    private void Awake()
    {
        Debug.Log(Screen.safeArea);

        _mainCam = Camera.main;

        // Get world height and width based on orthographic size
        float worldHeight = _mainCam.orthographicSize * 2f;
        float worldWidth = worldHeight * _mainCam.aspect;

        Debug.Log($"World Width: {worldWidth}, World Height: {worldHeight}");

        // Resize background sprite to fit
        if (_bgRenderer != null)
        {
            _bgRenderer.transform.localScale = new Vector3(worldWidth / _bgRenderer.sprite.bounds.size.x, worldHeight / _bgRenderer.sprite.bounds.size.y, 1f);
        }
    }
}