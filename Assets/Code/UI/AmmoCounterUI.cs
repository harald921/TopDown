using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class AmmoCounterUI : MonoBehaviour
{
    [SerializeField] Text _text;
    [SerializeField] Image _reloadFill;

    CoroutineHandle _reloadAnimationHandle;

    Player _player;

    public void Initialize(Player inPlayer)
    {
        _player = inPlayer;

        _player.weaponComponent.OnWeaponFire     += UpdateCurrentAmmo;
        _player.weaponComponent.OnWeaponDropped  += () => OnWeaponDropped();
        _player.weaponComponent.OnWeaponPickedUp += () => OnWeaponPickedUp(_player.weaponComponent.heldWeapon);
    }

    void UpdateCurrentAmmo()
    {

    }

    IEnumerator<float> _HandleReloadAnimation()
    {
        float reloadTime = _player.weaponComponent.heldWeapon.stats.reloadTime;
        float timer = 0;
        while (timer < reloadTime)
        {
            // Progress the reload animation bar fill
            float progress = Mathf.InverseLerp(0, reloadTime, timer);
            _reloadFill.fillAmount = progress;

            timer += Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }

        _reloadFill.fillAmount = 0;
    }

    void StopAndHideReloadAnimation()
    {
        Timing.KillCoroutines(_reloadAnimationHandle);
        _reloadFill.fillAmount = 0;
    }

    void OnWeaponDropped()
    {
        StopAndHideReloadAnimation();
        gameObject.SetActive(false);
    }

    void OnWeaponPickedUp(Weapon inWeapon)
    {
        Debug.Log("TODO: Add UpdateUIText() here");
    }

    void UpdateUIText(string inWeaponName, int inCurrentAmmo, int inMaxAmmo)
    {
        _text.text = inWeaponName + ": " + inCurrentAmmo + " | " + inMaxAmmo;
    }
}
