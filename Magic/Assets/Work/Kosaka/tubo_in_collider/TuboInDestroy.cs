﻿using UnityEngine;
using System.Collections.Generic;

public class TuboInDestroy : MonoBehaviour
{
    [SerializeField, Tooltip("鍋に入ってから消えるまでの時間(単位：秒)")]
    float DESTROY_TIME = 0.0f;

    [SerializeField, Range(0, 10), Tooltip("ラッシュイベント時の鍋に入っているアプモンの数")]
    int MAX_APPLE_NUM = 5;

    [SerializeField, Range(0, 10), Tooltip("ラッシュイベント時の鍋に入っているレーモンの数")]
    int MAX_LEMON_NUM = 5;

    [SerializeField, Tooltip("鍋に入る最大数")]
    int KUDAMON_MAX_COUNT = 10;

    [SerializeField
   , TooltipAttribute("ここに「AttackDrian」prefabを入れてください\n(プログラマー用)")]
    GameObject drian_attack_obj_ = null;

    //それぞれのくだモンの鍋に入った(消した)数
    int lemon_count_ = 0;
    int apumon_count_ = 0;
    bool is_in_momon_ = false;

    public bool is_in_dorian_ = false;

    //くだモンの名前
    const string LEMON_NAME = "le-mon";
    const string APUMON_NAME = "apumon";
    const string MOMON_NAME = "momon";

    const string JAMAMON_NAME = "jamamon";
    const string DORIANBOM_NAME = "dorianbomb_red";

    const string GOLD_APPLE_NAME = "apumon_gold";
    const string SILVER_LEMON_NAME = "re-mon_silver";

    RushEventer rush_eventer_ = null;

    PlayerAttacker player_attacker_ = null;
    GameStartDirector game_start_director_ = null;
    GameEndDirector game_end_director_ = null;

    LidControl lid_control_ = null;

    [SerializeField]
    GameObject smoke_prehub_ = null;

    public bool IsInLemon_ { get; set; }
    public bool IsInApple_ { get; set; }
    public bool IsInPeach_ { get; set; }
    public bool IsInJamamon_ { get; set; }

    //-----------------------------------------------------------------

    //それぞれのくだモンの鍋に入った(消した)数のゲッター
    public int GetLemonCount() { return lemon_count_; }
    public int GetApumonCount() { return apumon_count_; }
    public bool GetMomonCount() { return is_in_momon_; }
    public int GetKudamonCount()
    {
        var kudamon_add = lemon_count_ + apumon_count_;
        return kudamon_add;
    }
    public bool IsInDorain { get { return is_in_dorian_; } }

    //---------------------------------------------------------------------

    void Awake()
    {
        rush_eventer_ = FindObjectOfType<RushEventer>();
        lid_control_ = FindObjectOfType<LidControl>();

        IsInLemon_ = false;
        IsInApple_ = false;
        IsInPeach_ = false;
        IsInJamamon_ = false;
    }

    void Update()
    {
        FindPlayer();
        if (!MyNetworkLobbyManager.s_singleton.IsTutorial)
        {
            if (game_start_director_ == null) return;
        }
        //----------------------------------------------

        //ゲームが始まったら蓋をはずす

        lid_control_.can_rendering_lid_ = false;

        if (!MyNetworkLobbyManager.s_singleton.IsTutorial)
        {
            if (game_start_director_.IsReady || game_start_director_.IsConnect)
                lid_control_.can_rendering_lid_ = true;
        }



        //くだモンがMAXなら蓋をつける
        if (GetKudamonCount() >= KUDAMON_MAX_COUNT)
            lid_control_.can_rendering_lid_ = true;

        if (!MyNetworkLobbyManager.s_singleton.IsTutorial)
        {
            if (game_end_director_.IsStart)
                lid_control_.can_rendering_lid_ = true;
        }
        //----------------------------------------------
        RushEvent();
    }

    //鍋の中のTrigger判定
    void OnTriggerEnter(Collider other)
    {
        //それぞれのくだモンを「消す処理」と「カウント処理」（と「入ったものを出力」するためのデバッグ）
        if (other.name == LEMON_NAME)
        {
            Destroy(other.gameObject);
            if (GetKudamonCount() >= KUDAMON_MAX_COUNT) return;
            lemon_count_++;

            var smoke_ = Instantiate(smoke_prehub_).GetComponent<Transform>();
            smoke_.parent = GameObject.Find("Pot").transform;

            IsInLemon_ = true;

            AudioManager.Instance.PlaySe(2);
        }
        else if (other.name == APUMON_NAME)
        {
            Destroy(other.gameObject);
            if (GetKudamonCount() >= KUDAMON_MAX_COUNT) return;
            apumon_count_++;

            var smoke_ = Instantiate(smoke_prehub_).GetComponent<Transform>();
            smoke_.parent = GameObject.Find("Pot").transform;

            IsInApple_ = true;

            AudioManager.Instance.PlaySe(2);
        }
        else if (other.name == MOMON_NAME)
        {
            Destroy(other.gameObject);
            is_in_momon_ = true;

            var smoke_ = Instantiate(smoke_prehub_).GetComponent<Transform>();
            smoke_.parent = GameObject.Find("Pot").transform;

            IsInPeach_ = true;

            AudioManager.Instance.PlaySe(3);
        }
        else if (other.name == JAMAMON_NAME)
        {
            Destroy(other.gameObject);
            JamamonFlyOut();

            var smoke_ = Instantiate(smoke_prehub_).GetComponent<Transform>();
            smoke_.parent = GameObject.Find("Pot").transform;

            IsInJamamon_ = true;

            AudioManager.Instance.PlaySe(3);
        }
        else if (other.name == DORIANBOM_NAME)
        {
            if (other.gameObject.GetComponent<Ike3dorian>().IsExplosion) return;
            Destroy(other.gameObject);
            is_in_dorian_ = true;

            AudioManager.Instance.PlaySe(8);

            if (drian_attack_obj_ != null)
            {
                GameObject game_object = Instantiate(drian_attack_obj_);
                game_object.transform.position = transform.position;
                game_object.name = drian_attack_obj_.name;
            }
            else
            {
                Debug.Log("drian_attack_obj_ にプレハブが入っていません");
            }

            var smoke_ = Instantiate(smoke_prehub_).GetComponent<Transform>();
            smoke_.parent = GameObject.Find("Pot").transform;
        }
        else if (other.name == GOLD_APPLE_NAME)
        {
            var obj = other.GetComponent<SpecialEvent>();
            obj.ChangeFruits();
            Destroy(other.gameObject);
        }
        else if (other.name == SILVER_LEMON_NAME)
        {
            var obj = other.GetComponent<SpecialEvent>();
            obj.ChangeFruits();
            Destroy(other.gameObject);
        }

    }

    //---------------------------------------------------------------------

    void RushEvent()
    {
        if (!rush_eventer_.IsStart) return;
        lemon_count_ = MAX_LEMON_NUM;
        apumon_count_ = MAX_APPLE_NUM;
        lid_control_.can_rendering_lid_ = false;
    }

    public void ResetCount()
    {
        lemon_count_ = 0;
        apumon_count_ = 0;
    }

    public void ResetMomon()
    {
        is_in_momon_ = false;
    }

    public void ResetDorian()
    {
        is_in_dorian_ = false;
    }

    void JamamonFlyOut()
    {
        var kudamon_manager = GameObject.Find("FruitManager")
            .GetComponent<FruitCreater>();

        for (var apple_num = 0; apple_num < apumon_count_; apple_num++)
        {
            var apple = kudamon_manager.AppleCreate();
            apple.transform.position = transform.position + new Vector3(0.0f, 2.0f, 0.0f);
            apple.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10.0f, 10.0f), 30.0f, Random.Range(-10.0f, 10.0f));
        }

        for (var lemon_num = 0; lemon_num < lemon_count_; lemon_num++)
        {
            var lemon = kudamon_manager.LemonCreate();
            lemon.transform.position = transform.position + new Vector3(0.0f, 2.0f, 0.0f);
            lemon.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10.0f, 10.0f), 30.0f, Random.Range(-10.0f, 10.0f));
        }
        lemon_count_ = 0;
        apumon_count_ = 0;
    }

    void FindPlayer()
    {
        if (game_start_director_ != null) return;
        foreach (var player in FindObjectsOfType<PlayerAttacker>())
        {
            if (!MyNetworkLobbyManager.s_singleton.IsTutorial)
            {
                if (!player.isLocalPlayer) continue;
            }
            player_attacker_ = player;
            game_start_director_ = player.gameObject.GetComponent<GameStartDirector>();
            game_end_director_ = player.gameObject.GetComponent<GameEndDirector>();
        }
    }
}