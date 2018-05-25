using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWallScript : MonoBehaviour {

    #region Public Variables
    [Header("\tGame Designers Variables")]

    [Header("Door Type")]
    [Tooltip("Este valor indica si la puerta apagara las luces del escenario al pasar a la siguiente habitacion")]
    public bool isLightSwitcher;
    [Tooltip("Este valor indica la velocidad a la cual girará 180 grados la pared falsa")]
    public float turnSpeed = 2;

    [Header("\t    Own Script Variables")]
    [Range(1, 15)]
    public float checkerDistance = 6f;
    [Tooltip("Parent wall")]
    public GameObject parent;
    [Tooltip("Visual entero")]
    public GameObject visual;
    [Tooltip("Punto de giro")]
    public Transform wallPoint;
    #endregion

    #region Private Variables
    private bool lightsSwitched;
    private bool isDoorOpen = true;
    private bool isTurning;
    private bool isLeftRoomActive = true;
    private float currentRotationBeforeTurn;
    private float timer;

    private GameObject roomLeftBottom, roomRightTop;

    private Transform target;
    #endregion

    private void Awake()
    {
        SetDoors();
    }

    private void Start()
    {
        if (roomLeftBottom != null && roomRightTop != null)
        {
            DisableStartRooms();
        }

        target = GameManager.Instance.GetPlayer();
    }

    private void Update()
    {
        if (isTurning)
        {
            timer += Time.deltaTime * turnSpeed;

            parent.transform.rotation = Quaternion.Euler(Vector3.Lerp(Vector3.up * currentRotationBeforeTurn, Vector3.up * (currentRotationBeforeTurn + 180), timer));

            if (timer >= 1)
            {
                timer = 0;
                isTurning = false;
            }
        }
    }

    #region Show/Hide Door
    public void HideVisualDoor()
    {
        visual.SetActive(false);
    }

    public void ShowVisualDoor()
    {
        visual.SetActive(true);
    }
    #endregion

    #region Show/Hide Rooms
    private void SetDoors()
    {
        Ray ray;
        RaycastHit hit;

        //Right Ray
        ray = new Ray(transform.position + transform.forward * checkerDistance + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer")))
        {
            roomRightTop = hit.transform.parent.gameObject;
        }

        //Left Ray
        ray = new Ray(transform.position - transform.forward * checkerDistance + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer")))
        {
            roomLeftBottom = hit.transform.parent.gameObject;
        }

    }

    private void DisableStartRooms()
    {
        Ray ray;
        RaycastHit hit;

        ray = new Ray(GameManager.Instance.GetPlayer().position, Vector3.down);
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer")))
        {
            if (roomRightTop != hit.transform.parent.gameObject)
            {
                DisableRightTopRoom();
            }
        }

        ray = new Ray(GameManager.Instance.GetPlayer().position, Vector3.down);
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer")))
        {
            if (roomLeftBottom != hit.transform.parent.gameObject)
            {
                DisableLeftBottomRoom();
            }
        }
    }

    private void ActiveBothRooms()
    {
        roomRightTop.SetActive(true);
        roomRightTop.GetComponent<RoomScript>().HideBlackWalls();
        roomLeftBottom.SetActive(true);
        roomLeftBottom.GetComponent<RoomScript>().HideBlackWalls();
    }

    private void DisableRightTopRoom()
    {
        roomRightTop.SetActive(false);
        roomLeftBottom.GetComponent<RoomScript>().ShowBlackWalls();
    }

    private void DisableLeftBottomRoom()
    {
        roomLeftBottom.SetActive(false);
        roomRightTop.GetComponent<RoomScript>().ShowBlackWalls();
    }
    #endregion

    #region Block Door Methods
    public void BlockDoor()
    {
        isDoorOpen = false;
    }

    public void UnblockDoor()
    {
        isDoorOpen = true;
    }

    public bool IsDoorFromRoom(GameObject room)
    {
        return room == roomLeftBottom || room == roomRightTop;
    }
    #endregion

    #region Animation Methods
    public Vector3 GetFakeWallPoint(Vector3 targetPos)
    {
        return new Vector3(wallPoint.position.x, targetPos.y, wallPoint.position.z);
    }

    public bool OpenDoorAnimation()
    {
        if (isDoorOpen)
        {
            GameManager.Instance.ShowAllDoors();

            lightsSwitched = false;

            if (!GameManager.Instance.IsDirectLightActivated())
            {
                GameManager.Instance.SwitchMainLight();
                lightsSwitched = true;
            }

            ActiveBothRooms();
            CameraBehaviour.Instance.ChangeCameraBehaviourState(CameraBehaviour.CameraState.FakeWall);

            currentRotationBeforeTurn = parent.transform.eulerAngles.y;
            isTurning = true;
            timer = 0;

            return true;
        }
        else
        {
            //Sound deny opening
            return false;
        }
    }

    public void CloseDoorAnimation()
    {
        if (isLightSwitcher && !lightsSwitched)
        {
            //GameManager.Instance.SwitchMainLight();
            //lightsSwitched = true;
        }

        if (!isLeftRoomActive)
            DisableLeftBottomRoom();
        else
            DisableRightTopRoom();

        isLeftRoomActive = !isLeftRoomActive;

        GameManager.Instance.ShowAndHideDoors();
    }
    #endregion

    #region Public Methods
    public bool IsFullTurned()
    {
        return !isTurning;
    }
    #endregion

    #region Unity Inspector/Gizmos Methods
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + transform.forward * checkerDistance);
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up - transform.forward * checkerDistance);
    }
    #endregion
}
