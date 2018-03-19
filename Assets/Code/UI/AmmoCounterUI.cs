using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class AmmoCounterUI : MonoBehaviour
{
    // [SerializeField] TMPro.TextMeshProUGUI _text;
    // [SerializeField] Image _reloadProgressBar;
    // 
    // CoroutineHandle _reloadAnimationHandle;
    // 
    // PlayerWeaponComponent _weaponComponent;
    // 

    public void Initialize(PlayerWeaponComponent inWeaponComponent)
    {
        gameObject.SetActive(false);
    //    _weaponComponent = inWeaponComponent;
    //
    //    if (_weaponComponent.heldWeapon)
    //        UpdateText();
    //    else
    //        Hide();
    //
    //    StopAndHideReloadAnimation();
    //
    //    SubscribeEvents();
    }
    
    // 
    // void SubscribeEvents()
    // {
    //     _weaponComponent.OnWeaponPickedUp += (Weapon inPickedUpWeapon) => 
    //     {
    //         inPickedUpWeapon.fireComponent.OnFire         += UpdateText;
    //         inPickedUpWeapon.ammoComponent.OnReloadStart  += RunReloadAnimation;
    //         inPickedUpWeapon.ammoComponent.OnReloadStop   += UpdateText;
    // 
    //         Show();
    //     };
    // 
    //     _weaponComponent.OnWeaponDropped += (Weapon inDroppedWeapon) => Hide();
    // }
    // 
    // void RunReloadAnimation()
    // {
    //     _reloadAnimationHandle = Timing.RunCoroutine(_HandleReloadAnimation());
    // }
    // 
    // IEnumerator<float> _HandleReloadAnimation()
    // {
    //     float reloadTime = _weaponComponent.heldWeapon.stats.reloadTime;
    // 
    //     float timer = 0;
    //     while (timer < reloadTime)
    //     {
    //         // Progress the reload animation bar fill
    //         float progress = Mathf.InverseLerp(0, reloadTime, timer);
    // 
    //         _reloadProgressBar.fillAmount = progress;
    // 
    //         timer += Time.deltaTime;
    //         yield return Timing.WaitForOneFrame;
    //     }
    // }
    // 
    // void Hide()
    // {
    //     StopAndHideReloadAnimation();
    //     gameObject.SetActive(false);
    // }
    // 
    // void Show()
    // {
    //     gameObject.SetActive(true);
    //     UpdateText();
    // }
    // 
    // void StopAndHideReloadAnimation()
    // {
    //     Timing.KillCoroutines(_reloadAnimationHandle);
    //     _reloadProgressBar.fillAmount = 0;
    // }
    // 
    // void UpdateText()
    // {
    //     Weapon heldWeapon = _weaponComponent.heldWeapon;
    // 
    //     string weaponName  = heldWeapon.stats.name;
    //     int    currentAmmo = heldWeapon.currentAmmo;
    //     int    maxAmmo     = heldWeapon.stats.maxAmmo;
    // 
    //     _text.text = weaponName + ": " + currentAmmo + " | " + maxAmmo;
    // 
    //     _reloadProgressBar.fillAmount = (float)currentAmmo / (float)maxAmmo;
    // }
}
