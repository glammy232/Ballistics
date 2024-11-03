using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private BulletType _type;

    [SerializeField] private float _groundYPosition;

    private Character _parentCharacter;

    private const float MIN_DISTANCE = 10f;

    [SerializeField] private GameObject _explosionTemplate;
    public void Initialize(float startSpeed, float angle, Character parent)
    {
        _parentCharacter = parent;

        _rb.AddForce(Ballistics.CountFinalDirection(startSpeed, angle), ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collider)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("click"));

        Instantiate(_explosionTemplate, transform.position, Quaternion.identity);

        if (collider.gameObject.TryGetComponent(out Character player))
        {
            if (_type == BulletType.Tomato)
                player.GetDamage(_parentCharacter, player.GetMaxHealth / 10);
            else if(_type == BulletType.Poop)
                player.GetDamage(_parentCharacter, player.GetMaxHealth / 2);

            if (player.GetHealth <= 0)
                _parentCharacter.Kills++;
        }
        else if (collider.gameObject.TryGetComponent(out Field field))
        {
            if(field.ParentCharacter != null)
                field.ParentCharacter.GetDamage(_parentCharacter, 0);
        }

        Destroy(gameObject);
    }
    
    public void SetType(BulletType type) => _type = type;

    public enum BulletType
    {
        Tomato = 0,
        Poop = 1
    }
}
