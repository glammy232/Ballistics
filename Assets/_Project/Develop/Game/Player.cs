using System;

using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int ID;

    private float _speedKoaf;
    public float SpeedKoaf
    {
        get => _speedKoaf;
        set
        {
            if (value > 0)
            {
                _speedKoaf = (float)Math.Round(value, 2);
            }
        }
    }

    public bool CanFire;

    private const float FIRE_COOLDOWN = 0.5f;

    public const int MAX_HEALTH = 100;

    private int _health;
    public int Health
    {
        get => _health;

        set
        {
            _health = value;
            _field.UpdateHealthView(this);

            if (value <= 0)
                GameController.Instance.AddPlayerToKillHim(this, true);
        }
    }

    [SerializeField] private float _minDistance;

    [SerializeField] private float _maxProjectileSpeed;

    [SerializeField] private Bullet _bulletTemplate;

    private Vector3 _startTouchPosition;
    private Vector3 _lastTouchPosition;

    private Field _field;

    private float _lastFireTime;

    public void Initialize(Field field, int id)
    {
        ID = id;

        _field = field;

        _minDistance = 0f; _maxProjectileSpeed = 1000000f;

        Health = MAX_HEALTH;

        //SpeedKoaf = 4f;
        SpeedKoaf = SettingsModel.SpeedKoaf;//Test
    }

    private void FixedUpdate()
    {
        if (!CanFire)
            return;

        if (Input.touchCount > 0 && _startTouchPosition == Vector3.zero)
        {
            Vector3 screenPosition = Input.mousePosition;

            Vector3 worldPosition = new Vector3(screenPosition.x, screenPosition.y, 10f);

            worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);

            _startTouchPosition = worldPosition;
        }
        else if (Input.touchCount > 0 && _startTouchPosition != Vector3.zero)
        {
            Vector3 screenPosition = Input.mousePosition;

            Vector3 worldPosition = new Vector3(screenPosition.x, screenPosition.y, 10f);

            worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);

            _lastTouchPosition = worldPosition;
        }
        else if(Input.touchCount == 0 && _startTouchPosition != Vector3.zero && _lastTouchPosition != Vector3.zero && Vector3.Distance(_startTouchPosition, _lastTouchPosition) > _minDistance && _lastFireTime > SettingsModel.FireCooldown)
        {
            Bullet bullet = Instantiate(_bulletTemplate, transform.position + new Vector3(0, transform.localScale.y / 2f + _bulletTemplate.transform.localScale.y / 2f, 0), Quaternion.identity);

            GameController.Instance.Bullets.Add(bullet);
            
            float speed = Vector3.Distance(_startTouchPosition, _lastTouchPosition) * SpeedKoaf;

            _startTouchPosition = _startTouchPosition - _lastTouchPosition;
            _lastTouchPosition = Vector3.zero;

            Vector3 finalDirection = _lastTouchPosition - _startTouchPosition;

            float lengthOffinalDirection = Mathf.Sqrt(finalDirection.x * finalDirection.x + finalDirection.z * finalDirection.z);

            float cos = _startTouchPosition.x / lengthOffinalDirection;  

            float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;

            if (_startTouchPosition.z < _lastTouchPosition.z)
            {
                angle = 180 + 180 - angle;
            }

            _lastTouchPosition = Vector3.zero;

            bullet.Initialize(SettingsModel.MaxHeight, speed, angle, Bullet.BulletType.Tomato);

            _startTouchPosition = Vector2.zero;
            _lastTouchPosition = Vector2.zero;

            _lastFireTime = 0;
        }
        else
        {
            _startTouchPosition = Vector2.zero;
            _lastTouchPosition = Vector2.zero;
        }

        _lastFireTime += Time.fixedDeltaTime;
    }
}
