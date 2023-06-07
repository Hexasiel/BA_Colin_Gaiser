using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    //Properties
    [SerializeField] private float speedMod = 5f;
    [SerializeField] private float weaponCooldown = 0.5f;
    private bool canShoot = true;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashPower = 1f;
    [SerializeField] private float dashTime = 1f;
    private bool canDash = true;
    public int gold = 0;
    public GameUI gameUI;
    bool faceDirection = false;

    //External References
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] ParticleSystem dashParticleSystem;

    private void Awake()
    {
        instance = this;
        GoldPiece.OnGoldPieceCollected += PickUpGoldPiece;
    }

    void PickUpGoldPiece()
    {
        gold++;
        gameUI.SetGoldText();
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        } 
        if (Input.GetKey(KeyCode.Mouse1) && canShoot)
        {
            StartCoroutine(Shoot());
        }
    }
    
    IEnumerator Dash()
    {
        dashParticleSystem.Play();
        canDash = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (faceDirection){
            rb.velocity = new Vector2(dashPower, 0f);
        }
        else{
            rb.velocity = new Vector2(dashPower * -1, 0f);
        }
        yield return new WaitForSeconds(dashTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(weaponCooldown);
        canDash = true;
    }

    void Attack()
    {

    }

    IEnumerator Shoot()
    {
        canShoot= false;
        Vector3 directon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directon.z = 0;
        directon = (directon - transform.position).normalized;


        Rigidbody2D bulletrb = Instantiate(bulletPrefab,transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletrb.AddForce(directon * 1500);

        yield return new WaitForSeconds(weaponCooldown);
        canShoot= true;
    }

}
