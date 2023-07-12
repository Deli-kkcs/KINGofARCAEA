using Mirror;
//using System;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;
using UnityEngine.UI;

public class Empty : NetworkBehaviour
{
    public static Empty instance;

    public static List<int> list_netId = new();//���Id�б�
    public static List<string> list_playerName = new();//��������б�


    /// <summary>
    /// Empty <- GameManager
    /// </summary>
    public enum Temp_STATE
    {
        STATE_GAME_IDLING,
        STATE_GAME_STARTED,
        STATE_GAME_SUMMARY,
        STATE_DRAW_CARDS,
        STATE_JUDGE_CARDS,
        STATE_YIELD_CARDS,
        STATE_THROW_CARDS
    }
    public static Temp_STATE state_ = Temp_STATE.STATE_GAME_IDLING;
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
    public int index_in_listId;
    //public int totalScore;
    public List<int> roundScore = new();////�����
    public int totalMove;
    public List<int> turnMove = new();////�����
    //public int count_HandCard;
    //public int count_RoundUsedCard;
    //public int count_TotalUsedCard;

    public GameObject selectedCard;
    public GameObject scoreCard;
    //public List<GameObject> handCards = new();


    public float delay = 0.2f;


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
    public static STATE state = STATE.STATE_GAME_IDLING;

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
    public void CmdAddPlayer(int added_netId,string added_name)
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
    public void CmdDiscardScoreCard(int index)
    {
        RpcDiscardScoreCard(index);
    }
    [Command]
    public void CmdDiscardHandCard(int onlineID,int index)
    {
        RpcDiscardHandCard(onlineID,index);
    }
    [Command]
    public void CmdDrawHandCards(int onlineID, int times)
    {
        RpcDrawHandCards(onlineID, times);
    }
    [Command]
    public void CmdDrawScoreCard(int onlineID)
    {
        RpcDiscardScoreCard(onlineID);
    }
    [Command]
    public void CmdNewTurn()
    {
        ServerNewTurn();
    }
    [Command]
    public void CmdYieldCard()
    {
        RpcYieldCard();
    }
    [Command]
    public void CmdThrowCard()
    {
        RpcThrowCard();
    }
    [Server]
    public void ServerAddPlayer(int added_netId, string added_name)
    {
        if(CheckRepeatedNetId(added_netId))
        {
            Debug.Log("id�ظ���");
            return;
        }
        //Debug.Log("ServerAddPlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (added_netId == 1) return;
        instance.index_in_listId = list_netId.Count;
        list_netId.Add(added_netId);
        
        list_playerName.Add(added_name);

        

        RpcClearPlayer();
        for(int i=0;i<list_netId.Count;i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]);
        }
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
        Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
    }
    [Server]
    public void ServerRomovePlayer(int removed_netId)
    {
        //Debug.Log("ServerRomovePlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        if (removed_netId == 1) return;
        int removed_index = -1;
        for(int i=0;i<list_netId.Count;i++)
        {
            if (list_netId[i] == removed_netId)
            {
                removed_index = i;
                break;
            }
        }
        if(removed_index == -1)
        {
            Debug.LogError("δ�ҵ�ID��ɾ��!!");
            return;
        }



        Debug.Log("[Server] �뿪 " + removed_netId + " " + list_playerName[removed_index]);
        list_netId.RemoveAt(removed_index);
        list_playerName.RemoveAt(removed_index);
        Debug.Log("[Server] list_netId = " + GetContent_int(list_netId));
        Debug.Log("[Server] list_playerName = " + GetContent_string(list_playerName));
        RpcClearPlayer();
        for (int i = 0; i < list_netId.Count; i++)
        {
            RpcAddPlayer(list_netId[i], list_playerName[i]) ;
        }

        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
        RpcRefreshPlayer();
    }
    [Server]
    public void ServerStartGame()
    {
        state = STATE.STATE_GAME_STARTED;
        index_Round = 0;
        init_draw_num = 4;
        isSwitchHolder = false;
        index_CurrentHolder = index_CurrentPlayer = 0;

        ScoreCardManager.instance.RefillScoreCards();
        HandCardManager.instance.RefillHandCards();
        RpcInitialize(ScoreCardManager.list_index,HandCardManager.list_index);

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
            RpcSetHolder(index_CurrentHolder-1,true);

            for (int i = 0; i < list_netId.Count; i++)
            {
                RpcDrawScoreCard(list_netId[i]);////�п�
                RpcDrawHandCards(list_netId[i], init_draw_num);////�п�
            }

        }
        else
        {
            RpcSetHolder(index_CurrentHolder - 1, false);
            index_CurrentHolder++;
            if (index_CurrentHolder > list_netId.Count)
            {
                ///////SummaryGame();
                return;
            }
            RpcSetHolder(index_CurrentHolder - 1, true);
        }

        ServerNewTurn();
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
                
            }
            if (index_Circle == 3)
            {
                ServerNewRound();
                return;
            }
            RpcSetCircleNum(index_Circle);
        }
        RpcMyTurn(index_CurrentPlayer - 1);
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
        Debug.Log("[Client] list_netId = " + GetContent_int(list_netId));
    }
    [ClientRpc]
    public void RpcRefreshPlayer()
    {
        UIPlayerManager.instance.RefreshPlayer(list_netId, list_playerName);
    }
    [ClientRpc]
    public void RpcInitialize(List<int> list_index_ScoreCards,List<int> list_index_HandCards)
    {
        instance.roundScore.Clear();
        instance.turnMove.Clear();
        totalMove = 0;
        for (int j = 0; j < list_netId.Count; j++)
        {
            instance.roundScore.Add(0);
        }
        ScoreCardManager.instance.RefreshScoreCards(list_index_ScoreCards);
        HandCardManager.instance.RefreshHandCards(list_index_HandCards);
        //GameCardManager.list_instance.RefillHandCards();
        
        


    }
    [ClientRpc]
    public void RpcDrawScoreCard(int onlineID)
    {
        if (instance.netId !=onlineID)
        {
            ScoreCardManager.instance.Sync_DrawOneCard();
        }
        else
        {
            ScoreCardManager.instance.DrawOneCard();
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
        //        index_Card = i;
        //        break;
        //    }
        //}
        for (int j = 0; j < times;j++)
        {
            if (instance.netId != onlineID)
            {
                HandCardManager.instance.Sync_DrawOneCard();
            }
            else
            {
                HandCardManager.instance.DrawOneCard();
            }
            UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) + 1).ToString();
        }
    }
    [ClientRpc]
    public void RpcDiscardScoreCard(int index_Card)
    {
        UIManager.instance.DiscardScorecard(index_Card,new Vector2(Random.Range(-500f, 500f), Random.Range(-200f, 300f)), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
    }
    [ClientRpc]
    public void RpcDiscardHandCard(int onlineID,int index_Card)
    {
        int index_player = GetIndex_in_list_netId((int)onlineID);
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
        GameManager.state_ = GameManager.Temp_STATE.STATE_YIELD_CARDS;
        if (list_netId[index] == instance.netId)
        {
            UIPlayerManager.instance.MyTurn(index);
        }
        else
        {
            UIPlayerManager.instance.Sync_MyTurn(index);
        }
    }
    [ClientRpc]
    public void RpcSetCircleNum(int num_circle)
    {
        UIManager.instance.text_CircleNum.text = num_circle.ToString();
    }
    [ClientRpc]
    public void RpcYieldCard()
    {
        int index = GetIndex_in_list_netId((int)instance.netId);
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("������" + selectedCard.GetComponent<HandCard>().index_Card);
        UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) - 1).ToString();

        switch (selectedCard.GetComponent<HandCard>().index_Card)////��������
        {
            case 1001://����
                Debug.Log("����");
                break;
            case 1002://���µ�һ���μ�
                Debug.Log("���µ�һ���μ�");
                break;
            case 1003://ָ�㽭ɽ
                Debug.Log("ָ�㽭ɽ");
                break;
            case 1004://�ۿ���Ԫ
                Debug.Log("�ۿ���Ԫ");
                break;
            case 1005://��֮����
                Debug.Log("��֮����");
                break;
            case 1006://��֮����
                Debug.Log("��֮����");
                break;
            case 1007://������
                Debug.Log("������");
                break;
            case 1008://������
                Debug.Log("������");
                break;
            case 1009://����
                Debug.Log("����");
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
                ClientDrawHandCards((int)instance.netId, 2);
                //DrawHandCards(2, UIPlayerManager.index_CurrentPlayer - 1);
                break;
            case 3004://��ͷ��ʼ
                Debug.Log("��ͷ��ʼ");
                ClientDrawScoreCards((int)instance.netId);
                //DrawScoreCards(1, UIPlayerManager.index_CurrentPlayer - 1);
                break;
        }

        int count_turn = turnMove.Count;
        turnMove[count_turn -1]++;
        totalMove++;
        if (turnMove[count_turn - 1] >= 3)
        {
            UIManager.instance.UIFinishYieldCard();
        }

        ClientDiscardHandCard((int)instance.netId,selectedCard.GetComponent<HandCard>().index_Card);
        Destroy(selectedCard);
    }
    [ClientRpc]
    public void RpcThrowCard()
    {
        int index = GetIndex_in_list_netId((int)instance.netId);
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("�������" + selectedCard.GetComponent<HandCard>().index_Card);
        UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
        ClientDiscardHandCard((int)instance.netId, selectedCard.GetComponent<HandCard>().index_Card);
        Destroy(selectedCard);
        Client_ThrowCard_Judge();
    }
    [Client]
    public void ClientAddPlayer(int added_netId,string added_name)
    {
        //Debug.Log("ClientAddPlayer  netId = " + netId + " || instance.netId = " + instance.netId);
        Debug.Log("[Client] ���� :" + added_netId + " " + added_name);
        CmdAddPlayer(added_netId, added_name);
    }
    [Client]
    public void ClientStartGame()
    {
        CmdStartGame();
    }
    [Client]
    public void ClientDiscardScoreCard(int index)
    {
        CmdDiscardScoreCard(index);
    }
    [Client]
    public void ClientDiscardHandCard(int onlineID,int index)
    {
        CmdDiscardHandCard(onlineID,index);
    }
    [Client]
    public void ClientDrawHandCards(int onlineID, int times)
    {
        CmdDrawHandCards(onlineID, times);
    }
    [Client]
    public void ClientDrawScoreCards(int onlineID)
    {
        CmdDrawScoreCard(onlineID);
    }
    
    public void ClientNewTurn()
    {
        CmdNewTurn();
    }
    [Client]
    public void ClientYieldCard()
    {
        //CmdYieldCard();
        int index = GetIndex_in_list_netId((int)instance.netId);
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("������" + selectedCard.GetComponent<HandCard>().index_Card);
        switch (selectedCard.GetComponent<HandCard>().index_Card)////��������
        {
            case 1001://����
                Debug.Log("����");
                break;
            case 1002://���µ�һ���μ�
                Debug.Log("���µ�һ���μ�");
                break;
            case 1003://ָ�㽭ɽ
                Debug.Log("ָ�㽭ɽ");
                break;
            case 1004://�ۿ���Ԫ
                Debug.Log("�ۿ���Ԫ");
                break;
            case 1005://��֮����
                Debug.Log("��֮����");
                break;
            case 1006://��֮����
                Debug.Log("��֮����");
                break;
            case 1007://������
                Debug.Log("������");
                break;
            case 1008://������
                Debug.Log("������");
                break;
            case 1009://����
                Debug.Log("����");
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
                ClientDrawHandCards((int)instance.netId, 2);
                //DrawHandCards(2, UIPlayerManager.index_CurrentPlayer - 1);
                break;
            case 3004://��ͷ��ʼ
                Debug.Log("��ͷ��ʼ");
                ClientDrawScoreCards((int)instance.netId);
                //DrawScoreCards(1, UIPlayerManager.index_CurrentPlayer - 1);
                break;
        }

        int count_turn = turnMove.Count;
        turnMove[count_turn - 1]++;
        totalMove++;
        if (turnMove[count_turn - 1] >= 3)
        {
            UIManager.instance.UIFinishYieldCard();
        }

        ClientDiscardHandCard((int)instance.netId, selectedCard.GetComponent<HandCard>().index_Card);
        Destroy(selectedCard);
    }
    [Client]
    public void ClientThrowCard()
    {
        //CmdThrowCard();
        int index = GetIndex_in_list_netId((int)instance.netId);
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("�������" + selectedCard.GetComponent<HandCard>().index_Card);
        ClientDiscardHandCard((int)instance.netId, selectedCard.GetComponent<HandCard>().index_Card);
        Destroy(selectedCard);
        Client_ThrowCard_Judge();
    }
    [Client]
    public void Client_ThrowCard_Judge()
    {

        int index = GetIndex_in_list_netId((int)instance.netId);
        Debug.Log("ʣ�������� = " + UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text);
        if (int.Parse(UIPlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) <= 4)
        {
            
            ClientNewTurn();
        }
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
