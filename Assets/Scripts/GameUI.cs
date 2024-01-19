using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PlayerController _playerController;
    [SerializeField] private Image _fadeScreen;
    [SerializeField] GameObject _gameOverUI;
    [SerializeField] Crosshair _crosshair;

    void Start()
    {
        GetReferences();
        if (_crosshair != null)
        {
            var crosshair = Instantiate<Crosshair>(_crosshair, transform);
        }
    }

    private void OnGameOver()
    {
        _gameOverUI.SetActive(true);
        StartCoroutine(Fade(Color.clear, Color.black, 1f));
        
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            _fadeScreen.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    private void GetReferences()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _playerController.OnDeath += OnGameOver;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(0);
    }
}
