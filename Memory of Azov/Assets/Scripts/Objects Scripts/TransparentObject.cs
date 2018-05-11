using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObject : MonoBehaviour {

    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Genero fantasma?")]
    public bool spawnGhost;
    [Tooltip("Genero gema?")]
    public bool spawnGem;
    [Tooltip("Llamo a todos los fantasmas de la sala?")]
    public bool spawnAllGhosts;
    [Tooltip("La referencia de como sera el fantasma a generar")]
    public EnemySO enemyData;

    [Header("\t    Own Script Variables")]
    [Tooltip("Soy un muro?")]
    public bool isWall;
    [Tooltip("Soy una puerta? (Esta variable impide que haga shake el objeto tambien)")]
    public bool isDoor;
    [Tooltip("Soy estatico? (No shake)")]
    public bool isStatic;
    public List<Renderer> myRenderers = new List<Renderer>();
    public List<Animation> myAnimations = new List<Animation>();
    public AnimationClip shakeAnimation;
    public AnimationClip openAnimation;

    private Transform player;
    #endregion

    #region Private Variables
    private bool isMaterialHidden = false;
    private float transparencyValue;

    private bool isHorizontalDoor;

    private List<Material> myMats = new List<Material>();
    #endregion

    private void Start()
    {
        transparencyValue = isWall ? GameManager.Instance.wallsHidenByCameraTransparency : GameManager.Instance.objectsHidenByCameraTransparency;

        if (!isWall && !isDoor)
        {
            foreach (Animation a in myAnimations)
            {
                a.AddClip(shakeAnimation, "Shake");
                a.AddClip(openAnimation, "Open");
            }
        }

        if (isDoor)
        {
            isHorizontalDoor = transform.parent.eulerAngles.y != 0 && transform.parent.eulerAngles.y != 180;
        }

        foreach (Renderer r in myRenderers)
            myMats.Add(r.material);

        player = GameManager.Instance.GetPlayer();

        if (spawnGem)
            GameManager.Instance.IncreaseMaxGemsQuantity(gameObject);
    }

    private void Update()
    {
        if ((isDoor && !isHorizontalDoor))
        {
            return;
        }
        if (isMaterialHidden)
        {
            if (isWall)
            {
                if (Mathf.Abs((Camera.main.transform.position - transform.position).z) > Mathf.Abs((Camera.main.transform.position - player.position).z))
                {
                    ShowMaterial();
                }
            }
            else
            {
                if (Mathf.Abs((Camera.main.transform.position - transform.position).z) + GameManager.Instance.transparencyOffsetForward > Mathf.Abs((Camera.main.transform.position - player.position).z) ||
                    (Mathf.Abs((transform.position - player.position).x) > GameManager.Instance.transparencyOffsetLateral && !isDoor))
                {
                    ShowMaterial();
                }
            }
        }
        else
        {
            if (isWall)
            {
                if (Mathf.Abs((Camera.main.transform.position - transform.position).z) < Mathf.Abs((Camera.main.transform.position - player.position).z))
                {
                    HideMaterial();
                }
            }
            else
            {
                if (Mathf.Abs((Camera.main.transform.position - transform.position).z) + GameManager.Instance.transparencyOffsetForward < Mathf.Abs((Camera.main.transform.position - player.position).z) &&
                Mathf.Abs((transform.position - player.position).x) < GameManager.Instance.transparencyOffsetLateral)
                {
                    HideMaterial();
                }
            }
        }
    }

    #region Hidden Methods
    public bool IsMaterialHidden()
    {
        return isMaterialHidden;
    }

    public void HideMaterial()
    {
        for (int i = 0; i < myMats.Count; i++)
        {
            StandardShadersUtil.ChangeRenderMode(myMats[i], StandardShadersUtil.BlendMode.Fade);
            myMats[i].color = new Color(myMats[i].color.r, myMats[i].color.g, myMats[i].color.b, transparencyValue);
            myRenderers[i].material = myMats[i];
            isMaterialHidden = true;
        }
    }

    public void ShowMaterial()
    {
        for (int i = 0; i < myMats.Count; i++)
        {
            StandardShadersUtil.ChangeRenderMode(myMats[i], StandardShadersUtil.BlendMode.Opaque);
            myMats[i].color = new Color(myMats[i].color.r, myMats[i].color.g, myMats[i].color.b, 1f);
            myRenderers[i].material = myMats[i];
            isMaterialHidden = false;
        }
    }
    #endregion

    #region Action Methods
    public void ShakeObjectAnimation(bool isFirstCall = true)
    {
        if (isWall || isDoor || isStatic)
        {
            return;
        }

        if (myAnimations[0].isPlaying)
        {
            return;
        }

        if (spawnGhost)
        {
            Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);

            RaycastHit hit;

            Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

            GameObject go = EnemyManager.Instance.GetEnemy(hit.transform != null ? hit.transform.parent : this.transform, enemyData);

            go.transform.position = new Vector3(transform.position.x, hit.point.y + EnemyManager.Instance.enemyFloorYOffset, transform.position.z);
            go.transform.forward = transform.forward;

            spawnGhost = false;

            if (isFirstCall && spawnAllGhosts)
            {
                hit.transform.parent.GetComponent<RoomScript>().ShowAllEnemiesFromRoom();
            }
        }

        if (spawnGem)
        {
            GameObject go = ObjectsManager.Instance.GetItem(this.transform, ObjectsManager.ItemRequest.Gem);

            go.transform.position = transform.position;
            go.GetComponent<GemObject>().DiscoveredByFeature();

            spawnGem = false;
        }

        foreach (Animation a in myAnimations)
        {
            a.clip = a.GetClip("Shake");
            a.Play();
        }
    }

    public void OpenObject()
    {
        //If an object must be open like a room feature.
    }
    #endregion

}