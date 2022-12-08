using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunSystem : MonoBehaviour
{
    [Header("Throwing")]
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;

    [Header("Pickup")]
    public float animTime;

    [Header("Shooting")]
    public int maxAmmo;
    public int shotsPerSecond;
    public float hitForce;
    public float range;
    public bool tapAble;
    public float recoilForce;
    public float resetSmooth;
    public Vector3 scopePos;

    [Header("Data")]
    public int gunGfxLayer;
    public GameObject gunGfxs;
    public Collider gfxColliders;

    private float rotationTime;
    private float time;
    private bool held;
    private bool scoping;
    private bool reloading;
    private bool shooting;
    private int ammo;
    private Rigidbody rb;
    private Transform playerCam;
    private TMP_Text ammoText;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        ammo = maxAmmo;
    }

    private void Update()
    {
        


    }
}
