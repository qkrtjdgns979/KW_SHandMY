using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * PhotonNetwork를 사용하여 로비를 구현.
 */
public class PhotonInit : MonoBehaviour
{
    public InputField userID;
    public InputField roomName;
    //public InputField roomPwd;              //todo : 방생성시, 패스워드 구현은 아직못함. 추후에 할 예정.
    public Dropdown peopleDrop;
    public Text text_Msg;

    public string version = "v1.0";             //게임의 버전, 버전이 같아야 네트워크 연결이 가능.

    public GameObject scrollContents;
    public GameObject roomItem;
    public GameObject MakeRoomPanel;

    private void Awake()    //시작시, 네트워크 연결.
    {
        Screen.SetResolution(800, 600, false);
        
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings(version);    //version으로 로비에 입장하는 함수.

        text_Msg.text = "";

    }
    
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());  //접속과정에 대한 로그를 출력

    }
    void OnJoinedLobby()    //포톤 클라우드에 정상적으로 접속한 후 로비에 입장하면 호출되는 콜백 함수
    {
        Debug.Log("entered lobby");
        userID.text = GetUserID();          //이전에 사용했던 ID를 불러온다.
    }
    void OnJoinedRoom() //룸에 입장하면 호출되는 콜백 함수
    {
        StartCoroutine(this.LoadBattleField());
    }
    void OnPhotonRandomJoinFailed() //무작위 룸 접속에 실패한 경우 호출되는 콜백 함수
    {
        Debug.Log("PhotonRandomRoom Join Failed");

        RoomOptions option = new RoomOptions();     //무작위방에 접속실패했으므로(보통 들어갈 방이없을때) 들어갈 방을 생성한다.

        //Room에 대한 옵션설정.
        option.IsVisible = true;
        option.IsOpen = true;
        option.MaxPlayers = 2;

        string roomName = "ROOM_" + Random.Range(0, 999).ToString("000");

        PhotonNetwork.CreateRoom(roomName, option, TypedLobby.Default);     //방을생성.

    }
    void OnPhotonCreateRoomFailed(object[] error)   //createRoom함수를 사용하여 룸생성에 실패할 경우 콜백함수(대부분은 룸의 이름이 존재하는 경우)
    {
        Debug.Log(error[0].ToString()); //오류 코드
        Debug.Log(error[1].ToString()); //오류 메세지
    }
    void OnPhotonJoinRoomFailed(object[] msg)        //joinroom에 실패하였을시 호출되는 callback함수, 대부부분의 문제는 방이 없거나, 꽉찼거나.
    {
        //codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.
        Debug.Log(msg[1].ToString());
        StartCoroutine(DelayMsg(msg[1].ToString()));
    }

    public IEnumerator DelayMsg(string msg)          //MSG_text에 일정시간 string을 표현하는 코루틴함수.
    {
        text_Msg.text = msg;
        yield return new WaitForSeconds(1.0f);
        text_Msg.text = "";
    }

    void OnPhotonPlayerConnected(PhotonPlayer player)   //새로운 플레이어가 접속했을때
    {
        Debug.Log("New player.ID : " + player.ID);                                 //새로들어온놈 id(1씩증가, 마스터가 1이면 그다음놈은 2 ,3 , 4,5,6,)

    }
    void OnPhotonPlayerDisconnected(PhotonPlayer player)   //리모트 플레이어가 룸을 나갔을 때 호출 됩니다. 이 PhotonPlayer 는 이 시점에서 playerlist 에서 이미 제거된 상태 입니다
    {
        Debug.Log("Disconnect Player.ID : " + player.ID);
    }
    string GetUserID()
    {
        string userID = PlayerPrefs.GetString("USER_ID");       //이전에 사용했던 ID를 불러온다.

        if (string.IsNullOrEmpty(userID))                        //불러올 ID가 없으면 무작위로 선정.
        {
            userID = "USER_" + Random.Range(0, 999).ToString("000");
        }
        return userID;                                          //불러온 ID를 반환.
    }
    public void OnClickJoinRandomRoom()     //"Join Random Room"버튼을 누르면 실행되는 함수.(무작위 방에 접속)
    {
        PhotonNetwork.player.NickName = userID.text;        //설정한 ID를 NickName으로 설정하고
        PlayerPrefs.SetString("USER_ID", userID.text);      //저장한다.

        PhotonNetwork.JoinRandomRoom();                     //무작위 방에 접속.
    }
    IEnumerator LoadBattleField()
    {
        PhotonNetwork.isMessageQueueRunning = false;    //씬을 이동하는 동안 포톤 클라우드 서버로부터 네트워크 메세지 수신 중단

        AsyncOperation ao = SceneManager.LoadSceneAsync("scRoom");          //Room씬으로 이동.
        yield return ao;

    }
    public void OnOpenMakeRoomPanel()       //Room Create Panel을 on/off하는 함수.
    {
        bool OnOff_Flag = MakeRoomPanel.activeSelf;
        MakeRoomPanel.SetActive(!OnOff_Flag);
    }
    public void OnClickCreateRoom()         //"Make Room"버튼을 누를시 실행되는 함수.
    {
        string _roomName = roomName.text;       //input을 받아

        if (string.IsNullOrEmpty(_roomName))         //input이 비었다면, 무작위값
        {
            _roomName = "ROOM_" + Random.Range(0, 999).ToString("000");
        }

        PhotonNetwork.player.NickName = userID.text;        //Nickname을 저장하고
        PlayerPrefs.SetString("USER_ID", userID.text);

        RoomOptions roomOptions = new RoomOptions();    //room option을 셋팅하고
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        int peopleNum = 3;                  //default값은 4

        //dropdown.value - value의 index, 그렇다면 index가 아닌 value그 자체를 받는 함수는 없나?
        if (peopleDrop.value == 0)
            peopleNum = 2;
        else if (peopleDrop.value == 1)
            peopleNum = 3;


        roomOptions.MaxPlayers = (byte)peopleNum;
        /*
        //todo : 룸의 password를 셋팅해야하는데, 당장은 방법을 모르겠다. 해쉬테이블을 이용하는것 같은데 ROOM 클래스에서 접근방법을 모르겠다.
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "PASSWORD", "1234" } };



        if(inputPass.text == PhotonNetwork.room.CustomProperties["pass"].   tostring())
        {
            Photonnetwork.Joinroom(name);
        }
         */


        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);   //Room 생성.

    }

    //생성된 룸 목록이 변경됐을 때 호출되는 콜백 함수
    //이 함수는 포톤 클라우드 서버에 의해 호출되는 콜백 함수로서 이 함수 내에서 photonNetwork.GetRoomList함수를 호출하면 룸 목록을 가져올 수 있다.
    //룸목록이 갱신된후 클라이언트에 전달되는 시간은 약 5초정도가 걸리므로, 실시간으로 바로바로 갱신되지는 안음. 약간의 딜레이가 존재.
    void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOMITEM"))   //Room의 중복이 생기므로, 기존의 Room을 삭제.
            Destroy(obj);

        int rowCount = 0;
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())          //Roomlist를 하나씩 받아오며 실행.
        {
            GameObject room = Instantiate(roomItem) as GameObject;          //instance를 생성.
            room.transform.SetParent(scrollContents.transform, false);      //차일드화.
                       
            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;
            roomData.DispRoomData();

            roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoomItem(_room); });      //버튼입력에 대한 listner설정.


            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
        }
    }
    void OnClickRoomItem(RoomInfo room)         //방을 클릭시, 실행될 함수
    {
        PhotonNetwork.player.NickName = userID.text;
        PlayerPrefs.SetString("USER_ID", userID.text);

        PhotonNetwork.JoinRoom(room.Name);

    }
}
