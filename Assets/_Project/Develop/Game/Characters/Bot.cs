using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class Bot : Character
{
    private const float MIN_DISTANCE_KOAF = 0.3f;
    private const float MAX_DISTANCE_KOAF = 0.48f;

    [SerializeField] private float _botSpeedKoaf;

    protected override bool hasPoop
    {
        get => _hasPoop;

        set
        {
            _hasPoop = value;

            if (value == true)
            {
                _field.ActivatePoopButtonObject();
            }
        }
    }

    public override bool CanFire
    {
        get => _canFire;
        set
        {
            _canFire = value;

            if (value == true)
            {
                if (_gotDamage && Random.Range(0, 2) == 0 && _targetCharacter == null)
                {
                    _targetCharacter = _damager;
                }
                
                if (_targetCharacter != null)
                    return;
                
                _targetCharacter = GameController.Instance.GetCharacterWithMinHealth(this);

                if (_targetCharacter.Health == 100)
                {
                    while (_targetCharacter.GetID == this.GetID)
                    {
                        _targetCharacter = GameController.Instance.GetRandomCharacterTransform();
                    }
                }
            }
        }
    }

    private Character _targetCharacter;

    public override void Fire()
    {
        if (!CanFire)
            return;

        if (_targetCharacter == null)
            return;

        if (hasPoop && _targetCharacter.Health <= 50)
            UsePoop();

        _startTouchPosition = _targetCharacter.transform.position;
        _lastTouchPosition = transform.position;

        if (Input.touchCount == 0 && _startTouchPosition != Vector3.zero && _lastTouchPosition != Vector3.zero && Vector3.Distance(_startTouchPosition, _lastTouchPosition) > _minDistance && Vector3.Distance(_startTouchPosition, _lastTouchPosition) < _maxDistance && _lastFireTime > _fireCooldown + 1.5f)
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

            float speed = Vector3.Distance(_startTouchPosition, _lastTouchPosition) * speedKoaf * _botSpeedKoaf * Random.Range(MIN_DISTANCE_KOAF, MAX_DISTANCE_KOAF);

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
}
