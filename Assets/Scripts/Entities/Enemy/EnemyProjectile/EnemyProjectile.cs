using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage;
    private float moveSpeed;
    private Vector3 moveDirection;
    private float lifeTime = 5f;

    //===========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHeart>().UpdateCurrentHeart(-damage);
            Despawn();
        }
        if (collision.gameObject.CompareTag("Collisions"))
        {
            Despawn();
        }
        
    }
    //===========================================================================
    private void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * moveDirection;

        // Partical LifeTime
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0.1f)
        {

        }
        if (lifeTime <= 0)
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        lifeTime = 5;
        gameObject.SetActive(false);
        gameObject.transform.localPosition = Vector2.zero;
    }

    //===========================================================================
    public void SetDamage(int newAmount)
    {
        damage = newAmount;
    }
    public void SetMoveDirectionAndSpeed(Vector3 newMoveDirection, float newSpeed)
    {
        moveDirection = newMoveDirection;
        moveSpeed = newSpeed;
    }
}
