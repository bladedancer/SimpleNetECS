using System;
using Unity.Entities;
using UnityEngine;

public class CollisionEmitter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        emit(other.gameObject);   
    }

    private void OnCollisionEnter(Collision collision)
    {
        emit(collision.gameObject);
    }

    private void emit(GameObject other)
    {
        GameObjectEntity otherGameObjectEntity = other.GetComponent<GameObjectEntity>();
        if (!otherGameObjectEntity)
        {
            return;
        }

        Entity sourceEntity = GetComponent<GameObjectEntity>().Entity;
        EntityManager entityManager = World.Active.GetExistingManager<EntityManager>();
        Entity collisionEventEntity = entityManager.CreateEntity(typeof(CollisionComponent));
        entityManager.SetComponentData(collisionEventEntity, new CollisionComponent
        {
            source = sourceEntity,
            target = otherGameObjectEntity.Entity,
        });
    }
}