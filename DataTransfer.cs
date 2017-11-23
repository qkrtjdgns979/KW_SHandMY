using UnityEngine;


/*
 * PhotonView를 통해 네트워크 데이터를 송수신 하기 위한 스크립트.
 * 현재는 CurHex만 실시간으로 보냄.
 */
public class DataTransfer : MonoBehaviour
{
    private PlayerBase pb;
    private PhotonView pv;

    private int currX;
    private int currY;
    private int currZ;
    private void Awake()
    {
        pb = GetComponent<PlayerBase>();
        pv = GetComponent<PhotonView>();
        pv.synchronization = ViewSynchronization.UnreliableOnChange;
        pv.ObservedComponents[0] = this;
    }


    //Update는 컴퓨터의 사양에 영향을 받는 1프레임마다 실행되는 함수 vs Fixedupdate는 호출 주기가 항상 똑같은 함수.
    //Update의 호출 주기는 수많은 요인에 의하여 균등할 수 없음. 그래서 완벽히 일치하는 물리적인 상황이 있더라도
    //물리적인 연산을 Update에서 처리 할 경우 다른 결과가 나올 수 있음.
    //그렇기에 언제나 정확한 물리적인 연산을 처리하고 싶다면 호출 주기가 일정한 FixedUpdate에서 처리해야함.
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)        //송신,
        {
            stream.SendNext(pb.CurHex.MapPos.x);       //위치정보 전송
            stream.SendNext(pb.CurHex.MapPos.y);       //위치정보 전송
            stream.SendNext(pb.CurHex.MapPos.z);       //위치정보 전송
        }
        else                        //수신
        {
            currX = (int)stream.ReceiveNext();        //위치정보 수신
            currY = (int)stream.ReceiveNext();        //위치정보 수신
            currZ = (int)stream.ReceiveNext();        //위치정보 수신

            Hex hex = MapManager.mm.GetPlayerHex(currX,currY,currZ);     //위치정보를 수신후 curHex를 저장.
            pb.CurHex = hex;
        }
    }
    
}
