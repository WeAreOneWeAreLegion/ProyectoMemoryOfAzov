using UnityEngine;
using UnityEditor;

public class CameraBehaviour : MonoSingleton<CameraBehaviour> {

    public enum CameraState { Following, CrossDoor, Cinematic }
    public enum CameraLookState { Normal, LookUp, LookDown }

    [System.Serializable]
    public struct CameraLook
    {
        [Tooltip("Distancia que mantiene la camara respecto al target actual")]
        [Range(5, 120)] public float cameraDistance;
        [Tooltip("Altura que mantiene la camara respecto al target actual")]
        [Range(0, 4)] public float cameraYOffset;
        [Tooltip("Angulo que mantiene la camara respecto al target actual")]
        [Range(-10, 90)] public float cameraAngle;
    }

    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Header("Camera Variables")]
    [Tooltip("Velocidad de la camara al apuntar al target actual")]
    [Range(0,40)] public float cameraFollowSpeed = 15f;
    [Tooltip("Velocidad de rotacion de la camara al apuntar al target actual")]
    [Range(0,40)] public float cameraAngularSpeed = 15f;
    [Tooltip("La longitud en la cual el player parará de moverse al chochar una pared lateral (Linea Blanca)")]
    [Range(0,10)] public float sideDistance = 4.5f;
    [Tooltip("La longitud en la cual el player parará de moverse al chochar una trasera lateral (Linea Azul)")]
    [Range(0,10)] public float backwardsDistance = 4.5f;

    [Header("Camera Types")]
    public CameraLook normalCamera;
    public CameraLook lookUpCamera;
    public CameraLook lookDownCamera;
    public CameraLookState currentCameraLookState = CameraLookState.Normal;

    [Header("CrossDoor Variables")]
    [Tooltip("Cuantos segundos tardara la camara en moverse de la entrada de la puerta a la salida de la puerta")]
    [Range(0, 40)] public float crossDoorCameraSpeed = 0.2f;

    [Header("\t    Own Script Variables")]
    [Tooltip("Target a seguir")]
    public Transform target;
    [Tooltip("Ray lateral para detectar paredes y por tanto clampear la camara")]
    [Range(50,150)] public float raySidesDistance = 100f;
    #endregion

    #region Private Variables
    private float xLeftWall;
    private float xRightWall;
    private float zBackWall;
    private float currentRotation;
    private bool wallsFound;
    private bool backwardDistanceSet;
    private float backwardFirstDistance;

    private Vector3 moveAtPoint;

    private CameraState currentState;
    private CameraLook currentCameraLook;
    private Vector3 backwards = Vector3.zero;

    private TransparentObject currentWall;
    #endregion

    private void Start()
    {
        if (target == null)
        {
            target = GameManager.Instance.GetPlayer();
        }

        currentRotation = transform.eulerAngles.x;
        currentCameraLook = normalCamera;
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentCameraLook = normalCamera;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            currentCameraLook = lookUpCamera;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCameraLook = lookDownCamera;
        }

        if (Time.timeScale == 0)
            return;
        if (currentState == CameraState.Following)
            CameraMovement();
        else if (currentState == CameraState.CrossDoor)
            CrossDoorMovement();
        else if (currentState == CameraState.Cinematic)
        {   
            //Cinematic
        }
    }

    #region Following Methods
    private void CameraMovement()
    {
        if (currentCameraLook.cameraAngle != transform.eulerAngles.x)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.right * currentCameraLook.cameraAngle), cameraAngularSpeed * Time.deltaTime);
        }

        if (!wallsFound)
        {
            RaycastHit hit;

            Ray ray = new Ray(target.transform.position, -transform.right);
            Physics.Raycast(ray, out hit, raySidesDistance, LayerMask.GetMask("WallLayer"));
            //Set left wall
            xLeftWall = hit.point.x;
            //Debug.Log("Left wall name: "+hit.transform.name+" de: " +hit.transform.parent.parent.name);

            ray = new Ray(target.transform.position, transform.right);
            Physics.Raycast(ray, out hit, raySidesDistance, LayerMask.GetMask("WallLayer"));
            //Set right wall
            xRightWall = hit.point.x;
            //Debug.Log("Right wall name: "+hit.transform.name + " de: " + hit.transform.parent.parent.name);

            ray = new Ray(target.transform.position, -transform.forward);
            Physics.Raycast(ray, out hit, raySidesDistance, LayerMask.GetMask("WallLayer"));
            //Set back wall
            try
            {
                zBackWall = hit.point.z;
            }
            catch
            {
                Debug.LogWarning("Error no back wall");
                EditorApplication.isPaused = true;
            }
            //Debug.Log("Back wall name: "+hit.transform.name + " de: " + hit.transform.parent.parent.name);

            wallsFound = true;
        }

        bool rightCollision = Mathf.Abs(target.transform.position.x - xRightWall) < sideDistance;
        bool leftCollision = Mathf.Abs(target.transform.position.x - xLeftWall) < sideDistance;
        bool backCollision = Mathf.Abs(target.transform.position.z - zBackWall) < backwardsDistance;

        float yPosition = (target.position + (Vector3.up * currentCameraLook.cameraYOffset) - (transform.forward * currentCameraLook.cameraDistance)).y;

        if (!rightCollision && !leftCollision && !backCollision)
        {
            //TotalLerp
            transform.position = Vector3.Lerp(transform.position, target.position + (Vector3.up * currentCameraLook.cameraYOffset) - (transform.forward * currentCameraLook.cameraDistance), cameraFollowSpeed * Time.deltaTime);
        }
        else if (backCollision && (leftCollision || rightCollision))
        {
            if (leftCollision)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(xLeftWall + sideDistance, yPosition, zBackWall - (transform.forward * currentCameraLook.cameraDistance).z + backwardsDistance), cameraFollowSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(xRightWall - sideDistance, yPosition, zBackWall - (transform.forward * currentCameraLook.cameraDistance).z + backwardsDistance), cameraFollowSpeed * Time.deltaTime);
            }
            //Clamp to corner position
        }
        else if (leftCollision || rightCollision)
        {
            if (leftCollision)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(xLeftWall + sideDistance, yPosition, (target.position - (transform.forward * currentCameraLook.cameraDistance)).z), cameraFollowSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(xRightWall - sideDistance, yPosition, (target.position - (transform.forward * currentCameraLook.cameraDistance)).z), cameraFollowSpeed * Time.deltaTime);
            }
        }
        else //Backwards hit
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, yPosition, zBackWall - (transform.forward * currentCameraLook.cameraDistance).z + backwardsDistance), cameraFollowSpeed * Time.deltaTime);
        }

    }
    #endregion

    #region Cross Door Methods
    private void CrossDoorMovement()
    {
        transform.position = Vector3.Lerp(transform.position, moveAtPoint + (Vector3.up * currentCameraLook.cameraYOffset) - (transform.forward * currentCameraLook.cameraDistance), Time.deltaTime / crossDoorCameraSpeed);
    }

    public void MoveAtPoint(Vector3 referencePoint, bool isRightSide)
    {
        moveAtPoint = referencePoint + (Vector3.down * referencePoint.y) + (Vector3.up * GameManager.Instance.GetPlayer().position.y) + ((isRightSide ? -1 : 1) * Vector3.right * sideDistance);
    }

    public void EndCrossDoorMovement()
    {
        ChangeCameraBehaviourState(CameraState.Following);
        ResetWallDetection();
    }
    #endregion

    #region Public Methods
    public void ChangeCameraBehaviourState(CameraState newState)
    {
        currentState = newState;
    }

    public void ChangeCameraLookState(CameraLookState newLookState)
    {
        currentCameraLookState = newLookState;

        switch (currentCameraLookState)
        {
            case CameraLookState.Normal:
                currentCameraLook = normalCamera;
                break;
            case CameraLookState.LookUp:
                currentCameraLook = lookUpCamera;
                break;
            case CameraLookState.LookDown:
                currentCameraLook = lookDownCamera;
                break;
        }
    }

    public void ResetWallDetection()
    {
        wallsFound = false;
    }
    #endregion

    #region Unity Inspector/Gizmos Methods
    //Private Unity Methods
    private void OnValidate()
    {
        if (target == null)
            return;

        switch (currentCameraLookState)
        {
            case CameraLookState.Normal:
                currentCameraLook = normalCamera;
                break;
            case CameraLookState.LookUp:
                currentCameraLook = lookUpCamera;
                break;
            case CameraLookState.LookDown:
                currentCameraLook = lookDownCamera;
                break;
        }

        if (currentCameraLook.cameraAngle != transform.eulerAngles.x)
        {
            transform.rotation = Quaternion.Euler(Vector3.right * currentCameraLook.cameraAngle);
        }
        if (currentCameraLook.cameraDistance != (target.position - transform.position).magnitude)
        {
            transform.position = target.position + (Vector3.up * currentCameraLook.cameraYOffset) - (transform.forward * currentCameraLook.cameraDistance);
        }
        if (currentCameraLook.cameraYOffset != (transform.position - target.position + (transform.forward * currentCameraLook.cameraDistance)).y)
        {
            transform.position = target.position + (Vector3.up * currentCameraLook.cameraYOffset) - (transform.forward * currentCameraLook.cameraDistance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        backwards = transform.position - target.position;
        backwards.y = 0;
        backwards.Normalize();

        Gizmos.color = Color.white;
        Gizmos.DrawLine(target.transform.position, target.transform.position + (transform.right * sideDistance));
        Gizmos.DrawLine(target.transform.position, target.transform.position - (transform.right * sideDistance));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(target.transform.position, target.transform.position + (backwards * backwardsDistance));
    }
    #endregion
}
