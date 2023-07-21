using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCard : GrandCard
{
    public bool isAttackCard;//������(A)����
    public bool isExchangeCard;//������(E)����
    public bool isTimingCard;//��ʱ��(T)����
    public int count_offender;//�ܻ���Ӧ���е�����
    public GameObject panel_HandCardDetail;
    public GameObject panel_HandCardDetail_ReadOnly;
    public GameObject panel_New;
    public Image image_HandCard;
    public Image image_HandCard_ReadOnly;
    public List<int> index_attacker = new List<int>();////����� ����������б�
    public List<int> index_offender = new List<int>();////����� �ܻ�������б�
    public HandCard(int index, int count,bool isAttackCard, int count_offender, bool isExchangeCard, bool isTimingCard)
    {
        base.index_Card = index;
        this.grossCount = count;
        this.isAttackCard = isAttackCard;
        this.count_offender = count_offender;
        this.isExchangeCard = isExchangeCard;
        this.isTimingCard = isTimingCard;
    }

    public void ShowDetail()
    {
        panel_New.SetActive(false);
        if ( GameManager.instance.state_ == GameManager.Temp_STATE.STATE_YIELD_CARDS || GameManager.instance.state_ == GameManager.Temp_STATE.STATE_THROW_CARDS)
        {
            Empty.instance.selectedCard = gameObject;
        }


        if(gameObject.GetComponent<GrandCard>().used == true)//���ù�
        {
            panel_HandCardDetail_ReadOnly.SetActive(true);
            panel_HandCardDetail.SetActive(false);
            image_HandCard_ReadOnly.sprite = gameObject.GetComponent<Image>().sprite;
        }
        else
        {
            panel_HandCardDetail_ReadOnly.SetActive(false);
            panel_HandCardDetail.SetActive(true);
            image_HandCard.sprite = gameObject.GetComponent<Image>().sprite;
            UIPlayerManager.instance.ShowOrHide_OtherItems(false, -1);
            int index_Card = Empty.instance.selectedCard.GetComponent<HandCard>().index_Card;
            if ( (index_Card > 2000) && (index_Card < 3000))
            {
                UIManager.instance.panel_NoticeDefendCard.SetActive(true);
            }
            else
            {
                UIManager.instance.panel_NoticeDefendCard.SetActive(false);
            }
            UIPlayerManager.instance.ShowOrHide_OtherItems(true, index_Card);
        }

    }
    public void CloseDetail()
    {
        panel_HandCardDetail.SetActive(false);
        panel_HandCardDetail_ReadOnly.SetActive(false);
        UIPlayerManager.instance.ShowOrHide_OtherItems(false,-1);
    }
}
