using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandCard : MonoBehaviour
{
    public int index_Card;//������ͼ������ţ������ظ�
    public int grossCount;//��һ���Ƶ�����
    public int usedRound;//��ʹ�õĻغ�
    public List<int> index_attacker = new List<int>();//����������б�
    public List<int> index_offender = new List<int>();//�ܻ�������б�
}
