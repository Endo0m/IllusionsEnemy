using UnityEngine;

// Интерфейс для объектов, которые могут стрелять лазером
public interface ILaserShooter
{
    void Shoot(Vector3 target);
}