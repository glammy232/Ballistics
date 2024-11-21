public sealed class Player : Character
{
    public override void Initialize(InitializationValueObject valueObject)
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

        _nameText.text = "ÂÛ";
    }
}
