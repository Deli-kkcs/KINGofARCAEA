using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCardManager : MonoBehaviour
{
    public static ScoreCardManager instance;
    public int count_ScoreCard;//���������������ظ���
    public List<ScoreCard> scoreCards_info;//����+�����б����ظ���
    public List<GameObject> scoreCardsPrefab = new List<GameObject>();//Ԥ�����б����ظ���
    public static List<GameObject> scoreCardsStock = new List<GameObject>();//����ֿ⣨�ظ���

    public static List<int> list_index = new(); 

    public Text text_CardNum;

    public GameObject panel_MyScoreCard;
    void Awake()
    {
        instance = this;
        scoreCards_info = new List<ScoreCard>()
        {
            new ScoreCard(1,3,6),//�����ո�
            new ScoreCard(2,1,5),//����ȫ��
            new ScoreCard(3,0,5),//�ź��볡
        };
    }
    void Start()
    {
        for (int i = 0; i < scoreCards_info.Count; i++)////������������
        {
            scoreCardsPrefab[i].GetComponent<ScoreCard>().index_Card = scoreCards_info[i].index_Card;
            scoreCardsPrefab[i].GetComponent<ScoreCard>().grossCount = scoreCards_info[i].grossCount;
            scoreCardsPrefab[i].GetComponent<ScoreCard>().score = scoreCards_info[i].score;
        }
    }


    public void RefreshScoreCards(List<int>list)
    {
        count_ScoreCard = list.Count;
        scoreCardsStock.Clear();
        for(int i=0;i<list.Count;i++)
        {
            int index = -1;
            for(int j=0;j< scoreCardsPrefab.Count;j++)
            {
                if (list[i] == scoreCardsPrefab[j].GetComponent<ScoreCard>().index_Card)
                {
                    index = j;
                    break;
                }
            }
            scoreCardsStock.Add(scoreCardsPrefab[index]);
        }
        text_CardNum.text = count_ScoreCard.ToString();
    }
    public void RefillScoreCards()
    {
        count_ScoreCard = 0;
        for (int i = 0; i < scoreCards_info.Count; i++)//������
        {
            count_ScoreCard += scoreCards_info[i].grossCount;//һ�������ظ���
        }
        scoreCardsStock.Clear();
        for (int i = 0; i < scoreCards_info.Count; i++)//������
        {
            for (int j = 0; j < scoreCards_info[i].grossCount; j++)//һ�������ظ���
            {
                scoreCardsStock.Add(scoreCardsPrefab[i]);
            }
        }
        text_CardNum.text = count_ScoreCard.ToString();
        scoreCardsStock = RandomList(scoreCardsStock);


        list_index.Clear();
        for(int i=0;i<scoreCardsStock.Count;i++)
        {
            list_index.Add(scoreCardsStock[i].GetComponent<ScoreCard>().index_Card);
        }
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

    public void Sync_DrawOneCard()
    {
        count_ScoreCard -= 1;
        text_CardNum.text = count_ScoreCard.ToString();
        //
        //
        //
        scoreCardsStock.RemoveAt(0);
    }
    public void DrawOneCard()
    {
        count_ScoreCard -= 1;
        text_CardNum.text = (int.Parse(text_CardNum.text) - 1).ToString();
        //if (Empty.instance.scoreCard)
        //{
        //    UIManager.instance.CallClient_UIDiscardCard(Empty.instance.scoreCard.gameObject);
        //    //Destroy(UIPlayerManager.list_player[index].GetComponent<Player>().scoreCard);
        //}
        Debug.Log(scoreCardsStock[0].GetComponent<ScoreCard>().index_Card);
        //Debug.Log(scoreCardsStock[0].gameObject.name);
        
        Empty.instance.scoreCard = Instantiate(scoreCardsStock[0].gameObject, panel_MyScoreCard.transform);
        Empty.instance.scoreCard.SetActive(true);
        //UIPlayerManager.list_player[index].GetComponent<Player>().scoreCard.gameObject.SetActive(true);
        scoreCardsStock.RemoveAt(0);/////�жϿ�
    }
}
