using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCardManager : MonoBehaviour
{
    public int count_ScoreCard;//���������������ظ���
    public List<ScoreCard> scoreCardsCatagory;//����+�����б����ظ���
    public List<ScoreCard> scoreCardsPrefab = new List<ScoreCard>();//Ԥ�����б����ظ���
    public List<ScoreCard> scoreCardsStock = new List<ScoreCard>();//����ֿ⣨�ظ���
    void Awake()
    {
        scoreCardsCatagory = new List<ScoreCard>()
        {
            new ScoreCard(1,3,6),
            new ScoreCard(2,1,5),
            new ScoreCard(3,0,5),
        };
    }
    void Start()
    {
        for(int i=0;i<scoreCardsCatagory.Count;i++)//������
        {
            count_ScoreCard += scoreCardsCatagory[i].grossCount;//һ�������ظ���
        }
        for(int i=0;i< scoreCardsCatagory.Count; i++)//������
        {
            for (int j = 0; j < scoreCardsCatagory[i].grossCount;j++)//һ�������ظ���
            {
                scoreCardsStock.Add(scoreCardsPrefab[scoreCardsCatagory[i].index_Card - 1]);
            }
        }

    }
}
