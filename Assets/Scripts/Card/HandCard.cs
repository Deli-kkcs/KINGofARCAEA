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
    public List<int> index_attacker = new List<int>();////����� ����������б�
    public List<int> index_offender = new List<int>();////����� �ܻ�������б�
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
        PlayerManager.list_player[PlayerManager.index_CurrentPlayer - 1].selectedCard = gameObject;
        panel_HandCardDetail.SetActive(true);
        image_HandCard.sprite = gameObject.GetComponent<Image>().sprite;
        UIManager.index_SelectedHandCard = gameObject.GetComponent<HandCard>().index_Card;
        UIManager.index_Card_In_Hand = gameObject.transform.GetSiblingIndex();
    }
    public void CloseDetail()
    {
        panel_HandCardDetail.SetActive(false);
    }
    
}
