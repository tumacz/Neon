using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private GameObject _flashHolder;
    [SerializeField] private float _flashTime;
    [SerializeField] private Sprite[] _flashSprites;
    [SerializeField] private SpriteRenderer[] _spriteRenderers;

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        _flashHolder.SetActive(true);
        int flashSpriteIndex = UnityEngine.Random.Range(0, _flashSprites.Length);
        for (int i = 0; i< _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].sprite = _flashSprites[flashSpriteIndex];
        }
        Invoke("Deactivate", _flashTime);
    }

    private void Deactivate()
    {
        _flashHolder.SetActive(false);
    }
}
