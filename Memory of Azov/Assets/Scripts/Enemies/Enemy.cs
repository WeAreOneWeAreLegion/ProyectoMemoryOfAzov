using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Enemy : LightenableObject {

    public enum AnimationState { Move, Stun, Attack }
    public enum TargetType { Player, Surrogate }

    #region Public Variables
    [Header("\tGame Designers Variables")]

    [Header("Movement Variables")]
    [Tooltip("Velocidad a la que se mueve el personaje")]
    [Range(0, 200)] public float speed = 100f;
    [Tooltip("Velocidad a la que rota el personaje")]
    [Range(0, 360)] public float rotationSpeed = 120f;
    [Tooltip("Velocidad a la que oscila el personaje")]
    [Range(0, 200)] public float oscilationSpeed = 40;
    [Tooltip("Cuantas veces ira mas rapido mientras lo enfocan")]
    [Range(1, 5)] public float speedFactorWhenLightened = 2;
    [Tooltip("Cantidad de veces que se mueve de lado a lado por segundo")]
    [Range(0, 5)] public float oscilationsPerSecond = 1f;
    [Tooltip("Longitud de la oscilacion de lado a lado (Linea verde)")]
    [Range(1, 5)] public float oscilationAmplitude = 2;
    [Tooltip("Mira automaticamente al objectivo al qual esta dirigiendose si esta activado")]
    public bool immediateFacing;
    [Tooltip("Se mueve oscilatoriamente si esta activado")]
    public bool oscillationMovement;

    [Header("Health Variables")]
    [Tooltip("Vida inicia del personaje")]
    public int initialHp = 100;
    [Tooltip("Tiempo que el personaje estara aturdido si lo aturden")]
    [Range(1, 4)] public float timeStuned = 2f;

    [Header("Attack Variables")]
    [Tooltip("Valor del ataque del personaje")]
    public int ghostDamage = 20;
    [Tooltip("Radio de ataque del personaje")]
    [Range(0, 6)] public float attackRadius = 2f;
    [Tooltip("Tiempo entre ataques")]
    [Range(1, 4)] public float attackDelay = 2f;

    [Header("\t    Own Script Variables")]

    [Header("Component Variables")]
    [Tooltip("Meshes donde se aplicaran shaders i efectos")]
    public List<MeshRenderer> myMeshRenderers;

    [Header("Drop Variables")]
    [Tooltip("Objeto que deja caer el personaje al morir")]
    public ObjectsManager.ItemRequest itemToDrop = ObjectsManager.ItemRequest.Health;

    [Header("Debugation Variables")]
    [Tooltip("Variable que indica si se debuga en tiempo de ejecucion (Uso solo para testear acciones)")]
    public bool debugInRuntime;

    public Transform surrogateTarget;
    #endregion

    #region Private Variables
    protected float oscilatorLifeTime;
    protected float currentHp;
    protected float initialSpeed;
    protected bool recievingDamage;
    protected bool isStunned;
    protected bool isInvincible;
    protected int lanternDamage = 0;

    protected Transform target;
    protected Vector3 scapePoint;
    protected Vector3 direction;
   
    protected Material myMat;
    protected Rigidbody myRGB;
    protected Animator myAnimator;
    protected AudioSource myAudioSource;
    
    protected IEnemyState currentState;
    protected PlayerController.LightColor currentGhostColor;
    #endregion

    protected virtual void Start()
    {
        //Tag
        if (tag != GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Enemy))
            tag = GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Enemy);

        //Components
        myRGB = GetComponent<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>();
        myAudioSource = GetComponent<AudioSource>();
        myMat = GetComponentInChildren<MeshRenderer>().material;

        //Variables initalization
        initialSpeed = speed;
        currentHp = initialHp;
        oscilatorLifeTime = -0.75f;
        target = GameManager.Instance.GetPlayer();

        if (itemToDrop == ObjectsManager.ItemRequest.Gem)
            GameManager.Instance.IncreaseMaxGemsQuantity(gameObject);
    }

    protected virtual void Update()
    {
        if (Time.timeScale == 0)
            return;

        currentState.Execute();

        CheckPlayerDistance();

        if (recievingDamage)
            RecieveDamage();

    }

    #region Set-Up Method
    public virtual void SetUpEnemyVariables(EnemySO enemyData)
    {
        currentGhostColor = enemyData.ghostColor;

        switch (currentGhostColor)
        {
            case PlayerController.LightColor.Neutral:
                break;
            case PlayerController.LightColor.Secondary:
                break;
            case PlayerController.LightColor.Third:
                break;
        }

        speed = enemyData.speed;
        rotationSpeed = enemyData.rotationSpeed;
        oscilationSpeed = enemyData.oscilationSpeed;
        speedFactorWhenLightened = enemyData.speedFactorWhenLightened;
        oscilationsPerSecond = enemyData.oscilationsPerSecond;
        oscilationAmplitude = enemyData.oscilationAmplitude;
        immediateFacing = enemyData.immediateFacing;
        oscillationMovement = enemyData.oscillationMovement;

        initialHp = enemyData.initialHp;
        timeStuned = enemyData.timeStuned;

        ghostDamage = enemyData.ghostDamage;
        attackRadius = enemyData.attackRadius;
        attackDelay = enemyData.attackDelay;

        itemToDrop = enemyData.itemToDrop;

        //Start variables
        initialSpeed = speed;
        currentHp = initialHp;
        oscilatorLifeTime = -0.75f;
        target = GameManager.Instance.GetPlayer();

        GameManager.Instance.CreateEnemyHUD(transform, (int)currentHp);
    }
    #endregion

    #region State Machine Method
    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        currentState.Enter(this);
    }
    #endregion

    #region Action Methods
    public void MoveToTarget()
    {
        Vector3 myPosition = transform.position;
        Vector3 myTargetPosition = target.position;

        if (oscillationMovement)
        {
            float sinValue = Mathf.Sin((Time.time - oscilatorLifeTime) * oscilationsPerSecond);
            direction = transform.forward * speed + (transform.right * oscilationAmplitude * sinValue * oscilationSpeed);
        }
        else
        {
            direction = myTargetPosition - myPosition;
            direction.Normalize();

            direction *= speed;
        }

        if (debugInRuntime)
            Debug.DrawLine(myPosition, myPosition + (direction * 3), Color.blue, -1, false);

        myRGB.velocity = direction * Time.deltaTime;
        Debug.DrawLine(myPosition, myPosition + (transform.forward * myRGB.velocity.magnitude), Color.red, -1, false);
    }

    public void RotateToTarget()
    {
        Vector3 myPosition = transform.position;
        Vector3 myTargetPosition = target.position;

        Vector3 targetDirection = myTargetPosition - myPosition;

        if (debugInRuntime)
            Debug.DrawLine(myPosition, myPosition + targetDirection, Color.green, -1, false);

        if (immediateFacing)
        {
            transform.LookAt(myPosition + targetDirection);
        }
        else
        {
            if (Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up) > 0.2f)
            {
                //Left
                transform.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
                if (Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up) < 0)
                {
                    transform.LookAt(myPosition + targetDirection);
                }
            }
            else if (Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up) < -0.2f)
            {
                //Right
                transform.rotation *= Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0);
                if (Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up) > 0)
                {
                    transform.LookAt(myPosition + targetDirection);
                }
            }
        }
    }

    public virtual void CalculateEscapePoint()
    {
        scapePoint = GameManager.Instance.player.GetOutsideLightPoint(transform.position, 5);
        Debug.DrawLine(transform.position, scapePoint, Color.red, -1, false);

        surrogateTarget.position = scapePoint;
    }

    public virtual void EscapeMove()
    {
        direction = (surrogateTarget.position - transform.position).normalized;

        direction *= (speed / 2);

        myRGB.velocity = direction * Time.deltaTime;
    }

    public void DoDamage()
    {
        if (Vector3.Distance(target.position, transform.position) <= attackRadius)
        {
            GameManager.Instance.player.RecieveDamage(ghostDamage);
        }
    }

    public void Invencible()
    {
        isInvincible = true;
    }

    public void Vulnerable()
    {
        isInvincible = false;
    }

    public void StopMovement()
    {
        myRGB.velocity = Vector3.zero;
    }
    #endregion

    #region Sound Methods
    public void PlayAwakenSound()
    {
        myAudioSource.clip = SoundManager.Instance.GetSoundByRequest(SoundManager.SoundRequest.E_Cry);
        myAudioSource.Play();
    }
    #endregion

    #region Animation Methods
    public void ChangeAnimation(AnimationState animState)
    {
        myAnimator.ResetTrigger("Attack");

        switch (animState)
        {
            case AnimationState.Move:
                myAnimator.SetBool("IsStunned", false);
                break;
            case AnimationState.Stun:
                myAnimator.SetBool("IsStunned", true);
                break;
            case AnimationState.Attack:
                myAnimator.SetTrigger("Attack");
                break;
        }
    }
    #endregion

    #region Lighten Methods
    protected void CheckPlayerDistance()
    {
        if (Vector3.Distance(GameManager.Instance.GetPlayer().position, transform.position) < GameManager.Instance.player.GetInitialLanternLength())
        {
            GameManager.Instance.player.OnGhostEnter(this.gameObject);
        }
        else
        {
            GameManager.Instance.player.OnGhostExit(this.gameObject);
            OutsideLanternRange();
        }
    }

    public void InsideLanternRange(int damageToRecieve, bool stun)
    {
        if (isInvincible || !GameManager.Instance.player.IsCurrentLightOfColor(currentGhostColor))
            return;

        if (stun)
            ChangeState(new StunState_N());

        recievingDamage = true;
        lanternDamage = damageToRecieve;

        myMat.SetColor("_RimColor", Color.magenta);
        myMeshRenderers.ForEach(x => x.material = myMat);
        speed = initialSpeed * speedFactorWhenLightened;
    }

    public override void OutsideLanternRange()
    {
        if (currentHp > 0)
        {
            recievingDamage = false;
            lanternDamage = 0;

            Color ghostColor = Color.white;

            switch (currentGhostColor)
            {
                case PlayerController.LightColor.Neutral:
                    ghostColor = GameManager.Instance.player.neutralColor;
                    break;
                case PlayerController.LightColor.Secondary:
                    ghostColor = GameManager.Instance.player.secondColor;
                    break;
                case PlayerController.LightColor.Third:
                    ghostColor = GameManager.Instance.player.thirdColor;
                    break;
            }

            myMat.SetColor("_RimColor", ghostColor);
            myMeshRenderers.ForEach(x => x.material = myMat);
            speed = initialSpeed;
        }
    }
    #endregion

    #region Health Methods
    public virtual void RecieveDamage()
    {
        currentHp -= lanternDamage * Time.deltaTime;

        GameManager.Instance.ModifyEnemyHp(transform, Mathf.RoundToInt(currentHp));

        if (currentHp <= 0)
        {
            GameObject go = ObjectsManager.Instance.GetItem(this.transform, itemToDrop);

            if (go != null)
                go.transform.position = transform.position;

            myRGB.velocity = Vector3.zero;

            GameManager.Instance.DestroyEnemyHUD(transform);
            GameManager.Instance.IncreseNumOfGhostsCaptured();
            EnemyManager.Instance.ReturnEnemy(gameObject);
        }

    }
    #endregion

    #region Getter Methods
    public bool IsInAttackRadius()
    {
        return Vector3.Distance(target.position, transform.position) < attackRadius;
    }

    public bool IsCloseToScapePoint()
    {
        return Vector3.Distance(scapePoint, transform.position) < 1;
    }

    public override bool IsInSight()
    {
        return recievingDamage;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    public bool IsSoundPlaying()
    {
        return myAudioSource.isPlaying;
    }

    public bool IsAttackAnimationPlaying()
    {
        return myAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack");
    }

    public float GetStunTimer()
    {
        return timeStuned;
    }

    public float GetAttackDelay()
    {
        return attackDelay;
    }
    #endregion

    #region Setter Methods
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    #endregion

    #region Unity Gizmos Method
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + (transform.right * oscilationAmplitude / 2));
        Gizmos.DrawLine(transform.position, transform.position - (transform.right * oscilationAmplitude / 2));
    }
    #endregion
}
