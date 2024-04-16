using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManage : MonoBehaviour
{
    
    //계수
    /// <summary> 공격 성공 확률 감소 계수 (곱연산) </summary>
    double attackPercentChange = 0.91;

    //기본 스탯
    /// <summary> 플레이어 체력 </summary>
    int life = 3;
    /// <summary> 현재 소지한 경험치 </summary>
    int exp = 0;
    /// <summary> 누적 획득한 경험치 </summary>
    int totalexp = 0;

    /// <summary> 보스 체력 </summary>
    int bossLife = 0;
  
    /// <summary> 공격력 </summary>
    int attackDamage = 0;
    /// <summary> 공격 성공 확률 </summary>
    double attackPercent = 0;
    /// <summary> 공격 성공 확률을 int형으로 변환한 값 </summary>
    int iattackPercent = 0;


    //점수
    /// <summary> 공격 성공 횟수 </summary>
    int attackCount = 0;
    /// <summary> 점수
    /// [계산식]
    /// +공격력 만큼 (매 공격 성공마다)
    /// +소지 경험치 만큼 (매 공격 성공마다)
    /// +3000 (게임 클리어시)
    /// </summary>
    int score = 0;


    //강화 관련
    /// <summary> 공격력 강화에 소모하는 경험치 비용 (합연산) </summary>
    int attackDamageUpgradeCost = 0;
    /// <summary> 공격력 강화시 증가하는 공격력 (합연산) </summary>
    int attackDamageUpgradeValue = 0;
    /// <summary> 현재 공격력 강화 레벨 </summary>
    int attackDamageLevel = 0;

    /// <summary> 정확도 강화에 소모하는 경험치 비용 (합연산) </summary>
    int attackPercentUpgradeCost = 0;
    /// <summary> 정확도 강화시 증가하는 공격 성공 확률 (합연산) </summary>
    int attackPercentUpgradeValue = 0;
    /// <summary> 현재 정확도 강화 레벨 </summary>
    int attackPercentLevel = 0;
    /// <summary> 플레이어 체력이 1 이상일때 true </summary>
    bool isPlayerAlive = true;

    public GameObject ScoreBoard;
    public RawImage LifeFirst, LifeSecond, LifeThird;
    public Text ExpText, AttackCountText, BossLifeText, AttackButtonText,  UpgradePercentButtonText, UpgradeDamageButtonText, ScoreBoardText;    //화면에 보이는 UI를 수정할 수 있게 연결함

    static DateTime nowTime = DateTime.Now;
    string filepath = "";
    string scoreboardsay = "";

    /// <summary>
    /// 게임을 초기화 하는 함수
    /// </summary>
    void GameReset()
    {
        
        life = 3;
        bossLife = 300;
        exp = 0;
        totalexp = 0;
        attackDamage = 1;
        attackPercent = 100;

        attackDamageLevel = 1;
        attackDamageUpgradeValue = 1;
        attackDamageUpgradeCost = 20;

        attackPercentLevel = 1;
        attackPercentUpgradeValue = 5;
        attackPercentUpgradeCost = 15;

        attackCount = 0;

        ScoreBoard.SetActive(false);
        scoreboardsay = "ERROR";
        score = 0;
    }

    void GameClear()
    {
        score = score + 3000;
        scoreboardsay = "★☆GAME CLEAR☆★";
        ScoreBoard.SetActive(true);
    }
    void GameOver()
    {
        scoreboardsay = "GAME OVER";
        ScoreBoard.SetActive(true);
    }


    // 추가된 함수: 로그를 남기는 함수
    void Logger(string btn, string act)
    {
        nowTime = DateTime.Now;
        using (StreamWriter sw = new StreamWriter(filepath, true, System.Text.Encoding.GetEncoding("utf-8")))
        {
            sw.WriteLine($"{nowTime.ToString("yyyy/MM/dd HH:mm:ss:ff")},{btn},{act},{life},{bossLife},{score},{exp},{totalexp},{attackDamage},{attackPercent},{attackDamageLevel},{attackPercentLevel}");
        }
    }
    
    /// <summary>
    /// 공격 버튼을 클릭 시 실행되는 함수
    /// </summary>
    public void OnClickAttack()
    {
        if (!isPlayerAlive)  //다시하기
        {
            Logger("공격", "다시하기");
            GameReset();
        }
        else if (UnityEngine.Random.Range(0, 100) <= iattackPercent)    //공격 성공
        {
            Logger("공격", "성공");
            bossLife -= attackDamage;
            score = attackDamage + exp; 
            attackPercent *= attackPercentChange;
            exp += 30;
            totalexp += 30;
            attackCount++;
            
        }
        else    //공격 실패
        {
            Logger("공격", "실패");
            life--;
            attackPercent = 100;
            if(life <= 0)
            {
                GameOver();
            }
        }
        if(bossLife <= 0)
        {
            Logger("공격", "게임 클리어");
            GameClear();
        }
    }

    /// <summary>
    /// 공격력 강화 버튼을 클릭 시 실행되는 함수
    /// </summary>
    public void OnClickUpgradeDamage()
    {
        if (exp >= attackDamageUpgradeCost) //경험치 충분
        {
            Logger("공격력 강화", "성공");
            attackDamage += attackDamageUpgradeValue;
            exp -= attackDamageUpgradeCost;
            attackDamageLevel++;

            attackDamageUpgradeCost = attackDamageUpgradeCost + 5 * attackDamageLevel;
            attackDamageUpgradeValue = 4 + attackDamageLevel;
        }
        else
        {
            Logger("공격력 강화", "실패");
        }
    }

    /// <summary>
    /// 정확도 강화 버튼을 클릭 시 실행되는 함수
    /// </summary>
    public void OnClickUpgradePercent()
    {
        if(exp >= attackPercentUpgradeCost) //경험치 충분
        {
            Logger("정확도 강화", "성공");
            attackPercent += attackPercentUpgradeValue;
            exp -= attackPercentUpgradeCost;
            attackPercentLevel++;

            attackPercentUpgradeCost = attackPercentUpgradeCost + 4 * attackPercentLevel;
            attackPercentUpgradeValue = 4 + attackPercentLevel;
        }
        else
        {
            Logger("정확도 강화", "실패");
        }
    }

    /// <summary>
    /// 게임 종료 버튼을 클릭 시 실행되는 함수
    /// </summary>
    public void OnClickExit()
    {
        Logger("종료", "게임 종료");
        Debug.Log("Exit");
        Application.Quit();
    }


    public void Start()
    {
        filepath = "PlayLog\\Log_" + nowTime.ToString("yyyy_MM_dd_HH_mm_ss_ff") + ".csv";
        nowTime = DateTime.Now;
        using (StreamWriter sw = new StreamWriter(filepath, true, System.Text.Encoding.GetEncoding("utf-8")))
        {
            sw.WriteLine("시간,버튼,행동,플레이어 체력,보스 체력,점수,소유 경험치,총 경험치,공격력,공격 성공 확률,공격력 강화 레벨,정확도 강화 레벨");
        }
        Logger("게임 시작", "게임 시작");
        GameReset();
    }


    void Update()
    {
        
        switch (life)
        {
            case 0:
                LifeThird.enabled = false; // 세 번째 하트 이미지 비활성화
                break;
            case 1:
                LifeThird.enabled = true; // 세 번째 하트 이미지 활성화
                LifeSecond.enabled = false; // 두 번째 하트 이미지 비활성화
                break;
            case 2:
                LifeThird.enabled = true; // 세 번째 하트 이미지 활성화
                LifeSecond.enabled = true; // 두 번째 하트 이미지 활성화
                LifeFirst.enabled = false; // 첫 번째 하트 이미지 비활성화
                break;
            case 3:
                LifeThird.enabled = true; // 세 번째 하트 이미지 활성화
                LifeSecond.enabled = true; // 두 번째 하트 이미지 활성화
                LifeFirst.enabled = true; // 첫 번째 하트 이미지 활성화
                break;
        }

        


        //double 공격 확률을 int로 변환해 저장
        iattackPercent = Convert.ToInt32(attackPercent);

        //UI에 나타낼 텍스트
        ExpText.text = "경험치: " + exp.ToString();
        BossLifeText.text = bossLife.ToString() + " / 300";
        AttackCountText.text = "공격성공횟수:" + attackCount.ToString() + "번";

        if(life <=0)
        {
            isPlayerAlive = false;
            AttackButtonText.text = "다시 시작";
        }
        else
        {
            isPlayerAlive = true;
            AttackButtonText.text = "☆공격☆\n성공확률" + iattackPercent.ToString() + "%\n공격력" + attackDamage.ToString();
        }
        

        UpgradeDamageButtonText.text = "공격력 강화!\n" + attackDamageUpgradeCost.ToString() + "exp\nLV." + attackDamageLevel.ToString();
        UpgradePercentButtonText.text = "정확도 강화!\n" + attackPercentUpgradeCost.ToString() + "exp\nLV."+ attackPercentLevel.ToString();

        ScoreBoardText.text = $"\n\n☆{scoreboardsay}☆\n\n\nSCORE : {score}\n\nTOTAL DAMAGE : {300 - bossLife}\n\nATTACK COUNT : {attackCount}";

    }
}
