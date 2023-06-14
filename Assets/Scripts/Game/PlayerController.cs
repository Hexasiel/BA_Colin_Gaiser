using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour, IAttackable
{
    public static PlayerController instance;

    //Properties
    public int maxHealth = 500;
    public int m_health = 500;
    public int maxShield = 20;
    public int shield = 20;
    [SerializeField] private float speedMod = 5f;
    [SerializeField] private float gunCooldown = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private int attackDamage = 4;
    [SerializeField] private float attackKnockback = 4;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashPower = 1f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] LayerMask attackMask;

    //Internal variables
    public int gold = 0;
    private bool canDash = true;
    private bool canShoot = true;
    private bool canAttack = true;
    bool faceDirection = false;
    public bool battleMode = false;

    //Workshop Stats/Flags
    bool refillShieldOnDash = false;
    bool refillShieldOnKill = false;
    float[] dashCooldowns = new float[] { 1f, 1f, 0.75f, 0.5f };
    int[] damageValues = new int[] {10,15,20,20};

    //External References
    [SerializeField] Collider2D playerCollider;
    [SerializeField] GameObject shieldGO;
    [SerializeField] Animator animator;
    [SerializeField] Transform attackPos;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] ParticleSystem dashParticleSystem;
    public GameUI gameUI;

    private void Awake()
    {
        instance = this;
        GoldPiece.OnGoldPieceCollected += PickUpGoldPiece;
        Workshop.OnWorkshopLevelUpdated += UpdateWorkshopStats;
        Shrine.OnHealingTriggered += ReceiveHeal;
    }

    void ReceiveHeal(int health)
    {
        m_health += health;
        if (m_health > maxHealth) m_health = maxHealth;
    }

    void UpdateWorkshopStats()
    {
        refillShieldOnDash= false;
        refillShieldOnKill= false;
        switch(Workshop.workshopHighestLevel)
        {
            case 0: break;
            case 1: refillShieldOnKill = true; break;
            case 2: refillShieldOnKill = true; break;
            case 3: refillShieldOnKill = true; refillShieldOnDash = true; break;
        }
        attackDamage = damageValues[Workshop.workshopHighestLevel];
        dashCooldown = dashCooldowns[Workshop.workshopHighestLevel];   
    }

    // Update is called once per frame
    void Update()
    {
       CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            battleMode = !battleMode;
            gameUI.SwitchBattleMode();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && canDash)
        {
            StartCoroutine(Dash());
        }


        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speedMod * Time.deltaTime);
            sprite.flipX = false;
            faceDirection = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speedMod * Time.deltaTime);
            sprite.flipX = true;
            faceDirection = true;
        }

        //BattleMode Only
        if (!battleMode)
            return;
        //if (Input.GetKey(KeyCode.Mouse1) && canShoot)
        //{
        //    StartCoroutine(Shoot());
        //}
        if (Input.GetKey(KeyCode.Mouse0) && canAttack)
        {
            StartCoroutine(Attack());
        }

    }

    void RefillShield()
    {
        shield = maxShield;
        shieldGO.SetActive(true);
    }

    void IAttackable.GetDamage(int dmg)
    {
        if(shield > 0) {
            shield -= dmg;
            if(shield <= 0){
                shieldGO.SetActive(false); shield = 0;
            }
        }
        else
        {
            m_health -= dmg;
        }
        gameUI.UpdateUI();
    }

    void PickUpGoldPiece()
    {
        gold++;
        gameUI.UpdateUI();
    }

    IEnumerator Dash()
    {
        dashParticleSystem.Play();
        canDash = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        playerCollider.enabled = false;
        if (faceDirection){
            rb.velocity = new Vector2(dashPower, 0f);
        }
        else{
            rb.velocity = new Vector2(dashPower * -1, 0f);
        }
        yield return new WaitForSeconds(dashTime);
        playerCollider.enabled = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator Attack()
    {
        canAttack = false;

        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, attackMask);
        foreach (Collider2D collider2D in hitEnemies)
        {
            Vector2 knockback = (collider2D.transform.position - transform.position).normalized * attackKnockback;
            collider2D.gameObject.GetComponent<Enemy>().GetDamage(attackDamage, knockback);
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator Shoot()
    {
        canShoot= false;
        Vector3 directon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directon.z = 0;
        directon = (directon - transform.position).normalized;


        Rigidbody2D bulletrb = Instantiate(bulletPrefab,transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletrb.AddForce(directon * 1500);

        yield return new WaitForSeconds(gunCooldown);
        canShoot= true;
    }

}
