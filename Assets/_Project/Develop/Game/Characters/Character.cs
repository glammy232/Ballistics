using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected TMP_Text _nameText;

    private int _kills;
    public int Kills
    {
        get => _kills;
        set
        {
            _kills = value;

            if(TryGetComponent(out Player player))
            {
                GameController.Instance.MainPlayerKills = _kills;
            }
        }
    }

    private int _hits;

    private int _shotsFired;
    private int shotsFired
    {
        get => _shotsFired;
        set
        {
            _shotsFired = value;

            GameController.Instance.MainPlayerPersantageOfHits = _shotsFired;
        }
    }

    public int PersentageOfHits => (int)((float)_hits / (float)shotsFired * 100);

    protected Field _field;

    public Field GetField => _field;

    protected int _id;
    public int GetID => _id;

    protected int _maxHealth;
    public int GetMaxHealth => _maxHealth;

    protected int _health;
    public int GetHealth => _health;

    public virtual void SetHealth(int value, Character killer)
    {
        _health = value;

        _field.UpdateHealthView(this);

        if (value <= 0)
        {
            GetComponentInChildren<Renderer>().material.color = Color.black;

            GameController.Instance.GetCharactersController.KillCharacter(this, killer);
        }
    }

    public void GetDamage(Character damager, int damage)
    {
        _gotDamage = true;

        _damager = damager;

        SetHealth(_health - damage, damager);

        CanFire = true;
    }

    protected bool _gotDamage;
    protected Character _damager;

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

            if (value == true)
            {
                _field.ActivatePoopButtonObject();
                _field.SetPoopButtonListener(delegate
                {
                    UsePoop();
                });
            }
        }
    }
    public bool GetPoop => hasPoop;

    protected bool _hasHeart;
    protected virtual bool hasHeart
    {
        get => _hasHeart;
        set
        {
            _hasHeart = value;

            if(value == true)
            {
                _field.ActivateHeartButtonObject();
                _field.SetHeartButtonListener(delegate
                {
                    UseHeart();
                });
            }
        }
    }

    public void SetHasPoop(bool value) => hasPoop = value;

    public void SetHasHeart(bool value) => hasHeart = value;

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

    protected float _maxProjectileSpeed;

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

        SetHealth(valueObject.GetMaxHealth, null);

        speedKoaf = valueObject.GetSpeedKoaf;

        _nameText.text = GetID.ToString();
    }

    public void SetStartPosition(Vector3 startPosition) => StartPosition = startPosition;

    public virtual void UsePoop()
    {
        if (!_hasPoop || CanFire == false)
            return;

        _hasPoop = false;

        _nextProjectileIsPoop = true;
    }

    public virtual void UseHeart()
    {
        if (!hasHeart)
            return;

        hasHeart = false;

        SetHealth(100, null);
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
                bullet = Instantiate(_poopTemplate, transform.position + new Vector3(0, transform.localScale.y + _bulletTemplate.transform.localScale.y / 1.5f, 0), Quaternion.identity);

                _nextProjectileIsPoop = false;

                bullet.SetType(Bullet.BulletType.Poop);
            }
            else
            {
                bullet = Instantiate(_bulletTemplate, transform.position + new Vector3(0, transform.localScale.y + _bulletTemplate.transform.localScale.y / 1.5f, 0), Quaternion.identity);
            }

            float speed = Vector3.Distance(_startTouchPosition, _lastTouchPosition) * speedKoaf;

            _startTouchPosition = _startTouchPosition - _lastTouchPosition;

            Vector3 target = _lastTouchPosition;

            _lastTouchPosition = Vector3.zero;

            Vector3 finalDirection = -_startTouchPosition;

            float lengthOffinalDirection = Mathf.Sqrt(finalDirection.x * finalDirection.x + finalDirection.z * finalDirection.z);

            float cos = _startTouchPosition.x / lengthOffinalDirection;

            float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;

            if (_startTouchPosition.z < _lastTouchPosition.z)
                angle = 360 - angle;


            bullet.Initialize(speed, angle, this);

            _startTouchPosition = Vector2.zero;

            _lastFireTime = 0;

            shotsFired++;
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
