using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private BulletType _type;

    [SerializeField] private float _groundYPosition;

    private Character _parentCharacter;

    [SerializeField] private GameObject _explosionTemplate;
    public void Initialize(float maxHeight, float startSpeed, float angle, Character parent)
    {
        maxHeight = SettingsModel.MaxHeight;

        _parentCharacter = parent;

        _rb.velocity = Vector3.zero;

        Vector3 startDirection = Ballistics.StartDirection(maxHeight, startSpeed);

        float startSpeedOffset = Random.Range(2f, 3.25f);

        if (startSpeed <= Ballistics.MinStartSpeed(maxHeight))
        {
            startSpeed += startSpeedOffset;
            startDirection = Ballistics.StartDirection(maxHeight, startSpeed);
        }

        angle = angle * Mathf.Deg2Rad;

        Matrix4x4 rotationMatrix = new Matrix4x4(
            new Vector4(Mathf.Cos(angle), 0, Mathf.Sin(angle), 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-Mathf.Sin(angle), 0, Mathf.Cos(angle), 0),
            new Vector4(0, 0, 0, 1)
        );

        Vector3 finalDirection = rotationMatrix.MultiplyPoint3x4(startDirection);

        //Debug.Log($"<color=blue>Min speed: {Ballistics.MinStartSpeed(maxHeight)}</color>");

        if (startSpeed <= Ballistics.MinStartSpeed(maxHeight))
            finalDirection = finalDirection * (Ballistics.MinStartSpeed(maxHeight));
        else 
            finalDirection = finalDirection * startSpeed;

        _rb.AddForce(finalDirection, ForceMode.VelocityChange);
    }

    private void Update()
    {
        if (transform.position.y <= _groundYPosition || transform.position.z >= 16f || transform.position.z <= -24f || transform.position.x >= 8.5f || transform.position.x <= -8.5f)
        {
            Instantiate(_explosionTemplate, transform.position, Quaternion.identity);
            GameController.Instance.DestroyBullet(this);
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("click"));

        Instantiate(_explosionTemplate, transform.position, Quaternion.identity);

        if (collider.gameObject.TryGetComponent(out Character player))
        {
            if (_type == BulletType.Tomato)
                player.Health -= player.GetMaxHealth / 10;
            else if(_type == BulletType.Poop)
                player.Health -= player.GetMaxHealth / 2;

            player.SetDamager(_parentCharacter);

            if (player.Health <= 0)
                _parentCharacter.Kills++;
        }
        else if (collider.gameObject.TryGetComponent(out Field field))
        {
            if(field.ParentCharacter != null)
                field.ParentCharacter.CanFire = true;
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
