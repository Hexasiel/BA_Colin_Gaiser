using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] private float speedMod = 5f;
    [SerializeField] private float weaponCooldown = 0.5f;
    private bool canShoot = true;
    public int gold = 0;
    public GameUI gameUI;

    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject bulletPrefab;

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
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speedMod * Time.deltaTime);
            sprite.flipX = true;
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GetComponent<Rigidbody2D>().AddForce(Vector2.up * 300);
        //}
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (canShoot)
            {
                Shoot();
                StartCoroutine(WeaponCooldown());
            }
        }
    }
    
    void Shoot()
    {
        Vector3 directon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directon.z = 0;
        directon = (directon - transform.position).normalized;


        Rigidbody2D bulletrb = Instantiate(bulletPrefab,transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletrb.AddForce(directon * 1500);
    }

    IEnumerator WeaponCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(weaponCooldown);
        canShoot = true;
    }

}
