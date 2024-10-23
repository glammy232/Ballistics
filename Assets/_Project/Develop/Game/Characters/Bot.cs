using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class Bot : Character
{
    private const float MIN_DISTANCE_KOAF = 0.33f;
    private const float MAX_DISTANCE_KOAF = 0.41f;

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
                if (_targetCharacter != null)
                    return;

                int rand = Random.Range(0, 2);

                Debug.Log(_gotDamage + " " + GetID);
                if (_gotDamage && GameController.Instance.GetCharacterWithMinHealth(this).GetHealth < 100)
                {
                    if(rand == 0)
                    {
                        _targetCharacter = _damager;
                        Debug.Log("Damager IS: " + _targetCharacter.GetID);
                    }
                    else
                    {
                        _targetCharacter = GameController.Instance.GetCharacterWithMinHealth(this);
                        Debug.Log("MinHealth IS: " + _targetCharacter.GetID);
                    }
                }
                else if (_gotDamage && GameController.Instance.GetCharacterWithMinHealth(this).GetHealth == 100)
                {
                    _targetCharacter = _damager;
                    Debug.Log("Damager1 IS: " + _targetCharacter.GetID);
                }
                else
                {
                    if(_targetCharacter == null)
                    {
                        _targetCharacter = GameController.Instance.GetRandomCharacter(this);
                        Debug.Log("Rand IS: " + _targetCharacter.GetID);
                    }
                }
            }
        }
    }

    private Character _targetCharacter;

    public void SetTargetCharacterToNull() => _targetCharacter = null; 

    public override void Fire()
    {
        if (!CanFire)
            return;

        if (_targetCharacter == null)
        {
            return;
        }

        if (hasPoop && _targetCharacter.GetHealth <= 50)
            UsePoop();

        _startTouchPosition = _targetCharacter.transform.position;
        _lastTouchPosition = transform.position;

        if (Vector3.Distance(_startTouchPosition, _lastTouchPosition) > _minDistance && Vector3.Distance(_startTouchPosition, _lastTouchPosition) < _maxDistance && _lastFireTime > _fireCooldown + 1.5f)
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


            bullet.Initialize(SettingsModel.MaxHeight, speed, angle, this, _targetCharacter.transform.position);

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
