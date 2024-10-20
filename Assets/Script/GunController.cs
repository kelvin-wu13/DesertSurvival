using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 10f;
    public float fireRate = 15f;
    public int maxAmmo = 30;
    public float reloadTime = 5f;

    [Header("References")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public Animator gunAnimator; // New: Reference to the gun's Animator component

    private float nextTimeToFire = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        
        // If gunAnimator is not set in the inspector, try to get it from this GameObject
        if (gunAnimator == null)
        {
            gunAnimator = GetComponent<Animator>();
            if (gunAnimator == null)
            {
                Debug.LogError("Animator component not found on this GameObject or its children. Please assign it manually in the inspector.");
            }
            else
            {
                Debug.Log("Animator component found and assigned automatically.");
            }
        }
        else
        {
            Debug.Log("Animator component was pre-assigned in the inspector.");
        }
    }

    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }

    void Shoot()
    {
        currentAmmo--;

        Debug.Log("Shoot function called");  // Debug log

        // Trigger shoot animation
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Shoot");
            Debug.Log("Shoot animation triggered");  // Debug log
        }
        else
        {
            Debug.LogError("GunAnimator is null");  // Debug log
        }

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
        bullet.GetComponent<Bullet>().damage = damage;

        Destroy(bullet, 5f);
    }

    void StartReload()
    {
        if (currentAmmo == maxAmmo) return;

        isReloading = true;
        Debug.Log("StartReload function called");  // Debug log

        // Trigger reload animation
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Reload");
            Debug.Log("Reload animation triggered");  // Debug log
        }
        else
        {
            Debug.LogError("GunAnimator is null");  // Debug log
        }

        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        FinishReload();
    }

    void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload complete!");
    }
}