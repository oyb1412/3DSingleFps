using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class SceneManagerEX
{
    public SceneType CurrentScene = SceneType.Startup;
    public BaseScene CurrentSceneManager => GameObject.FindObjectOfType<BaseScene>();

    private UI_Fade _fade;

    public void Init() {
        if(_fade == null)
            _fade = GameObject.Find(NAME_UI_FAED).GetComponent<UI_Fade>();
    }

    public void LoadScene(SceneType type) {
        var tween = _fade.SetFade(true);
        if (type == SceneType.Exit) {
            tween.OnComplete(QuitGame);
        } else {
            if(type == SceneType.Startup) {
                tween.OnComplete(() => ClearLoadScene(type));
            } else {
                tween.OnComplete(() => DoNextScene(type));
            }
        }
    }

    private void ClearLoadScene(SceneType type) {
        Managers.Instance.Ingameclear();
        DoNextScene(type);
    }

    public void SetScene() {
        _fade.SetFade(false);
    }

    private void QuitGame() {
        Util.QuitGame();
    }

    private void DoNextScene(Define.SceneType type) {
        SceneManager.LoadScene(type.ToString());
        CurrentScene = type;
    }

}
