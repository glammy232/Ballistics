using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private BulletType _type;

    [SerializeField] private float _groundYPosition;

    private Character _parentCharacter;

    private const float MIN_DISTANCE = 10f;

    [SerializeField] private GameObject _explosionTemplate;
    public void Initialize(float maxHeight, float startSpeed, float angle, Character parent, Vector3 targetPos)
    {
        maxHeight = SettingsModel.MaxHeight;

        _parentCharacter = parent;

        _rb.velocity = Vector3.zero;

        if (Vector3.Distance(targetPos, transform.position) < MIN_DISTANCE)
        {
            float l = MIN_DISTANCE;
            float g = Mathf.Abs(Physics.gravity.y);
            float sin = Mathf.Sin(2 * angle * Mathf.Deg2Rad);

            float n = l * 2 * g;
            float m = sin * sin;

            startSpeed = Mathf.Sqrt(n / m) / 5f;

            startSpeed *= Random.Range(3.5f, 4.5f);
        }

        Vector3 startDirection = Ballistics.StartDirection(maxHeight, startSpeed);

        angle = angle * Mathf.Deg2Rad;

        Matrix4x4 rotationMatrix = new Matrix4x4(
            new Vector4(Mathf.Cos(angle), 0, Mathf.Sin(angle), 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-Mathf.Sin(angle), 0, Mathf.Cos(angle), 0),
            new Vector4(0, 0, 0, 1)
        );

        Vector3 finalDirection = rotationMatrix.MultiplyPoint3x4(startDirection);

        finalDirection = finalDirection * startSpeed;

        _rb.AddForce(finalDirection, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collider)
    {
        GameController.Instance.FiredBullets++;

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

            GameController.Instance.Hits++;
        }
        else if (collider.gameObject.TryGetComponent(out Field field))
        {
            if(field.ParentCharacter != null)
            {
                field.ParentCharacter.GetDamage(_parentCharacter, 0);
            }
        }

        GameController.Instance.DestroyBullet(this);
    }
    
    public void SetType(BulletType type) => _type = type;

    public enum BulletType
    {
        Tomato = 0,
        Poop = 1
    }
}
