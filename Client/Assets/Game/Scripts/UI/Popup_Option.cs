using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class OptionInfo {
    public bool First = true;
    public bool Bgm = true;
    public bool EfxFx = true;
    public bool 탄창위치Right = true;
    public bool 광고제거 = false;
    public int PlayCount;

    public const int MaxPlayCount = 10;
    public bool PolicyAgreement;
    public void SetBgm()
    {
        Bgm = !Bgm;
        SoundManager.Instance.IsBgmOn = Bgm;
    }
    public string GetBgmSpriteName()
    {
        return Bgm ? "buttons[ui-small-buttons_88]" : "buttons[ui-small-buttons_128]";
    }

    public void SetEfxFx()
    {
        EfxFx = !EfxFx;
        SoundManager.Instance.IsEfxOn = EfxFx;
    }

    public string GetEfxFxSpriteName()
    {
        return EfxFx ? "buttons[ui-small-buttons_78]" : "buttons[ui-small-buttons_128]";
    }
}

public class Popup_Option : MonoBehaviour {
    public LobbyUI LobbyUI;
    public Image ImageBgm;
    public Image ImageEfx;

    public void Init(LobbyUI lobbyUI)
    {
        LobbyUI = lobbyUI;
    }

    public void OnEnable()
    {
        SoundManager.Instance.IsBgmOn = GameManager.Inst.OptionInfo.Bgm;
        ChangeSprite(ImageBgm, GameManager.Inst.OptionInfo.GetBgmSpriteName());

        SoundManager.Instance.IsEfxOn = GameManager.Inst.OptionInfo.EfxFx;
        ChangeSprite(ImageEfx, GameManager.Inst.OptionInfo.GetEfxFxSpriteName());
    }


    public void SetBgm()
    {
        GameManager.Inst.OptionInfo.SetBgm();
        ChangeSprite(ImageBgm, GameManager.Inst.OptionInfo.GetBgmSpriteName());
    }

    public void SetEfxFx()
    {
        GameManager.Inst.OptionInfo.SetEfxFx();
        ChangeSprite(ImageEfx, GameManager.Inst.OptionInfo.GetEfxFxSpriteName());
    }

    public void Close()
    {
        if (LobbyUI != null) {
            LobbyUI.ShowMenu();
        }
    }

    public void ChangeSprite(Image image, string spriteName)
    {
        ResourceManager.Load<Sprite>(spriteName, (sprite) => {
            image.sprite = sprite;
        });
    }
}
