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

    public enum Temp_STATE
    {
        STATE_BUSYCONNECTING,
        
        STATE_GAME_IDLING,
        STATE_GAME_STARTED,
        
        STATE_DRAW_CARDS,
        STATE_YIELD_CARDS,
        STATE_SELECTING_TARGETPLAYER,
        STATE_THROW_CARDS,
        STATE_JUDGE_CARDS,
        STATE_ADDING_SCORES,
        STATE_GAME_SUMMARY,
    }
    public Temp_STATE state_;
    void Awake()
    {
        state_ = Temp_STATE.STATE_GAME_IDLING;
        instance = this;
    }
    void Start()
    {
        //RefillHandCards();
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void Initialize()
    {
        instance.index_Round = 0;
        instance.init_draw_num = 4;
        instance.isSwitchHolder = false;
        //UIPlayerManager.instance.Initialize();
        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.RefillHandCards();

        //GameCardManager.list_instance.RefillHandCards();
    }

    //public void StartGame()
    //{
    //    Initialize();
    //    state_ = Temp_STATE.STATE_GAME_STARTED;
    //    NewRound();
    //}
    //public void NewRound()
    //{
    //    instance.index_Round++;
    //    instance.index_Circle = 1;
    //    instance.isSwitchHolder = true;
    //    UIManager.instance.text_CircleNum.text = instance.index_Circle.ToString();
    //    if (instance.index_Round == 1)//��һ��֮ǰ
    //    {
    //        UIPlayerManager.index_CurrentHolder = 1;
    //        UIPlayerManager.list_player[UIPlayerManager.index_CurrentHolder - 1].GetComponent<Player>().image_Holder.SetActive(true);
    //        for (int i = 0; i < instance.count_Player; i++)
    //        {
    //            UIPlayerManager.list_player[i].GetComponent<Player>().DrawHandCards(4,i);
    //            UIPlayerManager.list_player[i].GetComponent<Player>().DrawScoreCards(1,i);
    //        }
            
    //    }
    //    else
    //    {
    //        UIPlayerManager.list_player[UIPlayerManager.index_CurrentHolder - 1].GetComponent<Player>().image_Holder.SetActive(false);
    //        UIPlayerManager.index_CurrentHolder++;
    //        if (UIPlayerManager.index_CurrentHolder > instance.count_Player)
    //        {
    //            SummaryGame();
    //            return;
    //        }
    //        UIPlayerManager.list_player[UIPlayerManager.index_CurrentHolder - 1].GetComponent<Player>().image_Holder.SetActive(true);
    //    }
        
    //    NewTurn();
    //    instance.isSwitchHolder = false;
    //}
    //public void NewTurn()
    //{
    //    if(instance.isSwitchHolder)
    //    {
    //        if (instance.index_Round == 1)//��һ��
    //        {
    //            UIPlayerManager.index_CurrentPlayer = 1;
    //        }
    //        else if (instance.index_Round != 1)//�ǵ�һ��
    //        {
    //            UIPlayerManager.instance.PassTurn();
    //            UIPlayerManager.index_CurrentPlayer = UIPlayerManager.index_CurrentHolder;
    //        }
    //    }
    //    else
    //    {
    //        UIPlayerManager.instance.PassTurn();
    //        UIPlayerManager.index_CurrentPlayer++;
    //        if (UIPlayerManager.index_CurrentPlayer > instance.count_Player)
    //        {
    //            UIPlayerManager.index_CurrentPlayer = 1;
    //        }
    //        if (UIPlayerManager.index_CurrentPlayer == UIPlayerManager.index_CurrentHolder)
    //        {
    //            instance.index_Circle++;
    //            UIManager.instance.text_CircleNum.text = instance.index_Circle.ToString();
    //        }
    //        if (instance.index_Circle == 3)
    //        {
    //            NewRound();
    //            return;
    //        }
    //    }
    //    UIPlayerManager.instance.MyTurn();
    //}
    
    public void SummaryGame()
    {
        //state_ = Temp_STATE.STATE_GAME_SUMMARY;
        Debug.Log("GAME OVER !!!");
    }
}
