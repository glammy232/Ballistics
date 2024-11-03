using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class Bot : Character
{
    private const float MIN_DISTANCE_KOAF = 0.205f;//0.165
    private const float MAX_DISTANCE_KOAF = 0.225f;//0.205

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

            if (value && GetID == GameController.Instance.GetCurrentCharacterID)
                GetComponent<Renderer>().material.color = Color.green;

            if (value == true)
            {
                if (_targetCharacter != null)
                    return;

                int rand = Random.Range(0, 2);

                if (_gotDamage && GameController.Instance.GetCharacterWithMinHealth(this).GetHealth < 100)
                {
                    if(rand == 0)
                        _targetCharacter = _damager;
                    else
                        _targetCharacter = GameController.Instance.GetCharacterWithMinHealth(this);
                }
                else if (_gotDamage && GameController.Instance.GetCharacterWithMinHealth(this).GetHealth == 100)
                    _targetCharacter = _damager;
                else if (!_gotDamage && GameController.Instance.GetCharacterWithMinHealth(this).GetHealth < 100)
                {
                    _targetCharacter = GameController.Instance.GetCharacterWithMinHealth(this);
                }
                else
                {
                    if(_targetCharacter == null)
                        _targetCharacter = GameController.Instance.GetRandomCharacter(this);
                }

                Debug.Log(_targetCharacter.GetID);
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
            return;

        if (hasPoop && _targetCharacter.GetHealth <= 50)
        {
            UsePoop();
            _field.HidePoopButton();
        }

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

            float speed = Vector3.Distance(_startTouchPosition, _lastTouchPosition) * speedKoaf * Random.Range(MIN_DISTANCE_KOAF, MAX_DISTANCE_KOAF);

            _startTouchPosition = _startTouchPosition - _lastTouchPosition;
            _lastTouchPosition = Vector3.zero;

            Vector3 finalDirection = -_startTouchPosition;

            float lengthOffinalDirection = Mathf.Sqrt(finalDirection.x * finalDirection.x + finalDirection.z * finalDirection.z);

            float cos = _startTouchPosition.x / lengthOffinalDirection;

            float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;

            if (_startTouchPosition.z < _lastTouchPosition.z)
                angle = 360 - angle;

            Vector3 targetPos = _targetCharacter.transform.position;

            if (Vector3.Distance(targetPos, transform.position) < 4f)
            {
                speed *= Random.Range(1.75f, 1.85f);//1.8f;
            }
            else if (Vector3.Distance(targetPos, transform.position) < 7f)
            {
                speed *= Random.Range(1.4f, 1.5f);//1.45f;
            }
            else if (Vector3.Distance(targetPos, transform.position) > 7f && Vector3.Distance(targetPos, transform.position) < 11f)
            {
                speed *= Random.Range(1.25f, 1.3f);//1.35f;
            }
            else if (Vector3.Distance(targetPos, transform.position) > 15f)
            {
                speed *= Random.Range(0.9f, 1f);//0.8f;
            }

            bullet.Initialize(speed, angle, this);

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
