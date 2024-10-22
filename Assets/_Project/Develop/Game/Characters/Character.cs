using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static System.TimeZoneInfo;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;

    private int _kills;
    public int Kills
    {
        get => _kills;
        set
        {
            _kills = value;
        }
    }

    protected Field _field;

    public Field GetField => _field;

    private int _id;
    public int GetID => _id;

    private int _maxHealth;
    public int GetMaxHealth => _maxHealth;

    private int _health;
    public int Health
    {
        get => _health;

        set
        {
            _health = value;

            _field.UpdateHealthView(this);
            
            if (value < 100 && CanFire == false)
                CanFire = true;
            
            if (value <= 0)
            {
                GetComponent<Renderer>().material.color = Color.black;
            
                GameController.Instance.AddPlayerToKillHim(this, true);
            }
        }
    }

    protected bool _gotDamage;
    protected Character _damager;

    public void SetDamager(Character damager)
    {
        _damager = damager;
        _gotDamage = true;
    }

    private Vector3 _startPosition;
    public Vector3 StartPosition
    {
        get => _startPosition;
        set
        {
            _startPosition = value;
        }
    }

    #region FireSettings

    [SerializeField] protected Bullet _bulletTemplate;
    [SerializeField] protected Bullet _poopTemplate;

    protected bool _hasPoop;

    protected virtual bool hasPoop
    {
        get => _hasPoop;

        set
        {
            _hasPoop = value;

            if(value == true)
            {
                _field.ActivatePoopButtonObject();
                _field.SetPoopButtonListener(delegate
                {
                    UsePoop();
                });
            }
        }
    }

    public bool SetHasPoop(bool value) => hasPoop = value;

    protected bool _nextProjectileIsPoop;

    protected bool _canFire;
    public virtual bool CanFire
    {
        get => _canFire;
        set
        {
            _canFire = value;
        }
    }

    private float _speedKoaf;
    protected float speedKoaf
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

    protected float _minDistance;

    protected float _maxDistance;

    private float _maxProjectileSpeed;

    protected Vector3 _startTouchPosition;
    protected Vector3 _lastTouchPosition;

    protected float _lastFireTime;

    protected float _fireCooldown;

    #endregion

    public void DeathAnimation()
    {
        GetComponent<Renderer>().material.DOColor(Color.black, 1f);
    }

    private void FixedUpdate() => Fire();

    public virtual void Initialize(InitializationValueObject valueObject)
    {
        _id = valueObject.GetID;

        _field = valueObject.GetField;

        _fireCooldown = valueObject.GetFireCooldown;

        _minDistance = valueObject.GetMinDistance;

        _maxDistance = valueObject.GetMaxDistance;

        _maxProjectileSpeed = valueObject.GetMaxProjectileSpeed;

        _maxHealth = valueObject.GetMaxHealth;

        Health = valueObject.GetMaxHealth;

        speedKoaf = valueObject.GetSpeedKoaf;

        _nameText.text = GetID.ToString();
    }

    public void SetStartPosition(Vector3 startPosition) => StartPosition = startPosition;

    public virtual void UsePoop()
    {
        if (!_hasPoop)
            return;

        _hasPoop = false;

        _nextProjectileIsPoop = true;
    }

    public virtual void Fire()
    {
        if (!CanFire)
            return;

        if (Input.touchCount > 0 && _startTouchPosition == Vector3.zero)
        {
            _startTouchPosition = GetMousePosition();
        }
        else if (Input.touchCount > 0 && _startTouchPosition != Vector3.zero)
        {
            _lastTouchPosition = GetMousePosition();
        }
        else if (Input.touchCount == 0 && _startTouchPosition != Vector3.zero && _lastTouchPosition != Vector3.zero && Vector3.Distance(_startTouchPosition, _lastTouchPosition) > _minDistance && Vector3.Distance(_startTouchPosition, _lastTouchPosition) < _maxDistance && _lastFireTime > _fireCooldown)
        {
            Bullet bullet;

            if (_nextProjectileIsPoop)
            {
                bullet = Instantiate(_poopTemplate, transform.position + new Vector3(0, transform.localScale.y / 2f + _bulletTemplate.transform.localScale.y / 2f, 0), Quaternion.identity);

                _nextProjectileIsPoop = false;

                bullet.SetType(Bullet.BulletType.Poop);
            }
            else
            {
                bullet = Instantiate(_bulletTemplate, transform.position + new Vector3(0, transform.localScale.y / 2f + _bulletTemplate.transform.localScale.y / 2f, 0), Quaternion.identity);
            }

            float speed = Vector3.Distance(_startTouchPosition, _lastTouchPosition) * speedKoaf;

            _startTouchPosition = _startTouchPosition - _lastTouchPosition;
            _lastTouchPosition = Vector3.zero;

            Vector3 finalDirection = -_startTouchPosition;

            float lengthOffinalDirection = Mathf.Sqrt(finalDirection.x * finalDirection.x + finalDirection.z * finalDirection.z);

            float cos = _startTouchPosition.x / lengthOffinalDirection;

            float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;

            if (_startTouchPosition.z < _lastTouchPosition.z)
                angle = 360 - angle;


            bullet.Initialize(SettingsModel.MaxHeight, speed, angle, this);

            _startTouchPosition = Vector2.zero;

            _lastFireTime = 0;
        }
        else
        {
            _startTouchPosition = Vector2.zero;
            _lastTouchPosition = Vector2.zero;
        }

        _lastFireTime += Time.fixedDeltaTime;
    }

    protected Vector3 GetMousePosition()
    {
        Vector3 screenPosition = Input.mousePosition;

        Vector3 worldPosition = new Vector3(screenPosition.x, screenPosition.y, 10f);

        return Camera.main.ScreenToWorldPoint(worldPosition);
    }
}
