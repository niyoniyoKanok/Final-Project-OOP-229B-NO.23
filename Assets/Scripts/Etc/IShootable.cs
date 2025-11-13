using UnityEngine;

public interface IShootable
{
    public GameObject Bullet { get; set; }
    public Transform ShootPoint { get; set; }

    public void Shoot();
}
