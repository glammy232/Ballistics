using UnityEngine;
using UnityEngine.VFX;

public sealed class Bot : Character
{
    private const float MIN_DISTANCE_KOAF = 0.205f;//0.165
    private const float MAX_DISTANCE_KOAF = 0.225f;//0.205

    private BotConfig _config;

    public BotConfig GetConfig => _config;

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

    protected override bool hasHeart
    {
        get => _hasHeart;
        set
        {
            _hasHeart = value;

            if (value == true)
            {
                _field.ActivateHeartButtonObject();
            }
        }
    }

    public override bool CanFire
    {
        get => _canFire;
        set
        {
            _canFire = value;
        }
    }

    private Character _targetCharacter;

    public Character GetTargetCharacter => _targetCharacter;

    public void Initialize(BotConfig config)
    {
        _config = config;
    }

    public void SetTargetCharacter(Character value) => _targetCharacter = value;

    public override void SetHealth(int value, Character killer)
    {
        _health = value;

        _field.UpdateHealthView(this);

        if (value <= 0)
        {
            GetComponentInChildren<Renderer>().material.color = Color.black;

            GameController.Instance.GetCharactersController.KillCharacter(this, killer);
        }
        if(GameController.Instance != null)
        {
            if (GameController.Instance.GameConfig.Complexity < 2)
            {
                int rand = Random.Range(0, 2);

                if (rand == 0)
                    UseHeart();
            }
            else if (value <= 10 && GameController.Instance.GameConfig.Complexity == 2)
            {
                UseHeart();
            }
            else if (value <= 10 && GameController.Instance.GameConfig.Complexity == 3)
            {
                UseHeart();
            }
        }
    }

    public override void Fire()
    {
        if (!CanFire)
            return;

        if (_targetCharacter == null)
            return;

        if (hasPoop && _targetCharacter.GetHealth <= 50 && GameController.Instance.GameConfig.Complexity > 0)
        {
            UsePoop();
            _field.HidePoopButton();
        }
        else
        {
            int rand = Random.Range(0, 2);

            if(rand == 0)
            {
                UseHeart();
                _field.HideHeartButton();
            }
        }

        _startTouchPosition = _targetCharacter.transform.position;
        _lastTouchPosition = transform.position;

        int level = GameController.Instance.GameConfig.Complexity switch 
        {
            0 => UserData.EasyLevel,
            1 => UserData.NormalLevel,
            2 => UserData.HardLevel,
            _ => UserData.HardcoreLevel,
        };

        if (Vector3.Distance(_startTouchPosition, _lastTouchPosition) > _minDistance && Vector3.Distance(_startTouchPosition, _lastTouchPosition) < _maxDistance && _lastFireTime > _fireCooldown + 1.5f - 0.08f * level)
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
