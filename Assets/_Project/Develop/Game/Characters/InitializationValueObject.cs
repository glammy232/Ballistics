public class InitializationValueObject
{
    private readonly Field _field;
    public Field GetField => _field;

    private readonly int _id;
    public int GetID => _id;

    private readonly float _fireCooldown;
    public float GetFireCooldown => _fireCooldown;

    private readonly float _minDistance;
    public float GetMinDistance => _minDistance;

    private readonly float _maxDistance;
    public float GetMaxDistance => _maxDistance;

    private readonly float _maxProjectileSpeed;
    public float GetMaxProjectileSpeed => _maxProjectileSpeed;

    private readonly int _maxHealth;
    public int GetMaxHealth => _maxHealth;

    private readonly float _speedKoaf;
    public float GetSpeedKoaf => _speedKoaf;

    public InitializationValueObject(Field field, int id, float fireCooldown, float minDistance, float maxDistance, float maxProjectileSpeed, int maxHealth, float speedKoaf)
    {
        _field = field;

        _id = id;

        _fireCooldown = fireCooldown;

        _minDistance = minDistance;

        _maxDistance = maxDistance;

        _maxProjectileSpeed = maxProjectileSpeed;

        _maxHealth = maxHealth;

        _speedKoaf = speedKoaf;
    }
}
