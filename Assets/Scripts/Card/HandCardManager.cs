using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardManager : MonoBehaviour
{
    public int count_HandCard;//�������������ظ���
    public List<HandCard> handCardsCatagory;//����+�����б����ظ���
    public List<HandCard> handCardsPrefab = new List<HandCard>();//Ԥ�����б����ظ���
    public List<HandCard> handCardsStock = new List<HandCard>();//����ֿ⣨�ظ���
    void Awake()
    {
        handCardsCatagory = new List<HandCard>()
        {
            new HandCard(1,2,false,false),
        };

    }
    void Start()
    {
       
        for (int i = 0; i < handCardsCatagory.Count; i++)//������
        {
            count_HandCard += handCardsCatagory[i].grossCount;//һ�������ظ���
        }
        for (int i = 0; i < handCardsCatagory.Count; i++)//������
        {
            //Debug.Log("i = " + i + "scoreCardsCatagory[i].count = " + scoreCardsCatagory[i].count);
            for (int j = 0; j < handCardsCatagory[i].grossCount; j++)//һ�������ظ���
            {
                //Debug.Log("j = " + j);
                handCardsStock.Add(handCardsPrefab[handCardsCatagory[i].index_Card - 1]);
            }
        }
    }
}
