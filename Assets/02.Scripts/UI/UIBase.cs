using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIBase : MonoBehaviour
{
    protected void BindEvent(GameObject go, Action<PointerEventData> action, Define.MouseEventType type)
    {
        UI_EventHandler evt = Util.GetorAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.MouseEventType.Enter:
                evt.OnEnterHandler += action;
                break;
            case Define.MouseEventType.LeftMouseDown:
                evt.OnClickHandler += action;
                break;
        }
    }
}
