using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//해당 씬에서 모든 매니저들을 관리하는 메인 매니저.
public class HexManager : MonoBehaviour
{
    public static HexManager hm;        //싱글턴

    public bool flag = false;                  //GameLoop를 돌리기 위한 flag. (flag == true)일때 다음 turn으로 넘어가게된다.
    public bool gameOver = false;
    
    public Text WhoIsTurn;
    //public Transform[] genHealPos;     //todo : 구현예정.
    
    private PhotonView pv;
    private void Awake()
    {
        if (!hm)
            hm = this;

        PhotonNetwork.isMessageQueueRunning = true;         //네트웍 메시지 연결.
        WhoIsTurn.text = "";
        gameOver = false;
        pv = GetComponent<PhotonView>();
    }
    // Use this for initialization
    void Start ()
    {
        MapManager.mm.CreateMap();                          //master와 client 모두 map을 만듬.

        if(PhotonNetwork.isMasterClient)                    //Master에서만 GameLoop을 실행.
        {
            PlayerManager.pm.GenPlayerTest();               //RPC호출을 통해 각 client는 자신의 탱크를 생성.
            StartCoroutine(GameStart());                    //코루틴 실행.
        }
    }
    private IEnumerator GameStart()
    {
        yield return StartCoroutine(wait_then_StartFinding());          //잠시 대기후 PhotonPlayer를 검색. 이 함수가 다 마친후 다음으로 넘어가게된다.

        StartCoroutine(TurnLoop());                        //Turn을 관리하는 함수를 실행.
    }
    IEnumerator EndGame()          //씬을 전환.
    {

        yield return new WaitForSeconds(2.5f);
        

        PhotonNetwork.isMessageQueueRunning = false;    //씬을 이동하는 동안 포톤 클라우드 서버로부터 네트워크 메세지 수신 중단


        AsyncOperation ao = SceneManager.LoadSceneAsync("scRoom");
        yield return ao;

    }
    [PunRPC]
    void EndGame_RPC()
    {
        StartCoroutine(EndGame());
    }
    //void MakeHeal()       todo : 나중에 다시 만들것.
    //{
    //    int genIdx = Random.Range(0, 4);
    //    PhotonNetwork.Instantiate("HealItem", genHealPos[genIdx].position, Quaternion.identity, 0);
    //}

    private IEnumerator TurnLoop()              //Turn을 관리하는 d함수.
    {
        flag = false;               //TurnLoop를 실행할때마다 flag을 false로 셋팅.
        yield return new WaitForSeconds(0.6f);      //잠시 대기. 무엇을 위한 대기인지 몰라 주석처리했다. 이것이 과연 필요할까

        if (PlayerManager.pm.IsVictory() == true)           //승리자를 있는지 확인하는 함수.
            gameOver = true;
        else
            PlayerManager.pm.GiveTurn();            //Master는 해당 턴의 player에게 턴을 준다.
            

        while (true)                             //flag가 변경될 때까지 무한루프
        {
            if (gameOver)
            {
                gameOver = false;
                Debug.Log("gameover");
                PlayerManager.pm.ShowUiText_RPC("Game Over!");
                pv.RPC("EndGame_RPC", PhotonTargets.All, null);

                break;
            }
            else if (flag)                           //flag가 변경되면 TurnLoop를 다시실행.
                StartCoroutine(TurnLoop());
            else                                //다음 프레임으로 넘김. 
                yield return null;

        }


    }   
    IEnumerator wait_then_StartFinding()            //잠시 대기후 PhotonPlayer를 검색. 이 함수가 다 마친후 다음으로 넘어가게된다.u
    {
        yield return new WaitForSeconds(3.0f);      //네트웍 지연을 기다림.
        //todo : 이부분에 camera fade away를 추가하자.
        PlayerManager.pm.Photon_Players = PhotonNetwork.playerList;
        pv.RPC("FindObjectAndSort_RPC", PhotonTargets.All, null);
        yield return null;      
    }
    [PunRPC]
    void FindObjectAndSort_RPC()
    {
        PlayerManager.pm.FindObjectAndSort();
    }
}
