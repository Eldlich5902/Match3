using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _default, _pressed;
    //[SerializeField] private AudioClip _compressClip, _uncompressClip;
    //[SerializeField] private AudioSource _source;

    public void Start()
    {
        _img.sprite = _default;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _img.sprite = _pressed;
        // _source.PlayOneShot(_compressClip);
        AudioManager.Instance.PlaySFX("Compressed");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _img.sprite = _default;
        //_source.PlayOneShot(_uncompressClip);
        AudioManager.Instance.PlaySFX("Uncompressed");
    }
}
