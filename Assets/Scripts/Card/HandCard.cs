using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCard : GrandCard
{
    public bool isAttackCard;//������(A)����
    public bool isExchangeCard;//������(E)����
    public bool isTimingCard;//��ʱ��(T)����
    public GameObject panel_HandCardDetail;
    public Image image_HandCard;
    
    public HandCard(int index, int count,bool isAttackCard, bool isExchangeCard, bool isTimingCard)
    {
        base.index_Card = index;
        this.grossCount = count;
        this.isAttackCard = isAttackCard;
        this.isExchangeCard = isExchangeCard;
        this.isTimingCard = isTimingCard;
    }

    public void ShowDetail()
    {
        panel_HandCardDetail.SetActive(true);
        image_HandCard.sprite = gameObject.GetComponent<Image>().sprite;
    }


}
