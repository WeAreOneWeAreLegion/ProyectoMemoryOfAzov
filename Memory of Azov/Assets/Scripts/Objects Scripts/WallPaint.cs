using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPaint : LightenableObject
{
    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Tiempo que tardara a hacer aparecer el objeto en caso de que este siendo enfocado")]
    [Range(1,5)] public float timeToSpawnObject = 3f;
    [Header("Drop Variables")]
    [Tooltip("Tipo de drop")]
    public ObjectsManager.ItemRequest itemToDrop = ObjectsManager.ItemRequest.Gem;

    [Header("Enemy Variables")]
    [Tooltip("Spawneo Fantasma?")]
    public bool spawnGhost;
    [Tooltip("Fantasma a instanciar")]
    public EnemySO enemyData;

    [Header("\t    Own Script Variables")]
    [Tooltip("Particulas al ser enfocado")]
    public GameObject particles;
    #endregion

    #region Private Variables
    private bool insideRadius;
    private float timer;

    private Transform target;
    #endregion

	private void Start ()
    {
        target = GameManager.Instance.GetPlayer();
        particles.SetActive(false);

        if (itemToDrop == ObjectsManager.ItemRequest.Gem && !spawnGhost)
            GameManager.Instance.IncreaseMaxGemsQuantity(this.gameObject);
    }
	
	private void Update ()
    {
        CheckPlayerDistance();
        Lightened();
    }

    #region Lighten Methods
    private void CheckPlayerDistance()
    {
        if (Vector3.Distance(target.position, transform.position) < target.GetComponent<PlayerController>().lanternDamageLength)
        {
            target.GetComponent<PlayerController>().OnLightenObjectEnter(this.gameObject);
        }
        else
        {
            target.GetComponent<PlayerController>().OnLightenObjectExit(this.gameObject);
            OutsideLanternRange();
        }
    }

    private void Lightened()
    {
        if (insideRadius)
        {
            if (!particles.activeInHierarchy)
                particles.SetActive(true);

            timer += Time.deltaTime;

            //Vibrate
            InputsManager.Instance.VibrationByValue(timer / (timeToSpawnObject * 2));

            if (timer >= timeToSpawnObject) //Spawn object and deactive
            {
                if (spawnGhost)
                {
                    Ray ray = new Ray(transform.position - (transform.forward * 2) + Vector3.up, Vector3.down);

                    RaycastHit hit;

                    Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

                    GameObject go = EnemyManager.Instance.GetEnemy(hit.transform != null ? hit.transform.parent : this.transform, enemyData);

                    go.transform.position = new Vector3(transform.position.x, hit.point.y + EnemyManager.Instance.enemyFloorYOffset, transform.position.z);
                    go.transform.forward = -transform.forward;

                }
                else
                {
                    //Spawn object
                    GameObject go = ObjectsManager.Instance.GetItem(transform, itemToDrop);

                    if (go != null)
                    {
                        go.transform.position = transform.position - transform.forward;
                        go.transform.forward = -transform.forward;
                    }

                }

                InputsManager.Instance.DeactiveVibration();
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (timer > 0)
            {
                if (particles.activeInHierarchy)
                    particles.SetActive(false);

                InputsManager.Instance.DeactiveVibration();
                timer = 0;
            }
        }
    }

    public override void InsideLanternRange()
    {
        if (GameManager.Instance.GetIsInCombateMode())
        {
            return;
        }
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
}
