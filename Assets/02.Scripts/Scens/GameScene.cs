using System.Collections;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public override void Clear()
    {
    }

    protected override void Start() {
        base.Start();

        StartCoroutine(Co_GameStart());
       
    }

    private IEnumerator Co_GameStart() {
        PlayerController player = GameObject.Find(NAME_PLAYER).GetComponent<PlayerController>();
        
        yield return new WaitUntil(() => player != null);

        GameObject ui = Managers.Resources.Instantiate(UI_PATH, null);

        int count = 0;

        foreach (Transform t in ui.transform) {
            t.GetComponent<UI_Base>().SetPlayer(player);
            count++;
        }

        yield return new WaitUntil(() => count >= ui.transform.childCount);

        Managers.GameManager.SetPlayer();
        Managers.Scene.SetScene();
        Managers.Instance.IngameInit();

        BgmController.instance.SetBgm(Bgm.Ingame, true);
    }
}
