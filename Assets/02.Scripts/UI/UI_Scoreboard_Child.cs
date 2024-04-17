using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Scoreboard_Child : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _killText;
    [SerializeField] private TextMeshProUGUI _deathText;
    [SerializeField] private Image _bgImage;
    public UnitBase UnitBase { get; private set; }
    
    public void Init(string name, UnitBase unit, Color color) {
        _nameText.text = name;
        _rankText.text = $"#{0:D3}";
        _killText.text = $"{0:D2}";
        _deathText.text = $"{0:D2}";

        UnitBase = unit;
        unit.KillNumberEvent += ((kill) => _killText.text = $"{kill:D2}");
        unit.DeathNumberEvent += ((death) => _deathText.text = $"{death:D2}");
        _bgImage.color = color;
    }
}
