using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarMask : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    public Player Player { get => _player; set => _player = value; }

    [SerializeField]
    private float _minX;
    public float MinX { get => _minX; set => _minX = value; }

    [SerializeField]
    private float _maxX;
    public float MaxX { get => _maxX; set => _maxX = value; }

    private void Update() =>  transform.localPosition = Vector3.right * ((_player.health / 100.0f) * (_maxX - _minX) + _minX);
}
