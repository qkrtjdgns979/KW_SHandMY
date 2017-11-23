using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager pm;         //싱글턴.
    private PhotonView pv;                  //rpc호출을 위한 photonview
    public Vector3[] genPos;                //player들을 gen할 position.
    public PhotonPlayer[] Photon_Players;
    
    private int CurTurnIdx = 0;              //현재 player 턴을 가리키는 index.
    [HideInInspector]
    public PlayerBase MyBase = null;               //현재 클릭되어있는 캐릭터.

    public List<PlayerBase> Players = new List<PlayerBase>();                   //수많은 player를 관리하기 위한 list. 모든 플레이어(ai와 자기 자신포함)가 포함됨.
    public List<PlayerBase> MyUnit = new List<PlayerBase>();                    //나의 유닛.
    public List<PlayerBase> EnemyUnit = new List<PlayerBase>();                 //상대방 유닛.
    
    [HideInInspector]
    public bool MyTurn = false;                                                 //나의 턴인지
    [HideInInspector]
    public bool CanMove = false;                                                //이동할 수 있는지.

    private void Awake()
    {
        if (pm == null)
            pm = this;
        pv = GetComponent<PhotonView>();
    }
    
    public void GenPlayerTest()     //player를 생성하는 함수.  rpc를 사용하여, player마다 각자 자리에 생성
    {
        int i = 0;

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            pv.RPC("InstantPlayer_RPC", player, i);
            i = 4;
        }
        
    }
    [PunRPC]
    public void InstantPlayer_RPC(int i)              //InstantPlayer_RPC
    {
        /*
         * 유닛의 종류(Resource폴더에 있는 이름으로)
         * 1."Infantry"
         * 2."Scout"
         * 3."MachineGun"
         * 4."Sniper"
         */

        //idx = 0
        if(PhotonNetwork.isMasterClient)
        {
            UserPlayer temp;
            Hex hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("Infantry_Red", hex.transform.position, hex.transform.rotation, 0)).GetComponent<UserPlayer>();
            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
            i++;
            //Rifle, idx = 1
            hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("Scout_Red", hex.transform.position, Quaternion.identity, 0)).GetComponent<UserPlayer>();
           

            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
            i++;
            //machinegun, idx = 2
            hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("MachineGun_Red", hex.transform.position, Quaternion.identity, 0)).GetComponent<UserPlayer>();
            
            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
            i++;
            //assault, idx = 3
            hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("Sniper_Red", hex.transform.position, Quaternion.identity, 0)).GetComponent<UserPlayer>();
           
            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
        }
        else        //idx =4
        {
            UserPlayer temp;
            Hex hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("Infantry_Blue", hex.transform.position, hex.transform.rotation, 0)).GetComponent<UserPlayer>();
           
            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
            i++;
            //Rifle, idx = 1
            hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("Scout_Blue", hex.transform.position, Quaternion.identity, 0)).GetComponent<UserPlayer>();
          

            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
            i++;
            //machinegun, idx = 2
            hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("MachineGun_Blue", hex.transform.position, Quaternion.identity, 0)).GetComponent<UserPlayer>();
           
            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
            i++;
            //assault, idx = 3
            hex = MapManager.mm.GetPlayerHex((int)genPos[i].x, (int)genPos[i].y, (int)genPos[i].z);
            temp = (PhotonNetwork.Instantiate("Sniper_Blue", hex.transform.position, Quaternion.identity, 0)).GetComponent<UserPlayer>();
        

            temp.transform.LookAt(Vector3.zero);
            temp.CurHex = hex;                                                    //player가 현재 위치한 Hex의 정보를 담고있는 CurHex에 저장하고, player를 그 위치로 이동시킴.
            MyUnit.Add(temp);
        }
    }
    public void FindObjectAndSort()                     //List Players에서 Enemy를 찾아 분류하는 함수,(초기설정을 위한 함수)
    { 
        foreach(PlayerBase pb in Players)
        {
            if(!pb.GetComponent<PhotonView>().isMine)
                EnemyUnit.Add(pb);
        }
    }
    public void MovePlayer(Hex start, Hex dest)     //해당 player를 목적지로 이동,턴종료,하이라이트된 Hex를 원상복구 시키는 함수.
    {
        //1. 출발점부터 도착지까지 도달할수 있는지 정보를 얻어옴.  (갈수 없다면 return)
        List<Hex> path = MapManager.mm.IsReachAble(start, dest, MyBase.MoveRange);
        if (path == null)
            return;
        
        if (path.Count <= MyBase.MoveRange && dest.Passable == true)              //플레이어의 상태를 Moving으로 변경하고, playerbase에 목적지 설정. 하이라이트된 맵 초기화.
        {
            MyBase.MoveHexes = path;      //GetPath함수를 사용하면 dest까지 가는 최단경로가 반환된다. 그러므로 MoveHexes에는 최단경로가 저장됨.

            MyBase.act = ACT.MOVING;
            MapManager.mm.ResetMapColor();
        }
    }
    public void MovePlayer(Hex start, Hex dest, PlayerBase pb)     //오버라이딩
    {
        //1. 출발점부터 도착지까지 도달할수 있는지 정보를 얻어옴.  (갈수 없다면 return)
        List<Hex> path = MapManager.mm.IsReachAble(start, dest, pb.MoveRange);
        if (path == null)
            return;

        if (path.Count <= pb.MoveRange && dest.Passable == true)              //플레이어의 상태를 Moving으로 변경하고, playerbase에 목적지 설정. 하이라이트된 맵 초기화.
        {
            pb.MoveHexes = path;      //GetPath함수를 사용하면 dest까지 가는 최단경로가 반환된다. 그러므로 MoveHexes에는 최단경로가 저장됨.

            pb.act = ACT.MOVING;
            MapManager.mm.ResetMapColor();
        }
    }
    public void TurnOver()         //해당 턴에 player가 모든 행동을 마치면 턴 index를 증가시키고 해당 player의 상태를 IDLE로 변경시키는 함수.
    {
        MyTurn = false;
        MyBase.act = ACT.IDLE;                      //턴이 종료되었으므로 해당 player의 상태를 IDLE로 변경.
        MyBase = null;
        
        pv.RPC("InformMastertoFinish", PhotonTargets.MasterClient, null);   //master에게 turn이 종료됬음을 알린다.
       
    }
    [PunRPC]
    public void InformMastertoFinish()  //위에서 rpc로 호출중. 마스터의 turnLoop flag를 변화시킴.
    {
        HexManager.hm.flag = true;
    }
    public void GiveTurn()      //턴을 해당유저에게 넘기고, UI출력,
    {
        string str = Photon_Players[CurTurnIdx].NickName + " TURN";
        pv.RPC("ReceiveTurn", Photon_Players[CurTurnIdx], null);    //rpc
        pv.RPC("ShowUiText", PhotonTargets.All, str);
        CurTurnIdx++;
        if (Photon_Players.Length <= CurTurnIdx)        //idx값이 초과하면 0으로.
            CurTurnIdx = 0;
    }
    public bool IsVictory()                             //승자가 있는지 확인하는 함수.
    {
        string str = "";

        if (MyUnit.Count == 0)      //내유닛이 없으면 진거.
        {
            str = "Lose!";
            pv.RPC("ShowUiText", PhotonTargets.All, str);
            return true;
        }
        else if (EnemyUnit.Count == 0)      //상대유닛이 없으면 이긴것.
        {
            str = "Win!";
            pv.RPC("ShowUiText", PhotonTargets.All, str);
            return true;
        }


        return false;
    }
    [PunRPC]
    public void ReceiveTurn()
    {
        MyBase = null;
        MyTurn = true;
        CanMove = true; 
    }
    [PunRPC]
    void ShowUiText(string str)     //RPC로 모든 player에게 UI text를 보여주는 함수.
    {
        Text text = HexManager.hm.WhoIsTurn;

        text.text = str;
        text.CrossFadeAlpha(1.0f, 0f, false);
        text.CrossFadeAlpha(0.0f, 2.0f, false);
    }
    public void ShowUiText_RPC(string str)
    {
        pv.RPC("ShowUiText", PhotonTargets.All, str);
    }
    public void SendMoveMappos_RPC(int x, int y, int z, int viewID)   //데이터전송의 딜레이를 없애기 위해, 움직이는 동작을 직접 수행하게 한다.
    {
        pv.RPC("SendMoveMappos", PhotonTargets.All, x, y, z, viewID);
    }
    [PunRPC]
    public void SendMoveMappos(int x, int y, int z, int viewID)     //데이터전송의 딜레이를 없애기 위해, 움직이는 동작을 직접 수행하게 한다.
    {
        PhotonView playerPV = PhotonView.Find(viewID);              //viewID를 통해 움직일 오브젝트를 찾는다.
        PlayerBase pb = playerPV.GetComponent<PlayerBase>();
        Hex moveHex = MapManager.mm.GetPlayerHex(x, y, z);          //움직일 장소를 찾고.
        MyBase = pb;
        MovePlayer(pb.CurHex, moveHex, pb);                         //움직인다.

    }
    public void PushTurnEnd()           //턴종료 버3튼을 누르면 실행되는 함수.
    {
        if(PlayerManager  .pm.MyTurn)
        {
            if (MyBase.act != ACT.MOVING)
            {
                MapManager.mm.ResetMapColor();
                TurnOver();
            }
        }
    }
    

}
