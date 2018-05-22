using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectionScript : LightenableObject {

    public enum DoorType { Normal, Hidden, Bell, OpenByOneSide }

    #region Public Variables
    [Header("\tGame Designers Variables")]

    [Header("Door Type")]
    [Tooltip("Este valor indica de que tipo es la puerta.")]
    public DoorType currentDoorType;
    [Tooltip("Este valor indica si la puerta apagara las luces del escenario al pasar a la siguiente habitacion")]
    public bool isLightSwitcher;
    [Tooltip("Este valor indica cuanto rato debes estar enfocando la puerta escondida para que se revele")]
    [Range(1,5)] public float lightenedTimeToShow = 3f;
    [Tooltip("Este valor indica si la puerta se abrira solo por la derecha, en caso contrario por el izquierdo")]
    public bool isRightOpener;

    [Header("\t    Own Script Variables")]
    [Range(1,15)] public float checkerDistance = 6f;
    [Tooltip("Collider principal de la puerta")]
    public BoxCollider myCollider;
    [Tooltip("Punto derecho de la puerta")]
    public Transform rightPoint;
    [Tooltip("Punto izquierdo de la puerta")]
    public Transform leftPoint;
    [Tooltip("Puerta derecha")]
    public GameObject rightDoor;
    [Tooltip("Puerta izquierda")]
    public GameObject leftDoor;
    [Tooltip("Visual entero")]
    public GameObject visual;
    [Tooltip("Objetos que actuan como mascaras para bloquear la puerta")]
    public List<GameObject> blockMaskList;
    [Tooltip("Objetos que actuan como muros para esconder la puerta")]
    public List<GameObject> hiddenWalls;
    [Tooltip("Particulas para cuando la puerta esta oculta")]
    public GameObject lighteningParticles;
    #endregion

    #region Private Variables
    private bool isSideDoor;
    private bool isDoorOpen;
    private bool insideRadius;
    private bool lightsSwitched;
    private float timer;

    private GameObject roomLeftBottom, roomRightTop;

    private Transform target;
    private Transform leftDownPoint, rightTopPoint;
    #endregion

    private void Awake()
    {
        if (myCollider == null)
        {
            myCollider = GetComponent<BoxCollider>();
        }

        blockMaskList.ForEach(x => x.SetActive(false));
        SetDoors();
    }

    private void Start()
    {
        if (transform.rotation.eulerAngles.y == 0 || transform.rotation.eulerAngles.y == 180)
        {
            isSideDoor = true;
        }

        if (roomLeftBottom != null && roomRightTop != null)
        {
            DisableStartRooms();
        }

        target = GameManager.Instance.GetPlayer();

        lighteningParticles.SetActive(false);
    }

    private void Update()
    {
        if (currentDoorType == DoorType.Hidden)
        {
            CheckPlayerDistance();
            Lightened();
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
        ray = new Ray(transform.position + transform.right * checkerDistance + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer")))
        {
            roomRightTop = hit.transform.parent.gameObject;
        }
        else
        {
            myCollider.isTrigger = false;
        }

        //Left Ray
        ray = new Ray(transform.position - transform.right * checkerDistance + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer")))
        {
            roomLeftBottom = hit.transform.parent.gameObject;
        }
        else
        {
            myCollider.isTrigger = false;
        }

    }

    private void DisableStartRooms()
    {
        isDoorOpen = true;

        leftPoint.transform.localPosition = -Vector3.right * checkerDistance;
        rightPoint.transform.localPosition = Vector3.right * checkerDistance;

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

    #region Bell Methods
    public void OpenByBell()
    {
        currentDoorType = DoorType.Normal;
    }
    #endregion

    #region Lighten Methods
    private void CheckPlayerDistance()
    {
        if (Vector3.Distance(target.position, transform.position) < GameManager.Instance.player.lanternDamageLength)
        {
            GameManager.Instance.player.OnLightenObjectEnter(this.gameObject);
        }
        else
        {
            GameManager.Instance.player.OnLightenObjectExit(this.gameObject);
            OutsideLanternRange();
        }
    }

    private void Lightened()
    {
        if (insideRadius)
        {
            if (!lighteningParticles.activeInHierarchy)
                lighteningParticles.SetActive(true);

            timer += Time.deltaTime;

            if (timer >= lightenedTimeToShow)
            {
                hiddenWalls.ForEach(x => x.SetActive(false));
                lighteningParticles.SetActive(false);
                currentDoorType = DoorType.Normal;
                timer = 0;
            }
        }
        else
        {
            if (timer > 0)
            {
                if (lighteningParticles.activeInHierarchy)
                    lighteningParticles.SetActive(false);

                timer = 0;
            }
        }
    }

    public override void InsideLanternRange()
    {
        if (GameManager.Instance.GetIsInCombateMode())
            return;

        insideRadius = true;
    }

    public override void OutsideLanternRange()
    {
        insideRadius = false;
    }

    public override bool IsInSight()
    {
        return insideRadius;
    }
    #endregion

    #region Block Door Methods
    public void BlockDoor()
    {
        blockMaskList.ForEach(x => x.SetActive(true));
        isDoorOpen = false;
    }

    public void UnblockDoor()
    {
        if (roomLeftBottom != null && roomRightTop != null)
        {
            isDoorOpen = true;
        }
        blockMaskList.ForEach(x => x.SetActive(false));
    }

    public bool IsDoorFromRoom(GameObject room)
    {
        return room == roomLeftBottom || room == roomRightTop;
    }
    #endregion

    #region Animation Methods
    public Vector3 GetDoorOpeningPos(Vector3 targetPos)
    {
        if (Vector3.Distance(targetPos, leftPoint.position) < Vector3.Distance(targetPos, rightPoint.position))
            //Left closer
            return new Vector3(leftPoint.position.x, targetPos.y, leftPoint.position.z);
        else
            //Right closer
            return new Vector3(rightPoint.position.x, targetPos.y, rightPoint.position.z);
    }

    public Vector3 GetDoorClosingPos(Vector3 targetPos)
    {
        if (Vector3.Distance(targetPos, leftPoint.position) < Vector3.Distance(targetPos, rightPoint.position))
            //Left closer
            return new Vector3(rightPoint.position.x, targetPos.y, rightPoint.position.z);
        else
            //Right closer
            return new Vector3(leftPoint.position.x, targetPos.y, leftPoint.position.z);
    }

    public bool IsRightClosestPoint(Vector3 targetPos)
    {
        return Vector3.Distance(targetPos, leftPoint.position) > Vector3.Distance(targetPos, rightPoint.position);
    }

    public void OpenDoorAnimation()
    {
        if (isDoorOpen && currentDoorType == DoorType.Normal || (currentDoorType == DoorType.OpenByOneSide && isRightOpener ? IsRightClosestPoint(GameManager.Instance.GetPlayer().transform.position) : !IsRightClosestPoint(GameManager.Instance.GetPlayer().transform.position)))
        {

            if (currentDoorType == DoorType.OpenByOneSide)
                currentDoorType = DoorType.Normal;

            GameManager.Instance.ShowAllDoors();

            lightsSwitched = false;

            if (!GameManager.Instance.IsDirectLightActivated())
            {
                GameManager.Instance.SwitchMainLight();
                lightsSwitched = true;
            }

            ActiveBothRooms();

            HideDoors();
            GameManager.Instance.player.StartCrossingDoor();

            //Perform the opening
            if (isSideDoor)
            {
                CameraBehaviour cb = CameraBehaviour.Instance;
                cb.ChangeCameraBehaviourState(CameraBehaviour.CameraState.CrossDoor);
                cb.MoveAtPoint(transform.position, GameManager.Instance.GetPlayer().position.x > transform.position.x);
            }
        }
    }

    public bool IsDoorOpen()
    {
        return isDoorOpen && (currentDoorType == DoorType.Normal || (currentDoorType == DoorType.OpenByOneSide && isRightOpener ? IsRightClosestPoint(GameManager.Instance.GetPlayer().transform.position) : !IsRightClosestPoint(GameManager.Instance.GetPlayer().transform.position)));
    }

    public void CloseDoorAnimation()
    {
        ShowDoors();

        if (isLightSwitcher && !lightsSwitched)
        {
            GameManager.Instance.SwitchMainLight();
            lightsSwitched = true;
        }

        if (isSideDoor)
        {
            if (target.position.x > transform.position.x)
            {
                DisableLeftBottomRoom();
            }
            else
            {
                DisableRightTopRoom();
            }
        }
        else
        {
            if (target.position.z > transform.position.z)
            {
                DisableLeftBottomRoom();
            }
            else
            {
                DisableRightTopRoom();
            }
        }

        GameManager.Instance.ShowAndHideDoors();
    }

    private void ShowDoors()
    {
        leftDoor.SetActive(true);
        rightDoor.SetActive(true);
        myCollider.enabled = true;
    }

    private void HideDoors()
    {
        leftDoor.SetActive(false);
        rightDoor.SetActive(false);
        myCollider.enabled = false;
    }
    #endregion

    #region Getter Methods
    public bool GetIsHiddenDoor()
    {
        return currentDoorType == DoorType.Hidden;
    }
    #endregion

    #region Unity Inspector/Gizmos Methods
    private void OnValidate()
    {
        if (currentDoorType == DoorType.Hidden && !hiddenWalls[0].activeInHierarchy)
        {
            foreach (GameObject hw in hiddenWalls)
                hw.SetActive(true);
        }
        else if (currentDoorType != DoorType.Hidden && hiddenWalls[0].activeInHierarchy)
        {
            foreach (GameObject hw in hiddenWalls)
                hw.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + (isSideDoor ? transform.right : Vector3.zero) * checkerDistance);   
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + (isSideDoor ? -transform.right : Vector3.zero) * checkerDistance);
    }
    #endregion
}
