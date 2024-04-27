using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Fade : MonoBehaviour {
    public static UI_Fade Instance;
    private const float FADE_TIME = 1f;
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

    /// <summary>
    /// trigger판정에 맞춰 페이드 실행
    /// </summary>
    public Tween SetFade(bool trigger) {
        if (trigger) {
            return _fadeImage.DOFade(1f, FADE_TIME);
        } else
            return _fadeImage.DOFade(0, FADE_TIME);
    }
}
