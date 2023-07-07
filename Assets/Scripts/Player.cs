using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class Player : NetworkBehaviour
{
    public static Player instance;
    public int my_netID;
    public int listID;
    public int totalScore;
    public List<int> roundScore = new List<int>();////�����
    public int totalMove;
    public List<int> turnMove = new List<int>();////�����
    
    public int count_HandCard;
    public int count_RoundUsedCard;
    public int count_TotalUsedCard;
    public int index_Player;
    public Text text_Index_Player;
    public string name_Player;
    public Text text_Name_Player;
    public Text Text_CardNum;
    public GameObject image_Holder;
    public GameObject image_MyTurn;
    public GameObject selectedCard;
    public GameObject scoreCard;
    public Player(int index_Player, string name_player)
    {
        totalScore = totalMove = 0;
        count_HandCard = 0;
        count_RoundUsedCard = 0;
        count_TotalUsedCard = 0;
        this.index_Player = index_Player;
        this.name_Player = name_player;
    }
    private void Awake()
    {
        instance = this;
    }

    public void DrawHandCards(int num,int index)//0��ʼ
    {
        Debug.Log("index " + index + " draw " + num + "hand cards");
        for(int i=0;i<num;i++)
        {
            HandCardManager.instance.DrawOneCard(index);
            PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text =  (int.Parse(PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text)+1).ToString();
        }
    }
    public void DrawScoreCards(int num, int index)//0��ʼ
    {
        Debug.Log("index " + index + " draw " + num + "score cards");
        for (int i = 0; i < num; i++)
        {
            ScoreCardManager.instance.DrawOneCard(index);
        }
    }
    public void YieldCard(int index)
    {
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("������" + selectedCard.GetComponent<HandCard>().index_Card);
        PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
        
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
                DrawHandCards(2, PlayerManager.index_CurrentPlayer - 1);
                break;
            case 3004://��ͷ��ʼ
                Debug.Log("��ͷ��ʼ");
                DrawScoreCards(1, PlayerManager.index_CurrentPlayer - 1);
                break;
        }

        int count_turn = PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().turnMove.Count;
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().turnMove[count_turn - 1]++;
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().totalMove++;
        if (PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].GetComponent<Player>().turnMove[count_turn - 1] >= 3)
        {
            UIManager.instance.UIFinishYieldCard();
        }

        UIManager.instance.DiscardCard(selectedCard);
        Destroy(selectedCard);
    }
    
    public void ThrowCard_Judge(int index)
    {
        if(int.Parse(Text_CardNum.text) <= 4)
        {
            GameManager.instance.NewTurn();
        }
    }
    public void ThrowCard(int index)
    {
        selectedCard.GetComponent<HandCard>().CloseDetail();
        Debug.Log("�������" + selectedCard.GetComponent<HandCard>().index_Card);
        PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text = (int.Parse(PlayerManager.list_player[index].GetComponent<Player>().Text_CardNum.text) - 1).ToString();
        UIManager.instance.DiscardCard(selectedCard);
        Destroy(selectedCard);
        ThrowCard_Judge(index);
    }
}
