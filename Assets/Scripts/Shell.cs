using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shell : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Material _material;

    [SerializeField] private float _minForce;
    [SerializeField] private float _maxForce;
    [SerializeField] private float _fadeTime = 2;
    [SerializeField] private float _lifetime = 4;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _material = GetComponent<Renderer>().material;
    }

    private void Start()
    {
        float force = Random.Range(_minForce, _maxForce);
        _rigidbody.AddForce(transform.right * force);
        _rigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(_lifetime);

        float percent = 0;
        float fadeSpeed = 1 / _fadeTime;
        Color initialColor = _material.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            _material.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
