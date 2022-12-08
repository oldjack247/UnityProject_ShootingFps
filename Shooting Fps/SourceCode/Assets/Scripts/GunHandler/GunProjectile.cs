using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunProjectile : MonoBehaviour
{
    public bool shootingEnabled = true;

    [Header("Attatch bullet prefab")]
    public GameObject bullet;

    [Header("Stats")]
    public float shootForce, upwardForce;
    public float timeBetweenShooting, reloadTime, rotationTime, timeBetweenShots, timeBetweenScope;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    private float accuracy;
    public float notAimingSpread, aimSpread;

    [Header("Recoil")]
    public Rigidbody playerRb;
    public float recoilForce;

    // Bools
    bool shooting, readyToShoot, reloading, aiming;

    [Header("References")]
    public Camera playerCam;
    public CamShake camShake;
    public float camShakeMagnitude;
    public float camShakeDuration;
    public GameObject muzzleFlash;
    public Transform attackPoint;
    public Vector3 scopePos;
    public float resetSmooth;
    private TMP_Text ammoText;
    public TextMeshProUGUI text;
    private Vector3 originalPos;
    private Quaternion originalRot;

    public bool allowInvoke = true;

    public GunState state;
    public enum GunState
    {
        notAiming,
        aiming
    }

    private void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        aimSpread = 0;

        originalPos = transform.localPosition;
        originalRot = transform.localRotation;

    }
    void Update()
    {
        MyInput();
        StateHandler();

        //Set Text
        text.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    private void MyInput()
    {
        //Input
        if (allowButtonHold) 
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (aiming = Input.GetKey(KeyCode.Mouse1) && !reloading)
            StartScoping();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            if (bulletsLeft <= 0)
                Reload();
        }

        //Scope
        if (aiming && !reloading)
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.Lerp(transform.localPosition, aiming ? scopePos : Vector3.zero, resetSmooth * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(originalRot, Quaternion.identity, resetSmooth * Time.deltaTime);
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, resetSmooth * Time.deltaTime);
        }

        //Reload
        if (reloading)
        {
            rotationTime += Time.deltaTime;
            var spinDelta = -(Mathf.Cos(Mathf.PI * (rotationTime / reloadTime)) - 1f) / 2;
            transform.localRotation = Quaternion.Euler(new Vector3(spinDelta * 360f, 0, 0));
        }

        
    }

    private void StateHandler()
    {
        // Mode - Null
        if (shooting)
        {
            state = GunState.notAiming;
            accuracy = notAimingSpread;
        }

        if (aiming)
        {
            state = GunState.aiming;
            accuracy = aimSpread;
        }
            
    }

    private void Shoot()
    {
        if (!shootingEnabled) return;

        readyToShoot = false;

        //Find the hit position using a raycast
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //Check if the ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        //Calculate direction
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Spread
        float x = Random.Range(-accuracy, accuracy);
        float y = Random.Range(-accuracy, accuracy);
        float z = Random.Range(-accuracy, accuracy);

        //Calc Direction with Spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, z);

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        //AddForce
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCam.transform.up * upwardForce, ForceMode.Impulse);

        //Activate bullet
        if (currentBullet.GetComponent<CustomProjectiles>()) currentBullet.GetComponent<CustomProjectiles>().activated = true;

        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        //Shake Camera
        camShake.StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        bulletsLeft--;
        bulletsShot--;

        if (allowInvoke)
        {
            Invoke("ShotReset", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil force
            transform.localPosition -= new Vector3(0, 0, recoilForce);
        }

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void ShotReset()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void StartScoping()
    {
        aiming = true;
    }

    private void Reload()
    {
        reloading = true;

        rotationTime = 0;    

        Invoke("ReloadingFinished", reloadTime);
    }
    private void ReloadingFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    public bool Aiming => aiming;

    #region Setters

    public void SetShootForce(float v)
    {
        shootForce = v;
    }
    public void SetUpwardForce(float v)
    {
        upwardForce = v;
    }
    public void SetFireRate(float v)
    {
        float _v = 2 / v;
        timeBetweenShooting = _v;
    }
    public void SetSpread(float v)
    {
        accuracy = v;
    }
    public void SetMagazinSize(float v)
    {
        int _v = Mathf.RoundToInt(v);
        magazineSize = _v;
    }
    public void SetBulletsPerTap(float v)
    {
        int _v = Mathf.RoundToInt(v);
        bulletsPerTap = _v;
    }

    #endregion


}
