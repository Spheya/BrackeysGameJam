using System.Collections;
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

    [SerializeField]
    private AudioClip[] _shootClips;
    private AudioSource audioSource;

    [SerializeField]
    private bool _doUpdate = true;
    public bool DoUpdate { get => _doUpdate; set => _doUpdate = value; }

    private bool _canShoot = true;

    private Vector3 aimPosLast = Vector3.zero;
    private Vector3 aimDirLast = Vector3.right;
    private bool usingMouse = true; // false when using controller

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_doUpdate)
        {
            UpdatePosition();
            UpdateShooting();
        }
    }

    private void UpdatePosition()
    {
        // Get the direction the player is aiming in
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (aimPos != aimPosLast)
        {
            usingMouse = true;
            aimPosLast = aimPos;
        }
        Vector3 aimDir = new Vector3(Input.GetAxisRaw("Right Joystick X"), Input.GetAxisRaw("Right Joystick Y"), 0.0f);
        if (aimDir.magnitude > 0.1)
        {
            usingMouse = false;
            aimDirLast = aimDir;
        }

        if (usingMouse)
        {
            aimDirLast = aimPosLast - Parent.transform.position;
        }

        AimAt(aimDirLast);
    }

    public void AimAt(Vector2 aimDir)
    {
        // Set the length of that direction to the distance
        Vector3 offset = aimDir.normalized * Distance;
        offset.z = offset.y / 100.0f;
        transform.position = _parent.transform.position + offset;

        // Rotate towards that direction and flip if the gun appears upside down
        float rotation = (Mathf.Atan2(offset.y, offset.x) * 180.0f / Mathf.PI + 180.0f) % 360.0f;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);
        transform.localScale = new Vector3(1.0f, (rotation < 270.0f && rotation > 90.0f) ? -1.0f : 1.0f, 1.0f);
    }

    private void UpdateShooting()
    {
        if (DoUpdate)
        {
            if (Input.GetMouseButton(0))
            {
                usingMouse = true;
                UpdatePosition();
                TryShoot();
            }
            else if (Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.Joystick1Button5))
            {
                usingMouse = false;
                UpdatePosition();
                TryShoot();
            }
        }
    }

    public bool TryShoot()
    {
        if (_canShoot)
        {
            Shoot();
            _canShoot = false;
            StartCoroutine(CanShootCoroutine());

            audioSource.clip = _shootClips[Random.Range(0, 9)];
            audioSource.Play();

            return true;
        }
        return false;
    }

    public IEnumerator CanShootCoroutine()
    {
        yield return new WaitForSeconds(_shootCooldown);
        _canShoot = true;
    }

    public void Shoot()
    {
        var bullet = Instantiate(Bullet);
        bullet.transform.position = transform.position + Vector3.forward * (_distance + 0.1f);
        bullet.transform.rotation = transform.rotation;
        bullet.transform.localScale = transform.localScale;

        if (_recording && _doUpdate)
        {
            Vector3 offset = transform.position - _parent.transform.position;
            _recording.RecordBullet(new Vector2(offset.x, offset.y));
        }
    }
}
