using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private BulletType _type;

    [SerializeField] private float _groundYPosition;

    public void Initialize(float maxHeight, float startSpeed, float angle, BulletType type)
    {
        maxHeight = SettingsModel.MaxHeight;

        _rb.velocity = Vector3.zero;

        _type = type;

        Vector3 startDirection = Ballistics.StartDirection(maxHeight, startSpeed);

        angle = angle * Mathf.Deg2Rad;

        Matrix4x4 rotationMatrix = new Matrix4x4(
            new Vector4(Mathf.Cos(angle), 0, Mathf.Sin(angle), 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-Mathf.Sin(angle), 0, Mathf.Cos(angle), 0),
            new Vector4(0, 0, 0, 1)
        );

        Vector3 finalDirection = rotationMatrix.MultiplyPoint3x4(startDirection);

        //Debug.Log($"<color=blue>Min speed: {Ballistics.MinStartSpeed(maxHeight)}</color>");

        if (startSpeed < Ballistics.MinStartSpeed(maxHeight))
            finalDirection = finalDirection * Ballistics.MinStartSpeed(maxHeight);
        else 
            finalDirection = finalDirection * startSpeed;

        _rb.AddForce(finalDirection, ForceMode.VelocityChange);
    }

    private void Update()
    {
        if (transform.position.y < _groundYPosition)
            GameController.Instance.DestroyBullet(this);
    }

    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.TryGetComponent(out Player player))
        {
            if (_type == BulletType.Tomato)
                player.Health -= Player.MAX_HEALTH / 10;
            else if(_type == BulletType.Poop)
                player.Health -= Player.MAX_HEALTH / 2;

            player.CanFire = true;

            player.GetComponent<Renderer>().material.color = Color.black;
        }
        if (collider.gameObject.TryGetComponent(out Bullet bullet) == false)
            GameController.Instance.DestroyBullet(this);
    }

    public enum BulletType
    {
        Tomato = 0,
        Poop = 1
    }
}
