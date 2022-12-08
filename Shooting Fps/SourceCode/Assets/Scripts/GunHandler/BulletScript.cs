using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public bool activated;

    [Header("References")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Header("Set the basic stats:")]
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    [Header("Explosion:")]
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    [Header("Lifetime:")]
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;
    public bool explodeDirectlyAfterDrop = true;

    private int collisions;

    private PhysicMaterial physic_mat;
    public bool alreadyExploded;

    void Start()
    {
        Setup();
    }

    void Update()
    {
        if (!activated) 
            return;

        if (collisions >= maxCollisions && activated) 
            Explode();

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0 && activated) 
            Explode();
    }

    private void Setup()
    {
        //Setup physics material
        physic_mat = new PhysicMaterial();
        physic_mat.bounciness = bounciness;
        physic_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physic_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Apply the physics material to the collider
        GetComponent<SphereCollider>().material = physic_mat;
    }

    public void Explode()
    {
        //Bug fixing
        if (alreadyExploded) 
            return;
        alreadyExploded = true;

        Debug.Log("Explode");

        //Instantiate explosion if attatched
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        //Check for enemies and damage them
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            //Damage enemies
            if (enemies[i].GetComponent<EnemyAi>())
                enemies[i].GetComponent<EnemyAi>().TakeDamage(explosionDamage);

            //Add explosion force to enemies
            if (enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange, 2f);

            //Invoke destruction
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<TrailRenderer>().emitting = false;
            Invoke("Delay", 0.08f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!activated) 
            return;

        //Explode on touch
        if (explodeOnTouch && collision.collider.CompareTag("Enemy")) 
            Explode();

        //Count up collisions
        collisions++;
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    #region Setters

    public void SetBounciness(float v)
    {
        bounciness = v;
    }
    public void SetGravity(float v)
    {
        if (v == 1) useGravity = true;
        else useGravity = false;
    }
    public void SetMaxCollisions(float v)
    {
        int _v = Mathf.RoundToInt(v);
        maxCollisions = _v;
    }
    public void SetMaxLifetime(float v)
    {
        int _v = Mathf.RoundToInt(v);
        maxLifetime = _v;
    }
    public void SetExplosionRange(float v)
    {
        explosionRange = v;
    }
    public void SetExplosionDamage(float v)
    {
        int _v = Mathf.RoundToInt(v);
        explosionDamage = _v;
    }

    #endregion
}
