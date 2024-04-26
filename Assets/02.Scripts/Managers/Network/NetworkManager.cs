using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class NetworkManager : MonoBehaviourPunCallbacks {
    
    public static NetworkManager Instance;
    public PhotonView PV { get; private set; }

    private void Awake() {
        Screen.SetResolution(960, 540, false);
        PV = GetComponent<PhotonView>();    
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    public void PhotonDestroy(GameObject go, float time) {
        StartCoroutine(CO_PhotonDestroy(go, time));
    }

    public void PhotonDestroy(float time,int viewId) {
        if(time > 0f)
            StartCoroutine(CO_PhotonDestroy(time, viewId));
        else
            PV.RPC("RPC_PhotonDestroy", RpcTarget.MasterClient, viewId);
    }

    private IEnumerator CO_PhotonDestroy(GameObject go, float time) {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(go);
    }

    private IEnumerator CO_PhotonDestroy(float time,int viewId) {
        yield return new WaitForSeconds(time);
        PV.RPC("RPC_PhotonDestroy", RpcTarget.MasterClient, viewId);
    }

    public void PhotonCreate(string path, Vector3 pos, Quaternion rot) {
        PV.RPC("RPC_PhotonCreate", RpcTarget.MasterClient, path, pos, rot);
    }

    [PunRPC]
    public void RPC_PhotonCreate(string path, Vector3 pos, Quaternion rot) {
        if (!PV.IsMine)
            return;

        PhotonNetwork.Instantiate(path, pos, rot);
    }

    [PunRPC]    
    public void RPC_PhotonDestroy(int viewId) {
        PhotonView view = PhotonView.Find(viewId);
        //if (view == null)
        //    return;

        //if(view.OwnerActorNr != PhotonNetwork.MasterClient.ActorNumber)
        //    view.TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);

        //PhotonNetwork.Destroy(view.gameObject);
        if (view != null && view.IsMine) {
            PhotonNetwork.Destroy(view.gameObject);
        } else if (view != null && PhotonNetwork.IsMasterClient) {
            if (view.OwnerActorNr != PhotonNetwork.MasterClient.ActorNumber)
                view.TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);

            PhotonNetwork.Destroy(view.gameObject);
        }
    }

    public void Test(UnityAction call) {
        StartCoroutine(CoTest(call));
    }


    private IEnumerator CoTest(UnityAction call) {
        PhotonNetwork.ConnectUsingSettings();
        yield return new WaitUntil(() => PhotonNetwork.IsConnected);

        yield return new WaitUntil(() => PhotonNetwork.InLobby);

        JoinOrCreateRoom();
        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        call?.Invoke();
    }


    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();
        Debug.Log("서버 접속");
        PhotonNetwork.JoinLobby();
    }



    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("Disconnected from server: " + cause.ToString());
        if (cause == DisconnectCause.DisconnectByClientLogic) {
            Debug.Log("Failed to join a lobby.");
        }
    }




    public override void OnJoinedLobby() {
        base.OnJoinedLobby();
        Debug.Log("로비 입장 성공");
    }





    public void CreateRoom() => PhotonNetwork.CreateRoom("room1", new RoomOptions { MaxPlayers = 2 });

    public void JoinRoom() => PhotonNetwork.JoinRoom("room2");

    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnCreatedRoom() => Debug.Log("방 생성 완료");

    public override void OnJoinedRoom() => Debug.Log("방 참가 완료");

    public override void OnCreateRoomFailed(short returnCode, string message) => Debug.Log("방 생성 실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => Debug.Log("방 참가 실패");

    public override void OnJoinRandomFailed(short returnCode, string message) => Debug.Log("랜덤 방 참가 실패");



    [ContextMenu("정보")]
    void Info() {
        if (PhotonNetwork.InRoom) {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        } else {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }
}