using Mirror;
//using System;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Empty : NetworkBehaviour
{
    public static Empty instance;

    public static List<int> list_netId = new();//���Id�б�
    public static List<string> list_playerName = new();//��������б�


    /// <summary>
    /// Empty <- GameManager
    /// </summary>
    //public enum Temp_STATE
    //{
    //    STATE_GAME_IDLING,
    //    STATE_GAME_STARTED,
    //    STATE_GAME_SUMMARY,
    //    STATE_DRAW_CARDS,
    //    STATE_JUDGE_CARDS,
    //    STATE_YIELD_CARDS,
    //    STATE_THROW_CARDS
    //}
    //public static Temp_STATE state_ = Temp_STATE.STATE_GAME_IDLING;
    public static int index_Round;//������1�� = 2Ȧ
    public static int index_Circle;//Ȧ��
    public static bool isSwitchHolder;//��������
    public static int init_draw_num;//��ʼ������

    /// <summary>
    /// Empty <- UIPlayerManager
    /// </summary>
    public static int index_CurrentPlayer = 0;//��1��ʼ��
    public static int index_CurrentHolder = 0;//��1��ʼ��


    /// <summary>
    /// Empty <- Player
    /// </summary>
    //public int totalScore;
    //public List<int> roundScore = new();////�����
    //public int totalMove;
    public List<int> turnMove = new();////�����
    public int count_MyHandCard;
    //public int count_RoundUsedCard;
    //public int count_TotalUsedCard;
    //public GameObject last_selectedCard;
    public GameObject selectedCard;
    public GameObject scoreCard;
    //public List<GameObject> handCards = new();


    /// <summary>
    /// Card
    /// </summary>
    public int index_Shown;
    public List<int> list_Card_1002_ScoreCard = new();
    public List<int> list_Card_1002_netId_ScoreCard = new();
    public List<int> list_Card_1005or1006_ScoreCard = new();
    public List<int> list_Card_1005or1006_netId_ScoreCard = new();
    public int temp_Card_1002_id_attacker;
    public List<int> temp_Card_1002_list_index_offender = new();
    public List<int> temp_card_1005or1006_list_index_offender;
    public int temp_Card_1007_checkCount;
    public int temp_Card_1007_id_attacker;
    public int temp_1008_index_holder;

    public List<int> temp_list_index_offender = new();
    /// <summary>
    /// State 
    /// </summary>
    public List<StateCard> list_stateCards = new();


    public float delay = 0.2f;




    private void Awake()
    {
        Delay_set_instance();
    }


    public void Delay_set_instance()
    {
        //Debug.Log("Delay_set_instance()");
        if (isLocalPlayer)
        {
            instance = this;
            //Debug.Log("AWAKE instance.netId = " + instance.netId);
        }
        if (!instance)
        {
            Invoke(nameof(Delay_set_instance), delay);
        }
    }
    [Command]
    public void CmdSetState(GameManager.Temp_STATE state)
    {
        //Debug.Log(GameManager.instance.state_ = state);
        GameManager.instance.state_ = state;
        instance.RpcSetState(state);
    }
    [Command]
    public void CmdAddPlayer(int added_netId, string added_name)
    {
        //Debug.Log("CmdAddPlayer()");
        ServerAddPlayer(added_netId, added_name);
    }
    [Command]
    public void CmdStartGame()
    {
        ServerStartGame();
    }
    [Command]
    public void CmdDiscardScoreCard(int index_ScoreCard)
    {
        RpcDiscardScoreCard(index_ScoreCard);
    }
    [Command]
    public void CmdDiscardHandCard(int onlineID, int index)
    {
        RpcDiscardHandCard(onlineID, index);
    }
    [Command]
    public void CmdDrawHandCards(int onlineID, int times)
    {
        RpcDrawHandCards(onlineID, times);
    }
    [Command]
    public void CmdDrawScoreCard(int onlineID, bool canDiscard)
    {
        RpcDrawScoreCard(onlineID,canDiscard);
    }
    [Command]
    public void CmdNewTurn()
    {
        instance.ServerNewTurn();
    }
    [Command]
    public void CmdGetHisAllHandCards(int id_attacker, List<int> list_index_offender)//1001 ����
    {
        RpcGetHisAllHandCards(id_attacker, list_index_offender);
    }
    [Command]
    public void CmdGiveMyAllHandCards(int id_attacker, List<int> list_index_handCard)//1001 ����
    {
        RpcReceiveHisAllHandCards(id_attacker, list_index_handCard);
    }
    [Command]
    public void CmdClearAllHandCards(int onlineID)//1001 ����
    {
        RpcClearAllHandCards(onlineID);
    }
    [Command]
    public void CmdDrawHandCards_Specific(int onlineID, List<int> list_index_handCard)//1001 ����
    {
        RpcDrawHandCards_Specific(onlineID, list_index_handCard);
    }
    [Command]
    public void CmdDrawScoreCard_Specific(int onlineID, int score)//1003 ָ�㽭ɽ
    {
        //RpcDrawScoreCard_Specific(onlineID, score);
    }
    [Command]
    public void CmdCard_1002_CollectAllScoreCards(int id_attacker, List<int> list_index_offender)
    {
        instance.list_Card_1002_ScoreCard.Clear();
        instance.list_Card_1002_netId_ScoreCard.Clear();
        for (int i = 0; i < list_index_offender.Count; i++)
        {
            //bool isOver = (i == list_netId.Count - 1);
            RpcCard_1002_CollectAllScoreCards(list_index_offender[i]);
        }
        instance.temp_Card_1002_id_attacker = id_attacker;
        instance.temp_Card_1002_list_index_offender = list_index_offender;
        instance.ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard();
    }
    [Command]
    public void CmdCard_1002_AddScoreCard(int score, int index_offender)
    {
        instance.list_Card_1002_ScoreCard.Add(score);
        instance.list_Card_1002_netId_ScoreCard.Add(list_netId[index_offender]);
        Debug.Log("1002   #" + GetContent_int(instance.list_Card_1002_ScoreCard));
        
    }
    [Command]
    public void CmdCard_1002_NextTurn(int id_turn, int index_last_Selected)
    {
        RpcCard_1002_NextTurn(id_turn, index_last_Selected);
    }
    [Command]
    public void CmdCard_1003_GetHisScoreCard(int id_attacker, List<int> list_index_offender,int index_Card)
    {
        RpcCard_1003_GetHisScoreCard(id_attacker, list_index_offender,index_Card);
    }
    [Command]
    public void CmdCard_1003_ReceiveScoreCard(int id_attacker, List<int> list_index_offender,int score)
    {
        RpcCard_1003_ReceiveScoreCard(id_attacker, list_index_offender,score);
    }
    [Command]
    public void CmdCard_1004_GetHisScoreCard(List<int> list_index_offender)
    {
        RpcCard_1004_GetHisScoreCard(list_index_offender);
    }
    [Command]
    public void CmdCard_1004_Show_Panel(int index_offender,int score)
    {
        RpcCard_1004_Show_Panel(index_offender,score);
    }
    [Command]
    public void CmdCard_1005_GetLeftSuspectedScore(List<int> list_index_offender)
    {
        RpcCard_1005_GetLeftSuspectedScore(list_index_offender);
    }
    [Command]
    public void CmdCard_1005or1006_CollectAllScoreCards(List<int> list_index_offender,int index_card)
    {
        instance.list_Card_1005or1006_ScoreCard.Clear();
        instance.list_Card_1005or1006_netId_ScoreCard.Clear();
        for (int i = 0; i < list_netId.Count; i++)
        {
            //bool isOver = (i == list_netId.Count - 1);
            RpcCard_1005or1006_CollectAllScoreCards(list_index_offender, list_netId[i]);
        }
        instance.temp_card_1005or1006_list_index_offender = list_index_offender;
        switch(index_card)
        {
            case 1005:
                instance.ServerDelay_RpcCard_1005_GetLeftScoreCard();
                break;
            case 1006:
                instance.ServerDelay_RpcCard_1006_GetRightScoreCard();
                break;
            default:break;
        }
    }
    [Command]
    public void CmdCard_1005or1006_AddScoreCard(int onlineId, int score/*, bool isover*/)
    {
        instance.list_Card_1005or1006_netId_ScoreCard.Add(onlineId);
        instance.list_Card_1005or1006_ScoreCard.Add(score);
        Debug.Log("�������ֵ" + score + "  id  = " + onlineId) ;
    }
    [Command]
    public void CmdCard_1006_GetRightSuspectedScore(List<int> list_index_offender)
    {
        RpcCard_1006_GetRightSuspectedScore(list_index_offender);
    }
    [Command]
    public void CmdCard_1007_CollectScoreCards(int id_attacker, List<int> list_index_offender)
    {
        instance.list_Card_1005or1006_ScoreCard.Clear();
        instance.list_Card_1005or1006_netId_ScoreCard.Clear();
        instance.temp_Card_1007_checkCount = 0;
        //int temp_index_CurrentPlayer = index_CurrentPlayer;
        int my_index = GetIndex_in_list_netId(id_attacker);
        if(index_Circle < 2)
        {
            Debug.Log("��һȦ my_index = " + my_index);
            for (int i = 0; i < list_netId.Count; i++)
            {
                if (i == my_index) continue;
                if(!list_index_offender.Contains(i)) continue;
                instance.temp_Card_1007_checkCount++;
                RpcCard_1007_CollectScoreCards(list_netId[i]);
            }
        }
        else if(index_Circle == 2)
        {
            for(int i = my_index + 1; ;i++)
            {
                if (i == list_netId.Count) i = 0;
               
                if (i == index_CurrentHolder - 1) break;
                if (!list_index_offender.Contains(i)) continue;
                instance.temp_Card_1007_checkCount++;
                RpcCard_1007_CollectScoreCards(list_netId[i]);
            }
        }
        Debug.Log("checkcount " + instance.temp_Card_1007_checkCount);
        instance.temp_Card_1007_id_attacker = id_attacker;
        instance.ServerDelay_ClientCard_1007_ShowPanel();
    }
    [Command]
    public void CmdCard_1008_AddBeforeRealize(int index_Card,int id_attacker,List<int> list_index_offender)
    {

        StateCard added = new();
        added.index_Card = index_Card;
        added.id_attacker = id_attacker;
        added.list_index_offender = list_index_offender;
        instance.list_stateCards.Add(added);

    }
    [Command]
    public void CmdCard_1008_Realize(int index_holder)
    {
        instance.temp_1008_index_holder = index_holder;
        instance.ServerDelay_ClientCard_1008_Realize();
        //instance.RpcCard_1008_BeforeRealize(index_holder);
    }
    //[Command]
    //public void CmdCard_1008_Realize()
    //{
    //    instance.RpcCard_1008_Realize();
    //}
    [Command]
    public void CmdCard_1008_ShowPanel(int id_attacker, int id_offender, int score)
    {
        RpcCard_1008_ShowPanel(id_attacker, id_offender, score);
    }
    [Command]
    public void CmdCheckCard_2001and2002(int index_Card,int id_attacker, List<int> list_index_offender)
    {
        instance.RpcCard_2001and2002_ShowPanel(index_Card, id_attacker, list_index_offender);

    }
    [Command]
    public void CmdCard_2001and2002_NextTurn(int index_Card,int id_attacker, int id_turn, List<int> list_index_offender)
    {
        instance.RpcCard_2001and2002_NextTurn(index_Card, id_attacker,id_turn, list_index_offender);
    }
    [Command]
    public void CmdCard_2001and2002_RefreshList(List<int> list_index_offender)
    {
        instance.RpcCard_2001and2002_RefreshList(list_index_offender);
    }
    [Command]
    public void CmdAddStateCard(int id_attacker, List<int> list_index_offender,int index_Card)
    {
        RpcAddStateCard(id_attacker, list_index_offender, index_Card);
    }


    //[Command]
    //public void CmdYieldCard()
    //{
    //    RpcYieldCard();
    //}
    //[Command]
    //public void CmdThrowCard()
    //{
    //    RpcThrowCard();
    //}
    [Server]
    public void ServerAddPlayer(int added_netId, string added_name)
    {
        if (CheckRepeatedNetId(added_netId))
        {
            Debug.Log("id�ظ���");
            return;
        }
        //Debug.Log("ServerAddPlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (added_netId == 1) return;
        list_netId.Add(added_netId);

        list_playerName.Add(added_name);



        RpcClearPlayer();
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]);
        }
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
        //Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        //Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
    }
    [Server]
    public void ServerRomovePlayer(int removed_netId)
    {
        //Debug.Log("ServerRomovePlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (removed_netId == 1) return;
        int removed_index = -1;
        for (int i = 0; i < list_netId.Count; i++)
        {
            if (list_netId[i] == removed_netId)
            {
                removed_index = i;
                break;
            }
        }
        if (removed_index == -1)
        {
            Debug.LogError("δ�ҵ�ID��ɾ��!!");
            return;
        }



        Debug.Log("[Server] �뿪 " + removed_netId + " " + list_playerName[removed_index]);
        list_netId.RemoveAt(removed_index);
        list_playerName.RemoveAt(removed_index);
        //Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        //Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
        RpcClearPlayer();
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]);
        }

        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
    }
    [Server]
    public void ServerStartGame()
    {
        index_Round = 0;
        init_draw_num = 4;
        isSwitchHolder = false;
        index_CurrentHolder = index_CurrentPlayer = 0;

        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.RefillHandCards();
        instance.RpcInitialize(ScoreCardManager.list_index, HandCardManager.list_index);
        instance.ServerSetState(GameManager.Temp_STATE.STATE_TURNING_ROUND);
        instance.ServerDelay_NewRound();
    }
    [Server]
    public void ServerDelay_NewRound()
    {
        if (GameManager.instance.state_ !=  GameManager.Temp_STATE.STATE_TURNING_ROUND)
        {
            //Debug.Log("[Server]Delay_NewRound");
            Invoke(nameof(ServerDelay_NewRound), 0.75f);
            return;
        }
        ServerNewRound();
    }
    [Server]
    public void ServerNewRound()
    {
        index_Round++;
        index_Circle = 1;
        isSwitchHolder = true;
        UIManager.instance.text_CircleNum.text = index_Circle.ToString();
        
        if (index_Round == 1)//��һ��֮ǰ
        {
            index_CurrentHolder = 1;
            RpcSetHolder(index_CurrentHolder - 1, true);

            for (int i = 0; i < list_netId.Count; i++)
            {
                RpcDrawScoreCard(list_netId[i],true);////�п�
                RpcDrawHandCards(list_netId[i], init_draw_num);////�п�
            }
            instance.ServerSetState(GameManager.Temp_STATE.STATE_TURNING_TURN);
            instance.ServerNewTurn();
            isSwitchHolder = false;
        }
        else
        {
            instance.list_Card_1002_ScoreCard.Clear();
            instance.list_Card_1002_netId_ScoreCard.Clear();
            for (int i = 0; i < list_netId.Count; i++)
            {
                instance.RpcCard_1002_CollectAllScoreCards(i);
            }
            Empty.instance.RpcSetState(GameManager.Temp_STATE.STATE_BUSYCONNECTING);
            instance.ServerDelay_RpcStartGainScore();
            instance.ServerDelay_NextRound();
        }
    }
    [Server]
    public void ServerDelay_NextRound()
    {
        if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_TURNING_ROUND)
        {
            //Debug.Log("[Server]Delay_NextRound");
            Invoke(nameof(ServerDelay_NextRound), 0.75f);
            return;
        }
        instance.RpcClearAllSuspectedCardOnNewRound();
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcDrawScoreCard(list_netId[i], true);////�п�
        }
        RpcSetHolder(index_CurrentHolder - 1, false);
        index_CurrentHolder++;
        if (index_CurrentHolder > list_netId.Count)
        {
            ///////SummaryGame();
            return;
        }
        RpcSetHolder(index_CurrentHolder - 1, true);
        instance.ServerNewTurn();
        isSwitchHolder = false;
    }
    [Server]
    public void ServerNewTurn()
    {
        if (isSwitchHolder)
        {
            if (index_Round == 1)//��һ��
            {
                index_CurrentPlayer = 1;
            }
            else if (index_Round != 1)//�ǵ�һ��
            {
                RpcPassTurn(index_CurrentPlayer - 1);
                index_CurrentPlayer = index_CurrentHolder;
            }
        }
        else
        {
            RpcPassTurn(index_CurrentPlayer - 1);
            index_CurrentPlayer++;
            if (index_CurrentPlayer > list_netId.Count)
            {
                index_CurrentPlayer = 1;
            }
            if (index_CurrentPlayer == index_CurrentHolder)
            {
                index_Circle++;
                instance.ServerSetState(GameManager.Temp_STATE.STATE_JUDGE_CARDS);
                UIPlayerManager.instance.Card_1008_Collect(index_CurrentPlayer - 1);//�ӵ�ǰ�����˿�ʼ��ȡ1008״̬��
                instance.ServerDelay_NewCircle();
                return;
            }
        }
        RpcSetIndex(index_Circle, index_CurrentPlayer, index_CurrentHolder, index_Round);
        RpcMyTurn(index_CurrentPlayer - 1);
    }
    [Server]
    public void ServerDelay_NewCircle()
    {
        if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_TURNING_TURN)
        {
            Invoke(nameof(ServerDelay_NewCircle), 0.3f);
            return;
        }
        instance.RpcClearStatesOnNewCircle();/////////λ�úò��ã�
        if (index_Circle == 3)
        {
            ServerNewRound();
            return;
        }
        RpcSetIndex(index_Circle, index_CurrentPlayer, index_CurrentHolder, index_Round);
        RpcMyTurn(index_CurrentPlayer - 1);
    }
    [Server]
    public void ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard()
    {

        if (instance.list_Card_1002_ScoreCard.Count != instance.temp_Card_1002_list_index_offender.Count)
        {
            //Debug.Log("[Server]Delay_RpcCard_1002_Show_Panel_SelectScoreCard");
            Invoke(nameof(ServerDelay_RpcCard_1002_Show_Panel_SelectScoreCard), delay);
            return;
        }
        Debug.Log("#1002 �������б� = " + GetContent_int(instance.list_Card_1002_ScoreCard));
        List<int> randomed_list_scoreCard = RandomList(instance.list_Card_1002_ScoreCard);
        instance.RpcCard_1002_Show_Panel_SelectScoreCard(instance.temp_Card_1002_id_attacker, instance.temp_Card_1002_list_index_offender, randomed_list_scoreCard);
    }
    [Server]
    public void ServerDelay_RpcCard_1005_GetLeftScoreCard()
    {
        Debug.Log("TTT");
        if (instance.list_Card_1005or1006_ScoreCard.Count != list_netId.Count)
        {
            Debug.Log("[Server]Delay_RpcCard_1005_GetLeftScoreCard");
            Invoke(nameof(ServerDelay_RpcCard_1005_GetLeftScoreCard), delay);
            return;
        }
        
        Debug.Log("#1005 �������б� = " +  GetContent_int(instance.list_Card_1005or1006_ScoreCard));
        Debug.Log("���������˵�id�б� = " +  GetContent_int(instance.list_Card_1005or1006_netId_ScoreCard));
        Debug.Log("�ܻ��ߵ�id�б� = " +  GetContent_int(instance.temp_card_1005or1006_list_index_offender));
        instance.RpcCard_1005_GetLeftScoreCard(instance.temp_card_1005or1006_list_index_offender,instance.list_Card_1005or1006_ScoreCard,instance.list_Card_1005or1006_netId_ScoreCard);
    }
    [Server]
    public void ServerDelay_RpcCard_1006_GetRightScoreCard()
    {
        if (instance.list_Card_1005or1006_ScoreCard.Count != list_netId.Count)
        {
            Debug.Log("[Server]Delay_RpcCard_1006_GetRightScoreCard");
            Invoke(nameof(ServerDelay_RpcCard_1006_GetRightScoreCard), delay);
            return;
        }
        
        Debug.Log("�������б� = " + GetContent_int(list_Card_1005or1006_ScoreCard));
        instance.RpcCard_1006_GetRightScoreCard(instance.temp_card_1005or1006_list_index_offender, instance.list_Card_1005or1006_ScoreCard, instance.list_Card_1005or1006_netId_ScoreCard);
    }
    [Server]
    public void ServerDelay_ClientCard_1007_ShowPanel()
    {
        if (list_Card_1005or1006_ScoreCard.Count != instance.temp_Card_1007_checkCount)
        {
            //Debug.Log("Delay 1007");
            Invoke(nameof(ServerDelay_ClientCard_1007_ShowPanel), delay);
            return;
        }
        instance.RpcCard_1007_ShowPanel(instance.temp_Card_1007_id_attacker, instance.list_Card_1005or1006_ScoreCard, instance.list_Card_1005or1006_netId_ScoreCard);
    }
    [Server]
    public void ServerDelay_ClientCard_1008_Realize()
    {
        //Debug.Log("#1008 index_holder = " + instance.temp_1008_index_holder);
        
        bool found_id = false;
        //Debug.Log("#1008 ״̬������ =" + instance.list_stateCards.Count);
        for (int i = 0; i < instance.list_stateCards.Count; i++)
        {
            if (instance.list_stateCards[i].id_attacker == Empty.list_netId[instance.temp_1008_index_holder] && (instance.list_stateCards[i].index_Card == 1008))
            {
                found_id = true;
                instance.RpcCard_1003_GetHisScoreCard(instance.list_stateCards[i].id_attacker, instance.list_stateCards[i].list_index_offender,1008);
                instance.list_stateCards.RemoveAt(i);
                //Debug.Log("#1008 �Ƴ��� " + i + "��״̬��");
            }
        }
        if (!found_id)
        {
            instance.temp_1008_index_holder++;
            if (instance.temp_1008_index_holder == list_netId.Count)
            {
                instance.temp_1008_index_holder = 0;
            }
            if (instance.temp_1008_index_holder == index_CurrentHolder - 1)
            {
                //Debug.Log("#1008 ����");
                instance.RpcCard_1008_ClosePanel();
                instance.ServerSetState(GameManager.Temp_STATE.STATE_TURNING_TURN);
                return;
            }
        }
        Invoke(nameof(ServerDelay_ClientCard_1008_Realize), 3f);
    }
    [Server]
    public void ServerDelay_RpcStartGainScore()
    {
        if (instance.list_Card_1002_ScoreCard.Count != list_netId.Count)
        {
            Debug.Log("[Server]Delay_RpcCard_1002_Show_Panel_SelectScoreCard");
            Invoke(nameof(ServerDelay_RpcStartGainScore), delay);
            return;
        }
        Debug.Log("�����б� = " + GetContent_int(instance.list_Card_1002_ScoreCard) + " ����id�б� = " + GetContent_int(instance.list_Card_1002_netId_ScoreCard));
        instance.RpcStartGainScore(index_CurrentHolder - 1,instance.list_Card_1002_ScoreCard,instance.list_Card_1002_netId_ScoreCard);
    }
    [Server]
    public void ServerSetState(GameManager.Temp_STATE state)
    {
        GameManager.instance.state_ = state;
        instance.RpcSetState(state);
    }
    [ClientRpc]
    public void RpcClearPlayer()
    {
        list_netId.Clear();
        list_playerName.Clear();
    }
    [ClientRpc]
    public void RpcAddPlayer(int added_netId, string added_name)
    {
        //Debug.Log("[Client] ServerAddPlayer()");
        list_netId.Add(added_netId);
        list_playerName.Add(added_name);
        //Debug.Log("[Client] list_netId = " + GetContent_int(list_netId));
    }
    [ClientRpc]
    public void RpcRefreshPlayer()
    {
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
    }
    [ClientRpc]
    public void RpcInitialize(List<int> list_index_ScoreCards,List<int> list_index_HandCards)
    {
        instance.count_MyHandCard = 0;
        //instance.roundScore.Clear();
        instance.turnMove.Clear();
        //instance.totalMove = 0;
        //for (int j = 0; j < list_netId.Count; j++)
        //{
        //    instance.roundScore.Add(0);
        //}
        ScoreCardManager.instance.RefreshScoreCards(list_index_ScoreCards);
        HandCardManager.instance.RefreshHandCards(list_index_HandCards);
        UIManager.instance.ClearHandCards_and_ScoreCard();
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        //GameCardManager.list_instance.RefillHandCards();
    }
    [ClientRpc]
    public void RpcDrawScoreCard(int onlineID,bool canDiscard)
    {
        if ((int)instance.netId !=onlineID)
        {
            ScoreCardManager.instance.Sync_DrawOneCard();
        }
        else
        {
            ScoreCardManager.instance.DrawOneCard(canDiscard);
        }
    }
    [ClientRpc]
    public void RpcDrawScoreCard_Specific(int onlineID, int score)//1003 ָ�㽭ɽ
    {
        if((int)instance.netId == onlineID)
        {
            
        }
    }
    [ClientRpc]
    public void RpcDrawHandCards(int onlineID,int times)
    {
        int index = GetIndex_in_list_netId(onlineID);
        //for (int i = 0; i < list_netId.Count; i++)
        //{
        //    if (list_netId[i] == onlineID)
        //    {
        //        onlineID = i;
        //        break;
        //    }
        //}
        for (int j = 0; j < times;j++)
        {
            if ((int)instance.netId != onlineID)
            {
                HandCardManager.instance.Sync_DrawOneCard();
            }
            else
            {
                HandCardManager.instance.DrawOneCard();
                
            }
            
            UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text)+1).ToString();
        }
    }
    [ClientRpc]
    public void RpcDrawHandCards_Specific(int onlineID, List<int> list_index_handCard)
    {
        int index = GetIndex_in_list_netId(onlineID);
        //for (int i = 0; i < list_netId.Count; i++)
        //{
        //    if (list_netId[i] == onlineID)
        //    {
        //        onlineID = i;
        //        break;
        //    }
        //}
        for (int j = 0; j < list_index_handCard.Count; j++)
        {
            if ((int)instance.netId != onlineID)
            {
                //HandCardManager.instance.Sync_DrawOneCard_Specific(); //��û��ʵ��
            }
            else
            {
                HandCardManager.instance.DrawOneCard_Specific(list_index_handCard[j]);

            }
            UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) + 1).ToString();
        }
        instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    [ClientRpc]
    public void RpcDiscardScoreCard(int onlineID)
    {
        UIManager.instance.DiscardScorecard(onlineID,new Vector2(Random.Range(-500f, 500f), Random.Range(-200f, 300f)), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
    }
    [ClientRpc]
    public void RpcDiscardHandCard(int onlineID,int index_Card)
    {
        int index_player = GetIndex_in_list_netId(onlineID);
        UIPlayerManager.list_player[index_player].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index_player].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
        UIManager.instance.DiscardHandcard(index_Card, new Vector2(Random.Range(-500f, 500f), Random.Range(-200f, 300f)), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
    }
    [ClientRpc]
    public void RpcSetHolder(int index, bool state)
    {
        UIPlayerManager.list_player[index].GetComponent<Player>().image_Holder.SetActive(state);
    }
    [ClientRpc]
    public void RpcPassTurn(int index)
    {
        UIPlayerManager.instance.PassTurn(index);
    }
    [ClientRpc]
    public void RpcMyTurn(int index)
    {
        instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
        if (list_netId[index] == (int)instance.netId)
        {
            UIPlayerManager.instance.MyTurn(index);
        }
        else
        {
            UIPlayerManager.instance.Sync_MyTurn(index);
        }
    }
    [ClientRpc]
    public void RpcSetIndex(int num_circle,int index_currentPlayer,int index_currentHolder,int index_round)
    {
        UIManager.instance.text_CircleNum.text = num_circle.ToString();
        index_CurrentPlayer = index_currentPlayer;
        index_CurrentHolder = index_currentHolder;
        index_Round = index_round;
    }
    [ClientRpc]
    public void RpcGetHisAllHandCards(int id_attacker, List<int> list_index_offender)
    {
        if (list_index_offender.Contains(GetIndex_in_list_netId((int)instance.netId)))
        {
            int count_myHandCards = HandCardManager.instance.GetCountOfMyHandCards();
            Debug.Log("�ҵ������� = " + instance.count_MyHandCard);
            instance.ClientGiveMyAllHandCards(id_attacker,HandCardManager.instance.GetIndexesOfMyHandCards());
            

            instance.ClientClearAllHandCards((int)instance.netId);
            instance.ClientDrawHandCards((int)instance.netId, count_myHandCards);
        }
    }
    [ClientRpc]
    public void RpcReceiveHisAllHandCards(int id_attacker, List<int> list_index_handCard)
    {
        //GameManager.state_ = GameManager.Temp_STATE.STATE_BUSYCONNECTING;
        if ((int)instance.netId == id_attacker)
        {
            instance.ClientDrawHandCards_Specific((int)instance.netId, list_index_handCard);
            //instance.ClientOnEndRealizeHandCard();
        }
    }
    [ClientRpc]
    public void RpcClearAllHandCards(int onlineID)
    {
        int index_player = GetIndex_in_list_netId(onlineID);
        UIPlayerManager.list_player[index_player].GetComponent<Player>().Text_CardNum.text = "0";
    }
    [ClientRpc]
    public void RpcCard_1002_CollectAllScoreCards(int index_offender/*,bool isover*/)
    {
        if ((int)instance.netId != list_netId[index_offender]) return;
        instance.scoreCard.SetActive(false);
        instance.CmdCard_1002_AddScoreCard(instance.scoreCard.GetComponent<ScoreCard>().score, index_offender);
        Destroy(instance.scoreCard);
    }
    [ClientRpc]
    public void RpcCard_1002_Show_Panel_SelectScoreCard(int id_attacker,List<int> list_index_offender,List<int> list_scoreCard)
    {
        UIManager.instance.UICard_1002_ShowPanel(id_attacker, list_index_offender, list_scoreCard);
        //if ((int)instance.netId != id_attacker) return;
    }
    [ClientRpc]
    public void RpcCard_1002_NextTurn(int last_id_turn, int index_last_Selected)
    {
        int index_id_turn = GetIndex_in_list_netId(last_id_turn);
        index_id_turn++;
        if(index_id_turn == list_netId.Count)
        {
            index_id_turn = 0;
        }
        UIManager.instance.UICard_1002_NextTurn(last_id_turn, list_netId[index_id_turn], index_last_Selected);
    }
    [ClientRpc]
    public void RpcCard_1003_GetHisScoreCard(int id_attacker,List<int> list_index_offender,int index_Card)
    {
        if (list_index_offender.Count == 0) return;
        if (list_index_offender[0] == GetIndex_in_list_netId((int)instance.netId))
        {
            if (index_Card == 1008)
            {
                Debug.Log("#1008 JUDGE");
                instance.CmdCard_1003_ReceiveScoreCard(id_attacker, list_index_offender, instance.scoreCard.GetComponent<ScoreCard>().score);
                return; 
            }
            if (list_index_offender[0] == GetIndex_in_list_netId(id_attacker))//ѡ���Լ��൱��remake
            {
                instance.ClientDrawScoreCard((int)instance.netId, true);
            }
            else
            {
                instance.CmdCard_1003_ReceiveScoreCard(id_attacker, list_index_offender, instance.scoreCard.GetComponent<ScoreCard>().score);
                instance.ClientDrawScoreCard((int)instance.netId, false);
            }
            
        }
        
    }
    [ClientRpc]
    public void RpcCard_1003_ReceiveScoreCard(int id_attacker, List<int> list_index_offender, int score)
    {
        if((int)instance.netId == id_attacker)
        {
            ScoreCardManager.instance.DrawOneCard_Specific(score);
            //instance.ClientDrawScoreCard_Specific((int)instance.netId,score);
            if(GameManager.instance.state_ == GameManager.Temp_STATE.STATE_JUDGE_CARDS) 
            {
                instance.CmdCard_1008_ShowPanel(id_attacker, list_netId[list_index_offender[0]], score);
                return; 
            }
            instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
        }
    }
    [ClientRpc]
    public void RpcCard_1004_GetHisScoreCard(List<int> list_index_offender)
    {
        if (list_index_offender[0] == GetIndex_in_list_netId((int)instance.netId))
        {
            instance.CmdCard_1004_Show_Panel(list_index_offender[0],instance.scoreCard.GetComponent<ScoreCard>().score);
        }
    }
    [ClientRpc]
    public void RpcCard_1004_Show_Panel(int index_offender, int score)
    {
        UIManager.instance.UICard_1004_ShowPanel(index_offender, score);
    }
    [ClientRpc]
    public void RpcCard_1005_GetLeftSuspectedScore(List<int> list_index_offender)
    {
        UIPlayerManager.instance.Card_1005_GetLeftSuspectedScore(list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_1005or1006_CollectAllScoreCards(List<int> list_index_offender, int index_offender)
    {
        if ((int)instance.netId != index_offender) return;
        instance.CmdCard_1005or1006_AddScoreCard((int)instance.netId,instance.scoreCard.GetComponent<ScoreCard>().score);
        //if (!list_index_offender.Contains((int)instance.netId)) return;
        //instance.scoreCard.SetActive(false);
        //Destroy(instance.scoreCard);
    }
    [ClientRpc]
    public void RpcCard_1005_GetLeftScoreCard(List<int> list_index_offender,List<int> list_Score,List<int>list_onlineId_of_ScoreCard)
    {
        if (list_index_offender.Count == 0 || list_index_offender.Count == 1)
        {
            Debug.Log("��������");
            return;
        }
        if (!list_index_offender.Contains(GetIndex_in_list_netId((int)instance.netId)))
        {
            Debug.Log("����ȥ��");
            return;
        }

        int my_index = GetIndex_in_list_netId((int)instance.netId);
        int last_index = my_index - 1;
        if(last_index < 0 )
        {
            last_index = list_netId.Count - 1;
        }
        while (!list_index_offender.Contains(last_index))
        {
            last_index--;
            if (last_index < 0)
            {
                last_index = list_netId.Count - 1;
            }
            if (last_index < -10)
            {
                Debug.Log("��ѭ����");
                return;
            }
        }
        
        int index_trueId = -1;
        for(int i=0;i<list_onlineId_of_ScoreCard.Count;i++)
        {
            if (list_onlineId_of_ScoreCard[i] == list_netId[last_index])
            {
                index_trueId = i;
                break;
            }
        }
        Debug.Log("my_index" + my_index + " last_index = " + last_index + " �����б�" + GetContent_int(list_Score) + " ��������λ�� " + index_trueId);
        ScoreCardManager.instance.Card_1002_ReGetScoreCard(ScoreCardManager.instance.GetScoreCardByScore(list_Score[index_trueId]));
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    [ClientRpc]
    public void RpcCard_1006_GetRightScoreCard(List<int> list_index_offender, List<int> list_Score, List<int> list_onlineId_of_ScoreCard)
    {
        if (list_index_offender.Count == 0 || list_index_offender.Count == 1)
        {
            Debug.Log("��������");
            return;
        }
        if (!list_index_offender.Contains(GetIndex_in_list_netId((int)instance.netId)))
        {
            Debug.Log("����ȥ��");
            return;
        }

        int my_index = GetIndex_in_list_netId((int)instance.netId);
        int next_index = my_index + 1;
        if (next_index >= list_netId.Count)
        {
            next_index = 0;
        }
        while (!list_index_offender.Contains(next_index))
        {
            next_index++;
            if (next_index >= list_netId.Count)
            {
                next_index = 0;
            }
            if (next_index > 990)
            {
                Debug.Log("��ѭ����");
                return;
            }
        }

        int index_trueId = -1;
        for (int i = 0; i < list_onlineId_of_ScoreCard.Count; i++)
        {
            if (list_onlineId_of_ScoreCard[i] == list_netId[next_index])
            {
                index_trueId = i;
                break;
            }
        }
        //Debug.Log("my_index" + my_index + " next_index = " + next_index + " �����б�" + GetContent_int(list_Score) + " ��������λ�� " + index_trueId);
        ScoreCardManager.instance.Card_1002_ReGetScoreCard(ScoreCardManager.instance.GetScoreCardByScore(list_Score[index_trueId]));
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    [ClientRpc]
    public void RpcCard_1006_GetRightSuspectedScore(List<int> list_index_offender)
    {
        UIPlayerManager.instance.Card_1006_GetRightSuspectedScore(list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_1007_CollectScoreCards(int index_offender)
    {
        if ((int)instance.netId != index_offender) return;
        instance.CmdCard_1005or1006_AddScoreCard((int)instance.netId, instance.scoreCard.GetComponent<ScoreCard>().score);
    }
    [ClientRpc]
    public void RpcCard_1007_ShowPanel(int id_attacker , List<int> list_score, List<int> list_id_of_score)
    {
        if ((int)instance.netId != id_attacker) return;
        UIManager.instance.UICard_1007_ShowPanel(list_score, list_id_of_score);
    }
    [ClientRpc]
    public void RpcCard_1008_ShowPanel(int id_attacker, int id_offender, int score)
    {
        UIManager.instance.UICard_1008_ShowPanel(id_attacker, id_offender, score);
    }
    [ClientRpc]
    public void RpcCard_1008_ClosePanel()
    {
        UIManager.instance.UICard_1008_ClosePanel();
    }
    [ClientRpc]
    public void RpcCard_2001and2002_ShowPanel(int index_Card, int id_turn, List<int> list_index_offender)
    {
        UIManager.instance.UICard_2001and2002_ShowPanel(index_Card, id_turn,id_turn, list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_2001and2002_NextTurn(int index_Card, int id_attacker,int id_turn, List<int> list_index_offender)
    {
        int index_id_turn = GetIndex_in_list_netId(id_turn);
        index_id_turn++;
        if (index_id_turn == list_netId.Count)
        {
            index_id_turn = 0;
        }
        UIManager.instance.UICard_2001and2002_NextTurn(index_Card, id_attacker, list_netId[index_id_turn], list_index_offender);
    }
    [ClientRpc]
    public void RpcCard_2001and2002_RefreshList(List<int> list_index_offender)
    {
        instance.temp_list_index_offender = list_index_offender;
        instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
    }
    [ClientRpc]
    public void RpcAddStateCard(int id_attacker, List<int> list_index_offender, int index_Card)
    {
        UIPlayerManager.instance.AddStateCard(id_attacker, list_index_offender, index_Card);
    }
    [ClientRpc]
    public void RpcStartGainScore(int index_Shown,List<int> list_score, List<int> list_id_of_score)
    {
        instance.index_Shown = index_Shown;
        instance.list_Card_1002_ScoreCard = list_score;
        instance.list_Card_1002_netId_ScoreCard = list_id_of_score;
        ClientGainScore();
    }
    
    [ClientRpc]
    public void RpcSetState(GameManager.Temp_STATE state)
    {
        GameManager.instance.state_ = state;
        Debug.Log(GameManager.instance.state_ = state);
    }
    [ClientRpc]
    public void RpcClearStatesOnNewCircle()
    {
        UIPlayerManager.instance.ClearStatesOnNewRound();
    }
    [ClientRpc]
    public void RpcClearAllSuspectedCardOnNewRound()
    {
        UIPlayerManager.instance.Card_1002_ClearAllSuspectedCard();
    }

    [Client]
    public void ClientAddPlayer(int added_netId,string added_name)
    {
        Debug.Log("[Client] ���� :" + added_netId + " " + added_name);
        instance.CmdAddPlayer(added_netId, added_name);
    }
    [Client]
    public void ClientStartGame()
    {
        instance.CmdStartGame();
    }
    [Client]
    public void ClientDiscardScoreCard(int index_ScoreCard)
    {
        instance.CmdDiscardScoreCard(index_ScoreCard);
    }
    [Client]
    public void ClientDiscardHandCard(int onlineID,int index)
    {
        instance.CmdDiscardHandCard(onlineID,index);
    }
    [Client]
    public void ClientDrawHandCards(int onlineID, int times)
    {
        instance.CmdDrawHandCards(onlineID, times);
    }
    [Client]
    public void ClientDrawHandCards_Specific(int onlineID, List<int> list_index_handCard)
    {
        instance.CmdDrawHandCards_Specific(onlineID, list_index_handCard);
    }
    [Client]
    public void ClientDrawScoreCard(int onlineID, bool canDiscard)
    {
        instance.CmdDrawScoreCard(onlineID,canDiscard);
    }
    [Client]
    public void ClientDrawScoreCard_Specific(int onlineID, int score)
    {
        //instance.CmdDrawScoreCard_Specific(onlineID,score);
    }
    [Client]
    public void ClientNewTurn()
    {
        instance.CmdNewTurn();
    }
    [Client]
    public void ClientYieldCard()
    {
        //GameManager.instance.state_ = GameManager.Temp_STATE.STATE_YIELD_CARDS;
        GameManager.instance.state_ = GameManager.Temp_STATE.STATE_BUSYCONNECTING;
        instance.CmdSetState(GameManager.Temp_STATE.STATE_BUSYCONNECTING);
        ClientDelay_AfterYieldCard();
    }
    [Client]
    public void ClientDelay_AfterYieldCard()
    {
        if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_BUSYCONNECTING)
        {
            Invoke(nameof(ClientDelay_AfterYieldCard), 0.3f);
            return;
        }
        instance.selectedCard.GetComponent<HandCard>().CloseDetail();
        if (instance.selectedCard.GetComponent<HandCard>().count_offender != 0)
        {
            UIPlayerManager.instance.Show_Button_Select();
        }
        else
        {
            GameManager.instance.state_ = GameManager.Temp_STATE.STATE_BUSYCONNECTING;
            instance.CmdSetState(GameManager.Temp_STATE.STATE_BUSYCONNECTING);
            instance.temp_list_index_offender.Clear();
            for (int i=0;i<list_netId.Count;i++)
            {
                instance.temp_list_index_offender.Add(i);
            }
            instance.CmdCheckCard_2001and2002(instance.selectedCard.GetComponent<HandCard>().index_Card, (int)instance.netId, instance.temp_list_index_offender);
            instance.ClientRealizeHandCard();
        }


        instance.count_MyHandCard--;
        instance.ClientDiscardHandCard((int)instance.netId, instance.selectedCard.GetComponent<HandCard>().index_Card);
        instance.selectedCard.SetActive(false);
    }
    [Client]
    public void ClientThrowCard()
    {
        instance.selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("�������" + instance.selectedCard.GetComponent<HandCard>().index_Card);
        instance.count_MyHandCard--;
        instance.ClientDiscardHandCard((int)instance.netId, instance.selectedCard.GetComponent<HandCard>().index_Card);
        Destroy(instance.selectedCard);
        instance.Client_ThrowCard_EndJudge((int)instance.netId);
    }
    [Client]
    public void Client_ThrowCard_EndJudge(int onlineID)
    {
        //if(onlineID != (int)instance.netId) { return; }
        Debug.Log("ʣ�������� = " + instance.count_MyHandCard);
        if (instance.count_MyHandCard <= 4)
        {
            instance.CmdSetState(GameManager.Temp_STATE.STATE_TURNING_TURN);
            instance.ClientNewTurn();
        }
    }
    [Client]
    public void ClientRealizeHandCard()
    {
        if(GameManager.instance.state_ != GameManager.Temp_STATE.STATE_YIELD_CARDS)
        {
            Debug.Log("GGG");
            Invoke(nameof(ClientRealizeHandCard), 1f);
            return;
        }
        //if (list_index_offender[0]!= -1)//���Ƿ���ѡ��
        //{
            switch (instance.selectedCard.GetComponent<HandCard>().index_Card)////��������
            {
                case 1001://����
                    Debug.Log("����");
                    instance.CmdGetHisAllHandCards((int)instance.netId,instance.temp_list_index_offender);
                    break;
                case 1002://���µ�һ���μ�
                    Debug.Log("���µ�һ���μ�");
                    instance.CmdCard_1002_CollectAllScoreCards((int)instance.netId, instance.temp_list_index_offender);
                    break;
                case 1003://ָ�㽭ɽ
                    Debug.Log("ָ�㽭ɽ");
                    instance.CmdCard_1003_GetHisScoreCard((int)instance.netId, instance.temp_list_index_offender,1003);
                    break;
                case 1004://�ۿ���Ԫ
                    Debug.Log("�ۿ���Ԫ");
                    instance.CmdCard_1004_GetHisScoreCard(instance.temp_list_index_offender);
                    break;
                case 1005://��֮����
                    Debug.Log("��֮����");
                    //instance.temp_list_index_offender.Clear();
                    //for (int i=0;i<list_netId.Count;i++)
                    //{
                    //    instance.temp_list_index_offender.Add(i);
                    //}
                    instance.CmdCard_1005_GetLeftSuspectedScore(instance.temp_list_index_offender);
                    instance.CmdCard_1005or1006_CollectAllScoreCards(instance.temp_list_index_offender,1005);
                    break;
                case 1006://��֮����
                    Debug.Log("��֮����");
                    //instance.temp_list_index_offender.Clear();
                    //for (int i = 0; i < list_netId.Count; i++)
                    //{
                    //    instance.temp_list_index_offender.Add(i);
                    //}
                    instance.CmdCard_1006_GetRightSuspectedScore(instance.temp_list_index_offender);
                    instance.CmdCard_1005or1006_CollectAllScoreCards(instance.temp_list_index_offender, 1006);
                    break;
                case 1007://������
                    Debug.Log("������");
                    //list_index_offender = new List<int> { 2 };
                    instance.CmdCard_1007_CollectScoreCards((int)instance.netId, instance.temp_list_index_offender);
                    break;
                case 1008://������
                    Debug.Log("������");
                    instance.CmdAddStateCard((int)instance.netId, instance.temp_list_index_offender,1008);
                    break;
                case 1009://����
                    Debug.Log("����");
                    instance.CmdCard_1005_GetLeftSuspectedScore(instance.temp_list_index_offender);
                    instance.CmdCard_1005or1006_CollectAllScoreCards(instance.temp_list_index_offender, 1005);
                break;
                case 1010://������
                    Debug.Log("������");
                    break;

                case 2001://���
                    Debug.Log("���");
                    break;
                case 2002://�������
                    Debug.Log("�������");
                    break;
                case 2003://�����ӳ�
                    Debug.Log("�����ӳ�");
                    break;

                case 3001://����
                    Debug.Log("����");
                    break;
                case 3002://˽�˶�����̨
                    Debug.Log("˽�˶�����̨");
                    break;
                case 3003://��������
                    Debug.Log("��������");
                    instance.ClientDrawHandCards((int)instance.netId, 2);
                    Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
                    break;
                case 3004://��ͷ��ʼ
                    Debug.Log("��ͷ��ʼ");
                    instance.ClientDrawScoreCard((int)instance.netId,true);
                    Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);
                    break;
            }
        //}
        //if(instance.selectedCard.GetComponent<HandCard>().index_Card !=1001)
        instance.ClientOnEndRealizeHandCard();



    }
    [Client]
    public void ClientOnEndRealizeHandCard()
    {
        if(GameManager.instance.state_ == GameManager.Temp_STATE.STATE_BUSYCONNECTING)
        {
            Debug.Log("BUSY");
            Invoke(nameof(ClientOnEndRealizeHandCard), delay);
            return;
        }
        //Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_YIELD_CARDS);

        int count_turn = instance.turnMove.Count;
        instance.turnMove[count_turn - 1]++;
        Debug.Log("instance.turnMove[count_turn - 1] =" + instance.turnMove[count_turn - 1]);
        //instance.totalMove++;
        if (instance.turnMove[count_turn - 1] >= 3)
        {
            Debug.Log("׼������");
            UIManager.instance.UIFinishYieldCard();
        }
        Destroy(instance.selectedCard);
    }
    [Client]
    public void ClientGiveMyAllHandCards(int id_attacker, List<int> list_index_handCard)//1001 ����
    {
        Debug.Log("#1001 ClientGiveMyAllHandCards Ӧ����ID = " + id_attacker);
        CmdGiveMyAllHandCards(id_attacker, list_index_handCard);
    }
    [Client]
    public void ClientClearAllHandCards(int onlineID)
    {
        instance.count_MyHandCard = 0;
        UIManager.instance.ClearAllHandCards();
        instance.CmdClearAllHandCards(onlineID);
    }
    [Client]
    public void ClientCard_1002_NextTurn(GameObject scoreCard)
    {
        ScoreCardManager.instance.Card_1002_ReGetScoreCard(scoreCard);
        CmdCard_1002_NextTurn((int)instance.netId, scoreCard.transform.GetSiblingIndex());
    }
    
    [Client]
    public void ClientGainScore()
    {
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_ADDING_SCORES);
        UIManager.instance.DiscloseScoreCard(instance.index_Shown, instance.list_Card_1002_ScoreCard,instance.list_Card_1002_netId_ScoreCard);
        instance.index_Shown++;
        if (instance.index_Shown == list_netId.Count)
        {
            instance.index_Shown = 0;
        }
        if (instance.index_Shown == index_CurrentHolder - 1)
        {
            Invoke(nameof(ClientDelay_EndGainScore), 3f);
            return;
        }
        Invoke(nameof(ClientGainScore), 3f);
    }
    [Client]
    public void ClientDelay_EndGainScore()
    {
        Empty.instance.CmdSetState(GameManager.Temp_STATE.STATE_TURNING_ROUND);
        UIManager.instance.EndDiscloseScoreCard();
    }


    public bool CheckRepeatedNetId(int checked_netId)
    {
        for(int i=0;i<list_netId.Count;i++)
        {
            if (list_netId[i] == checked_netId) return true;
        }
        return false;
    }
    public string GetContent_int(List<int> list)
    {
        string a="";
        for(int i=0;i< list.Count;i++)
        {
            a += list[i].ToString();
            a += " ";
        }
        return a;
    }
    public string GetContent_string(List<string> list)
    {
        string a = "";
        for(int i = 0; i < list.Count; i++)
        {
            a += list[i].ToString();
            a += " ";
        }
        return a;
    }
    public int GetIndex_in_list_netId(int netId)
    {
        for(int i=0;i<list_netId.Count;i++)
        {
            if(netId == list_netId[i]) return i;
        }
        return -1;
    }
    public void Call_ClientCard_1002_NextTurn(GameObject scoreCard)
    {
        instance.ClientCard_1002_NextTurn(scoreCard);
    }
    public List<T> RandomList<T>(List<T> inList)
    {
        List<T> newList = new List<T>();
        int count = inList.Count;
        for (int i = 0; i < count; i++)
        {
            int temp = UnityEngine.Random.Range(0, inList.Count - 1);
            T tempT = inList[temp];
            newList.Add(tempT);
            inList.Remove(tempT);
        }
        //�����һ��Ԫ�����������
        T tempT2 = newList[newList.Count - 1];
        newList.RemoveAt(newList.Count - 1);
        newList.Insert(UnityEngine.Random.Range(0, newList.Count), tempT2);
        inList = newList;
        return inList;
    }







    /*
    [Command]
    public void CmdAddPlayer(int added_netId)
    {
        Debug.Log("[Server] CmdAddPlayer()");
        ServerAddPlayer(added_netId);
    }
    [Command]//��������������������������������������������������������������������������������������������������������������������������������������
    public void CmdRemovePlayer(int added_netId)
    {
        //Debug.Log(NetworkClient.ready + " " + NetworkClient.active + "" + NetworkClient.isConnected);
        //NetworkClient.ready = true;
        //NetworkClient.connectState = ConnectState.Connected;
        //Debug.Log(NetworkClient.ready + " " + NetworkClient.active + "" + NetworkClient.isConnected);

        //if(!isServer)//����������������������������������������������������������������������������������������������������������������������
        //{
        //    return;
        //}
        //Debug.Log("isServer = " + Empty.list_instance.isServer);
        //Debug.Log("isClient = " + Empty.list_instance.isClient);


        Debug.Log("[Server] CmdRemovePlayer()");
        ServerRemovePlayer(added_netId);
    }
    [Server]
    public void ServerAddPlayer(int added_netId)
    {
        Debug.Log("[Server] ServerAddPlayer()");
        UIPlayerManager.list_netId.Add(added_netId);
        UIPlayerManager.instance.RefreshPlayer();
        //Debug.Log("isClient = " + isClient);
        //Debug.Log("isServer = " + isServer);
        RpcAddPlayer(UIPlayerManager.list_netId);
    }
    [Server]
    public void ServerRemovePlayer(int added_netId)
    {
        Debug.Log("[Server] ServerRemovePlayer()");
        int i = 0;
        for (; i < UIPlayerManager.list_netId.Count; i++)
        {
            if (UIPlayerManager.list_netId[i] == added_netId) break;
        }
        if (i == UIPlayerManager.list_netId.Count)
        {
            Debug.LogError("δ�ҵ�id��ɾ��!!");
            return;
        }
        //Debug.Log("i = " + i);
        UIPlayerManager.list_netId.RemoveAt(i);
        UIPlayerManager.instance.RefreshPlayer();
        
        Debug.Log("ǰ RpcRemovePlayer(i)--- Empty.instance = " + Empty.instance.name);
        TEST_2();
        RpcRemovePlayer(UIPlayerManager.list_netId);
        Debug.Log("�� RpcRemovePlayer(i)--- Empty.instance = " + Empty.instance.name);
    }
    [ClientRpc]
    public void TEST_2()
    {
        Debug.Log("TEST_2");
    }
    [ClientRpc]
    public void RpcAddPlayer(List<int> list_netId)
    {

        Debug.Log("[Client] RpcAddPlayer()");
        UIPlayerManager.instance.ClearPlayer();
        UIPlayerManager.list_netId = list_netId;
        //if (!temp_netID) Debug.LogError("TNND ");
        //if(!temp_player)
        //{
        //    Debug.Log("DON'T have temp_player");
        //    Invoke(nameof(RpcAddPlayer), delay);
        //    return;
        //}
        UIPlayerManager.instance.RefreshPlayer();

    }


    [ClientRpc]
    public void RpcRemovePlayer(List<int> list_netId)
    {
        //if (!temp_netID) Debug.LogError("TNND ");
        Debug.Log("[Client] RpcRemovePlayer()");
        UIPlayerManager.list_netId = list_netId;
        //if(!temp_player)
        //{
        //    Debug.Log("DON'T have temp_player");
        //    Invoke(nameof(RpcAddPlayer), delay);
        //    return;
        //}
        UIPlayerManager.instance.RefreshPlayer();
        Debug.Log("list_netId.Count = " + UIPlayerManager.list_player.Count);


        ////MyNetworkManager.instance.My_OnServerDisconnect();
        //MyNetworkManager.instance.base_OnClientDisconnect();
        //MyNetworkManager.instance.base_OnStopClient();
        //MyNetworkManager.instance.base_OnServerDisConnect(conn);

        //MyNetworkManager.instance.NonePara_base_OnServerDisConnect();
        //////////////////////////////////////////////////
    }
    */
}
