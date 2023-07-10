using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
public class UIPlayerManager : MonoBehaviour
{
    public static UIPlayerManager instance;
    //public static List<Player> list_player_info = new List<Player>();
    public static List<GameObject> list_player = new();
    public GameObject playerPrefab;
    public static int index_CurrentPlayer = 0;//��1��ʼ��
    public static int index_CurrentHolder = 0;//��1��ʼ��

    public GameObject content_Player;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        //list_player_info.Clear();
        //list_player_info.Add(new Player(1, "Andy"));
        //list_player_info.Add(new Player(2, "Bob"));
        //list_player_info.Add(new Player(3, "F**k"));
    }

    //public void Initialize()
    //{
        //index_CurrentHolder = index_CurrentPlayer = 0;
        //list_player.Clear();
        //ClearChild(content_Player.transform);
        
        //for (int i = 0 ; i < GameManager.instance.count_Player ; i++)
        //{
            //list_player.Add(playerPrefab);
            //list_player[i].GetComponent<Player>().index_Player = i + 1;
            //list_player[i].GetComponent<Player>().text_Index_Player.text = list_player_info[i].index_Player.ToString();
            //list_player[i].GetComponent<Player>().text_Name_Player.text = list_player_info[i].name_Player.ToString();
            //list_player[i].GetComponent<Player>().roundScore.Clear();
            //for (int j = 0; j < GameManager.instance.count_Player; j++)
            //{
            //    list_player[i].GetComponent<Player>().roundScore.Add(0);
            //}
            //list_player[i] = Instantiate(list_player[i],content_Player.transform);
        //}
    //}
    public void ClearChild(Transform t_parent)
    {
        Transform t_child;
        for (int i = 0; i < t_parent.transform.childCount; i++)
        {
            t_child = t_parent.transform.GetChild(i);
            Destroy(t_child.gameObject);
        }
    }

    public void RefreshPlayer(List<int> list_netId,List<string> list_name)
    {
        //Transform t_parent;
        //for (int i = 0; i < content_Player.transform.childCount; i++)
        //{
        //    t_parent = content_Player.transform.GetChild(i);
        //    Destroy(t_parent.gameObject);
        //}


        for (int i = 0; i< content_Player.transform.childCount; i++)
        {
            Destroy(content_Player.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < list_netId.Count; i++)
        {
            playerPrefab.GetComponent<Player>().my_netID = list_netId[i];
            playerPrefab.GetComponent<Player>().text_Index_Player.text = (i+1).ToString();
            playerPrefab.GetComponent<Player>().name_Player = list_name[i];
            playerPrefab.GetComponent<Player>().text_Name_Player.text = list_name[i];
            Instantiate(playerPrefab, content_Player.transform);
        }
    }

    //public void RemovePlayer(int netID)
    //{
    //    for(int i=0;i<list_player.Count;i++)
    //    {
    //        if (list_player[i].my_netID == netID)
    //        {
    //            //Debug.Log("ɾ��id" + netID);
    //            //Destroy(list_player[i].gameObject);
    //            Destroy(content_Player.t_parent.GetChild(i).gameObject);
    //            list_player.RemoveAt(i);
    //            return;
    //        }
    //    }
    //    Debug.LogError("û�ҵ���Ӧid��ɾ����ң���");
    //}
    public void PassTurn()
    {
        //HandCardManager.list_Scroll_MyHandCard[index_CurrentPlayer - 1].SetActive(false);
        list_player[index_CurrentPlayer - 1].GetComponent<Player>().image_MyTurn.SetActive(false);
    }
    public void MyTurn()
    {
        //HandCardManager.list_Scroll_MyHandCard[index_CurrentPlayer - 1].SetActive(true);
        list_player[index_CurrentPlayer - 1].GetComponent<Player>().image_MyTurn.SetActive(true);
        GameManager.state_ = GameManager.STATE.STATE_DRAW_CARDS;
        list_player[index_CurrentPlayer - 1].GetComponent<Player>().DrawHandCards(2, index_CurrentPlayer - 1);
        GameManager.state_ = GameManager.STATE.STATE_YIELD_CARDS;
        list_player[index_CurrentPlayer - 1].GetComponent<Player>().turnMove.Add(0);
    }
}
