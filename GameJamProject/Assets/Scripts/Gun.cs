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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDir = aimPos - Parent.transform.position;
        aimDir.z = 0.0f;

        Vector3 offset = aimDir.normalized * Distance;
        offset.z = offset.y;

        transform.position = _parent.transform.position + offset;

        float rotation = (Mathf.Atan2(offset.y, offset.x) * 180.0f / Mathf.PI + 180.0f) % 360.0f;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);
        transform.localScale = new Vector3(1.0f, (rotation < 270.0f && rotation > 90.0f) ? -1.0f : 1.0f, 1.0f);
    }
}
