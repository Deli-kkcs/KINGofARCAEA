using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;//���ú����ͱ���!
    public int count_Player;
    public int index_Round;//������1�� = 2Ȧ
    public int index_Circle;//Ȧ��
    public bool isSwitchHolder;//��������
    public int init_draw_num;//��ʼ������

    public enum STATE
    {
        STATE_GAME_IDLING,
        STATE_GAME_STARTED,
        STATE_GAME_SUMMARY,
        STATE_DRAW_CARDS,
        STATE_JUDGE_CARDS,
        STATE_YIELD_CARDS,
        STATE_THROW_CARDS
    }
    public static STATE state_ = STATE.STATE_GAME_IDLING;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        instance.count_Player = PlayerManager.list_player_info.Count;
        //Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state_)
        {
            case STATE.STATE_GAME_IDLING:
                break;
            case STATE.STATE_GAME_STARTED:
                break;
            case STATE.STATE_GAME_SUMMARY:
                break;
            case STATE.STATE_DRAW_CARDS:
                break;
            case STATE.STATE_JUDGE_CARDS:
                break;
            case STATE.STATE_YIELD_CARDS:
                break;
            case STATE.STATE_THROW_CARDS:
                break;
            default:
                break;
        }
    }

    public void Initialize()
    {
        instance.index_Round = 0;
        instance.init_draw_num = 4;
        instance.isSwitchHolder = false;
        PlayerManager.instance.Initialize();
        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.Initialize();
        //GameCardManager.instance.Initialize();
    }

    public void StartGame()
    {
        Initialize();
        state_ = STATE.STATE_GAME_STARTED;
        NewRound();//0�����һ��֮ǰ
    }
    public void NewRound()
    {
        instance.index_Round++;
        instance.index_Circle = 1;
        instance.isSwitchHolder = true;
        UIManager.instance.text_CircleNum.text = instance.index_Circle.ToString();
        if (instance.index_Round == 1)//��һ��֮ǰ
        {
            PlayerManager.index_CurrentHolder = 1;
            PlayerManager.list_player[PlayerManager.index_CurrentHolder - 1].image_Holder.SetActive(true);
            for (int i = 0; i < instance.count_Player; i++)
            {
                PlayerManager.list_player[i].DrawHandCards(4,i);
                PlayerManager.list_player[i].DrawScoreCards(1,i);
            }
            
        }
        else
        {
            PlayerManager.list_player[PlayerManager.index_CurrentHolder - 1].image_Holder.SetActive(false);
            PlayerManager.index_CurrentHolder++;
            if (PlayerManager.index_CurrentHolder > instance.count_Player)
            {
                SummaryGame();
                return;
            }
            PlayerManager.list_player[PlayerManager.index_CurrentHolder - 1].image_Holder.SetActive(true);
        }
        
        NewTurn();
        instance.isSwitchHolder = false;
    }
    public void NewTurn()
    {
        if(instance.isSwitchHolder)
        {
            if (instance.index_Round == 1)//��һ��
            {
                PlayerManager.index_CurrentPlayer = 1;
            }
            else if (instance.index_Round != 1)//�ǵ�һ��
            {
                PlayerManager.instance.PassTurn();
                PlayerManager.index_CurrentPlayer = PlayerManager.index_CurrentHolder;
            }
        }
        else
        {
            PlayerManager.instance.PassTurn();
            PlayerManager.index_CurrentPlayer++;
            if (PlayerManager.index_CurrentPlayer > instance.count_Player)
            {
                PlayerManager.index_CurrentPlayer = 1;
            }
            if (PlayerManager.index_CurrentPlayer == PlayerManager.index_CurrentHolder)
            {
                instance.index_Circle++;
                UIManager.instance.text_CircleNum.text = instance.index_Circle.ToString();
            }
            if (instance.index_Circle == 3)
            {
                NewRound();
                return;
            }
        }
        PlayerManager.instance.MyTurn();
    }
    
    public void SummaryGame()
    {
        state_ = STATE.STATE_GAME_SUMMARY;
        Debug.Log("GAME OVER !!!");
    }
}
