using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static Define;

public class UI_Fade : MonoBehaviour {
    public static UI_Fade Instance;
    private Image _fadeImage;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if( Instance != this) {
            Destroy(gameObject);
        }
        _fadeImage = GetComponentInChildren<Image>();
        _fadeImage.color = Color.black;

    }

    public Tween SetFade(bool trigger) {
        if (trigger) {
            return _fadeImage.DOFade(1f, FADE_TIME);
        } else
            return _fadeImage.DOFade(0, FADE_TIME);
    }
}
