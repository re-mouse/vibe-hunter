﻿using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer background;
    [SerializeField]
    private Sprite normalEntitySprite;
    [SerializeField]
    private Sprite bossEntitySprite;
    [SerializeField]
    private Text bossKillTimer;
    [SerializeField]
    private GameObject battleDamagePrefab;
    [SerializeField]
    private RectTransform battleDamageSpawnTransform;
    [SerializeField]
    private Text gold;
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Text health;

    private Coroutine bossKillTimerCoroutine;

    private void Awake()
    {
        EntitySpawner.OnEntitySpawn.AddListener(UpdateEntityHealthBar);

        EntitySpawner.OnEntityTakeDamage.AddListener(x => UpdateEntityHealthBar());
        EntitySpawner.OnEntityTakeDamage.AddListener(ShowDamageText);
#if UNITY_ANDROID
        EntitySpawner.OnEntityTakeDamage.AddListener(x => MMVibrationManager.Haptic(HapticTypes.SoftImpact));
#endif
#if UNITY_IOS
        EntitySpawner.OnEntityTakeDamage.AddListener(x => MMVibrationManager.Haptic(HapticTypes.RigidImpact));
#endif

        EntitySpawner.OnBossSpawn.AddListener(() => bossKillTimerCoroutine = StartCoroutine(ShowBossKillTimer()));
        EntitySpawner.OnBossDeath.AddListener(() => StopCoroutine(bossKillTimerCoroutine));
        EntitySpawner.OnBossDeath.AddListener(() => bossKillTimer.text = "");
        EntitySpawner.OnBossDeath.AddListener(() => background.sprite = normalEntitySprite);

        Player.OnPlayerInfoUpdate.AddListener(UpdateGold);
    }

    private IEnumerator ShowBossKillTimer()
    {
        background.sprite = bossEntitySprite;
        float timer = EntitySpawner.timeToKillBoss;
        while (timer > 0)
        {
            timer -= 0.1f;
            bossKillTimer.text = timer.ToString("0.0");
            yield return new WaitForSeconds(.1f);
        }
        background.sprite = normalEntitySprite;
        bossKillTimer.text = "";
    }

    private void UpdateEntityHealthBar()
    {
        if (EntitySpawner.CurrentEntity == null)
        {
            healthBar.value = 0;
            health.text = "0/0";
            return;
        }

        healthBar.value = 1.0f * EntitySpawner.CurrentEntity.GetHealth() / EntitySpawner.CurrentEntity.GetMaxHealth();
        health.text = $"{PlayerInfo.GetAdaptedInt(EntitySpawner.CurrentEntity.GetHealth())}/{PlayerInfo.GetAdaptedInt(EntitySpawner.CurrentEntity.GetMaxHealth())}";
    }

    private void ShowDamageText(ulong damage)
    {
        if (damage == 0)
            return;
        var damageTextObject = Instantiate(battleDamagePrefab, battleDamageSpawnTransform);
        var pos = damageTextObject.GetComponent<RectTransform>().anchoredPosition;
        pos.x += Random.Range(-160, 160);
        pos.y += Random.Range(-5, 150);
        damageTextObject.GetComponent<RectTransform>().anchoredPosition = pos;
        damageTextObject.GetComponent<Text>().text = $"-{PlayerInfo.GetAdaptedInt(damage)} HP";
    }

    private void UpdateGold()
    {
        gold.text = PlayerInfo.GetAdaptedInt(Player.GetPlayerInfo().gold);
    }
}
