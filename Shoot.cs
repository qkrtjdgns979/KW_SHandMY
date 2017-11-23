using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private int ShellDmg = 20;       //총알데미지. 값설정은 playerbase에서.
    private PhotonView pv;
    public float ShellSpeed = 10.0f;
    public Transform FireTr;
    public GameObject Obj_Shell;
    private Animator animator;
    
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        ShellDmg = GetComponent<PlayerBase>().ShellDmg;
        animator = GetComponent<Animator>();
    }


    public void Fire_RPC(int viewID)    //피격자의 viewID를 이용해 target을 찾고, 발사.
    {
        pv.RPC("Fire", PhotonTargets.All, viewID);
    }
    [PunRPC]
    void Fire(int viewID)       //viewID를 이용해 피격자를 찾고, 상대방에게 shell발사.
    {
        PhotonView targetPv = PhotonView.Find(viewID);  //피격자검색
        
        transform.LookAt(targetPv.transform);           //피격자를 바라보고.

        //발사.
        GameObject shell = Instantiate(Obj_Shell, FireTr.position, FireTr.rotation);
        shell.GetComponent<Rigidbody>().velocity = shell.transform.forward * ShellSpeed;
        targetPv.GetComponent<Hp>().TakeDmg(ShellDmg);
        animator.SetTrigger("IsShoot");
    }
    
}
