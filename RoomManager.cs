using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*
 * 씬을 전환할때 데이터를 전송하는 방법은 3가지가 있다
 * 1)데이터를 저장하는것 (ex plyerPrefs)
 * 2)static을 사용하는것
 * 3)dontdestroyonload
 */

public class RoomManager : MonoBehaviour
{
    public Text Room_text;
    public GameObject panelPlayers;
    public GameObject userItem;
    public GameObject startBtn;

    private PhotonView pv;

    public Button InputChatBtn;
    public InputField InputChat;
    public Text TextChatMsg;

    public UserData My_UserData;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        PhotonNetwork.isMessageQueueRunning = true;     //씬전환을 하는동안 네트워크메세지를 못받게해놨는데, 씬전환이 되었으므로 이것을 받게 바꿈

        Room_text.text = PhotonNetwork.room.Name;       //현재 룸의 이름을 셋팅

        if (!PhotonNetwork.isMasterClient)              //오직 룸의 master만이 start버튼을 누를 수 있음. 아니면 start버튼을 해제.
            startBtn.SetActive(false);
        
        printPlayer();              //현재 Room에 접속중인 User를 생성.

    }
    private void Start()
    {
        string logStr = "<color=#00ff00>[" + PhotonNetwork.player.NickName + "] Connected</color>";     //Room에 접속하면 접속 로그를 출력.
        pv.RPC("LogMsg", PhotonTargets.All, logStr, null);
    }
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());  //접속과정에 대한 로그를 출력]
    }

    void printPlayer()      //현재 방에 접속중인 USER를 Panel에 생성하는 함수.
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("USERITEM"))        //중복을 피하기위해.   
            Destroy(obj);
        
        foreach (PhotonPlayer user in PhotonNetwork.playerList)             //현재 User를 차례로 접근.
        {
            GameObject _userItem = Instantiate(userItem) as GameObject;             //생성후 차일드화.
            _userItem.transform.SetParent(panelPlayers.transform, false);

            UserData _userData = _userItem.GetComponent<UserData>();
            _userData.userName = user.NickName;


            if (user.IsMasterClient)                //Room의 방장일 경우,  색을 red로 바꾸고 제일 위로 올림.
            {
                _userItem.GetComponent<Text>().color = Color.red;
                _userItem.transform.SetAsFirstSibling();        //child오브젝트의 순서를 가장 첫번째로 올려주는 함수. SetAsFirstSibling(). <->SetAsLastSibling().
            }
            if (user.IsLocal)
            {
                My_UserData = _userData;
            }

            _userData.DispUserData();
        }
    }
    
    void OnPhotonPlayerConnected(PhotonPlayer player)   //새로운 플레이어가 접속했을때
    {
        printPlayer();
    }
    void OnPhotonPlayerDisconnected(PhotonPlayer player)   //리모트 플레이어가 룸을 나갔을 때 호출 됩니다. 이 PhotonPlayer 는 이 시점에서 playerlist 에서 이미 제거된 상태 입니다
    {
        if (PhotonNetwork.isMasterClient)
            startBtn.SetActive(true);

        printPlayer();
    }
    public void BackLobby()
    {   //현재 룸을 빠져나가며 생성한 모든 네트워크 객체를 삭제.
        string logStr = "<color=#00ff00>[" + PhotonNetwork.player.NickName + "] DisConnected</color>";
        pv.RPC("LogMsg", PhotonTargets.All, logStr, null);
        PhotonNetwork.LeaveRoom();
    }
    void OnLeftRoom()   //룸에서 접속 종료되었을 때 호출되는 콜백 함수.
    {
        SceneManager.LoadScene("scLobby");
    }
    public void StartGame()
    {
        if (PhotonNetwork.room.PlayerCount < 2)         //인원이 2명보다 적으면,  실행할 수 없음.
        {
            string errStr = "<color=#ff0000ff>There is insufficient player.</color>";
            LogMsg(errStr, null);
            return;
        }

        string logStr = "<color=#000000ff>Game Start</color>";          //게임시작 로그를 출력하고, 게임시작.
        pv.RPC("LogMsg", PhotonTargets.All, logStr, null);
        pv.RPC("RpcLoadGame", PhotonTargets.All, null);

    }
    [PunRPC]
    void RpcLoadGame()                          //방장이 "start" 버튼을 누르면, rpc로 전체에게 시작을 알림.
    {
        StartCoroutine(this.LoadGame());
    }

    IEnumerator LoadGame()          //씬을 전환.
    {

        yield return new WaitForSeconds(1.0f);

        PhotonNetwork.isMessageQueueRunning = false;    //씬을 이동하는 동안 포톤 클라우드 서버로부터 네트워크 메세지 수신 중단


        AsyncOperation ao = SceneManager.LoadSceneAsync("scInGame");
        yield return ao;

    }
    public void PushChat()  //채팅을 하면 호출되는 함수.
    {
        pv.RPC("LogMsg", PhotonTargets.All, InputChat.text, PhotonNetwork.player.NickName);
        InputChat.text = "";
    }
    [PunRPC]
    void LogMsg(string msg, string player_Name)         //rpc로 메세지를 출력하는 함수.
    {
        if (!string.IsNullOrEmpty(player_Name))     //playerName이 있으면, 로그앞에 PlayerName을 붙인다.
        {
            TextChatMsg.text += "\n[" + player_Name + "] : ";
            TextChatMsg.text += msg;
        }
        else                                        //없다면, 안붙임.
            TextChatMsg.text += "\n" + msg;

    }
    private void Update()
    {
        if (Input.GetKeyDown("enter"))
            PushChat();
    }

}

