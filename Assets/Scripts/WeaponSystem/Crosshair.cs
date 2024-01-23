using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private Canvas _canvas;
    private RectTransform _canvasRect;
    Vector3 _mouseInput;
    Vector3 _normalizedPosition;

    private void Start()
    {
        GetReferences();
    }

    void Update()
    {
        _mouseInput = Input.mousePosition;
        _normalizedPosition = new Vector3
            (
               Mathf.Clamp(((_mouseInput.x* _canvasRect.rect.width + _canvasRect.rect.width / 2) / _canvasRect.rect.width),0 , _canvasRect.rect.width),
               Mathf.Clamp(((_mouseInput.y* _canvasRect.rect.height + _canvasRect.rect.height / 2) / _canvasRect.rect.height),0, _canvasRect.rect.height)
            );
    }

    private void FixedUpdate()
    {
        transform.position = _normalizedPosition;
    }

    private void GetReferences()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasRect = _canvas.GetComponent<RectTransform>();
        _canvas.GetComponent<GameUI>()._playerController.OnDeath += OnGameOver;
    }

    private void OnGameOver()
    {
        Destroy(gameObject);
    }
}