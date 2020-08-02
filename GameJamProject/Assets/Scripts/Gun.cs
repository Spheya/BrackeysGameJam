using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private float _distance = 2.0f;
    public float Distance { get => _distance; set => _distance = value; }

    [SerializeField]
    private GameObject _parent;
    public GameObject Parent { get => _parent; set => _parent = value; }

    [SerializeField]
    private Recording _recording;
    public Recording Recording { get => _recording; set => _recording = value; }

    [SerializeField]
    private Bullet _bullet;
    public Bullet Bullet { get => _bullet; set => _bullet = value; }

    [SerializeField]
    private float _shootCooldown;
    public float ShootCooldown { get => _shootCooldown; set => _shootCooldown = value; }

    private bool _canShoot = true;

    private void Update()
    {
        UpdatePosition();
        UpdateShooting();
    }

    private void UpdatePosition()
    {
        // Get the direction the player is aiming in
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDir = aimPos - Parent.transform.position;
        aimDir.z = 0.0f;

        // Set the length of that direction to the distance
        Vector3 offset = aimDir.normalized * Distance;
        offset.z = offset.y;
        transform.position = _parent.transform.position + offset;

        // Rotate towards that direction and flip if the gun appears upside down
        float rotation = (Mathf.Atan2(offset.y, offset.x) * 180.0f / Mathf.PI + 180.0f) % 360.0f;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);
        transform.localScale = new Vector3(1.0f, (rotation < 270.0f && rotation > 90.0f) ? -1.0f : 1.0f, 1.0f);
    }

    private void UpdateShooting()
    {
        if (Input.GetMouseButton(0))
            TryShoot();
    }

    public bool TryShoot()
    {
        if (_canShoot)
        {
            Shoot();
            _canShoot = false;
            StartCoroutine(CanShootCoroutine());

            return true;
        }
        return false;
    }

    public IEnumerator CanShootCoroutine()
    {
        yield return new WaitForSeconds(_shootCooldown);
        _canShoot = true;
    }

    private void Shoot()
    {
        var bullet = Instantiate(Bullet);
        bullet.transform.position = transform.position + Vector3.forward * (_distance + 0.1f);
        bullet.transform.rotation = transform.rotation;
        bullet.transform.localScale = transform.localScale;
    }
}
