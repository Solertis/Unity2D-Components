﻿using UnityEngine;
using System.Collections;
using Matcha.Dreadful.Colors;
using Matcha.Dreadful.FX;
using DG.Tweening;

public class PlayerManager : CacheBehaviour
{
    private _PlayerData player;

    void Start()
    {
        player = GameObject.Find(_PLAYER_DATA).GetComponent<_PlayerData>();

        Init();
    }

    void Init()
    {
        Messenger.Broadcast<int>("init lvl", player.LVL);
        Messenger.Broadcast<int>("init hp", player.HP);
        Messenger.Broadcast<int>("init ac", player.AC);
        Messenger.Broadcast<int>("init xp", player.XP);
        Messenger.Broadcast<GameObject, GameObject, GameObject>
            ("init weapons", player.equippedWeapon, player.leftWeapon, player.rightWeapon);
        Messenger.Broadcast<Transform>("player placed", transform);
    }

    public void TakesHit(string weaponType, Weapon weapon, Collider2D coll, int hitFrom)
    {
        // calculate damage
        player.HP -= (int)(weapon.damage * DIFFICULTY_DAMAGE_MODIFIER);

        // produce effects
        // params for ShakeCamera = duration, strength, vibrato, randomness
        Messenger.Broadcast<float, float, int, float>("shake camera", .5f, .3f, 20, 5f);
        Messenger.Broadcast<int>("reduce hp", player.HP);

        // float xDistance = hitFrom == LEFT ? 2f : -2f;
        // transform.DOJump(new Vector3(transform.position.x + xDistance, transform.position.y, transform.position.z), .2f, 1, .5f, false);

        if (hitFrom == RIGHT)
        {
            BroadcastMessage("RepulseToLeft", 5.0F);
        }
        else
        {
            BroadcastMessage("RepulseToRight", 5.0F);
        }

        if (player.HP > 0)
        {
            MFX.FadeToColorAndBack(spriteRenderer, MCLR.bloodRed, 0f, .2f);
        }
        else
        {
            Messenger.Broadcast<string, Collider2D, int>("player dead", "projectile", coll, hitFrom);
        }
    }

    public void TouchesEnemy(string weaponType, CreatureEntity enemy, Collider2D coll, int hitFrom)
    {
        // calculate damage
        player.HP -= (int)(enemy.touchDamage * DIFFICULTY_DAMAGE_MODIFIER);

        // produce effects
        Messenger.Broadcast<int>("reduce hp", player.HP);

        if (player.HP > 0)
        {
            MFX.FadeToColorAndBack(spriteRenderer, MCLR.bloodRed, 0f, .2f);
        }
        else
        {
            Messenger.Broadcast<string, Collider2D, int>("player dead", "struckdown", coll, hitFrom);
        }
    }

    void OnPlayerHit(string weaponType, Collider2D coll, int hitFrom)
    {

        // Messenger.Broadcast<string, Collider2D, int>("player dead", "projectile", coll, hitFrom);
    }

    void OnPrizeCollected(int worth)
    {

    }

    void OnLevelCompleted(bool status)
    {

    }

    void OnEnable()
    {
        Messenger.AddListener<string, Collider2D, int>("player hit", OnPlayerHit);
        Messenger.AddListener<int>( "prize collected", OnPrizeCollected);
        Messenger.AddListener<bool>( "level completed", OnLevelCompleted);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener<string, Collider2D, int>("player hit", OnPlayerHit);
        Messenger.RemoveListener<int>( "prize collected", OnPrizeCollected );
        Messenger.RemoveListener<bool>( "level completed", OnLevelCompleted);
    }
}