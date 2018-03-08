using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Photon.MonoBehaviour
{
    [SerializeField] Type      _type;
    [SerializeField] float     _size = 0.1f;
    [SerializeField] LayerMask _collidesWith;

    int _damage;
    Vector3 _velocity;
    float _lifetime;

    public void Initialize(int inDamage, float inLifetime, Vector3 inVelocity)
    {
        _damage = inDamage;
        _lifetime = inLifetime;
        _velocity = inVelocity;

        photonView.RPC("NetInitialize", PhotonTargets.Others, inDamage, inLifetime, inVelocity, transform.position);
    }

    [PunRPC]
    void NetInitialize(int inDamage, float inLifetime, Vector3 inVelocity, Vector3 inOrigin, PhotonMessageInfo inInfo)
    {
        // float netDelta = NetworkManager.CalculateNetDelta(inInfo.timestamp) * 3;

        _damage = inDamage;
        _lifetime = inLifetime;         
        _velocity = inVelocity;
        transform.position = inOrigin;

        CheckCollision(inOrigin);
    }

    void Update()
    {
        CheckCollision(transform.position);
        Move();
        ProgressLifetime();
    }


    void ProgressLifetime()
    {
        _lifetime -= Time.deltaTime;

        if (_lifetime <= 0)
            Destroy(gameObject);
    }

    void Move()
    {
        transform.position += _velocity * Time.deltaTime;
    }

    void CheckCollision(Vector3 inOrigin, float inNetDelta = 0)
    {
        RaycastHit[] hits = Physics.SphereCastAll(inOrigin, _size, _velocity.normalized, _velocity.magnitude * (Time.deltaTime + inNetDelta));
        foreach (RaycastHit hit in hits)
            if (_collidesWith.Contains(hit.collider.gameObject.layer))
                OnCollision(hit.collider);
    }

    void OnCollision(Collider hitCollider)
    {
        if (photonView.isMine)
        {
            Player hitPlayer = hitCollider.GetComponent<Player>();
            if (hitPlayer)
                hitPlayer.healthComponent.photonView.RPC("DealDamage", PhotonTargets.All, _damage, _type);
        }

        Destroy(gameObject);
    }

    public enum Type
    {
        None,

        Ballistic,
        Plasma
    }
}


/*  Networked Bullet Alternatives
 *  
 *  - Local hit detection
 *      When a player fires their weapon, an RPC is sent to all other players that the shooter fires their weapon.
 *      If the bullet hits for the player that fired their weapon, send an RPC to the other players of what player was hit, and the position of him and the hitting bullet. 
 *      If the shooters bullet, or the shooters hit player is too far away from where they are located at the server, kick the shooter.
 *      
 *      Pros: Easy. Looks really good for the shooter. 
 *      Cons: People recieving fire will recieve damage despite the bullets not being close to them on their screens
 *  
 *  
 *    
 *  - Dead reckoning, Local hit detection
 *      When a player fires their weapon, an RPC is sent to all other players that the shooter fires their weapon.
 *      When the message arrives, the fired weapon will estimate where the bullet would be located for the player who fired their weapon
 *      If the bullet hits for the player that fired their weapon, send an RPC to the other players of what player was hit, and the position of him and the hitting bullet. 
 *      If the shooters bullet, or the shooters hit player is too far away from where they are located at the server, kick the shooter.
 *      
 *      Pros: Easy. Looks really good for the shooter. Bullets hitting you will appear to be relatively close to you when you get hurt.
 *      Cons: For other players, when you fire - the bullets will appear to be appearing in front of you. This looks weird. 
 *      
 *  
 *  
 *  - Server hit detection
 *      When a player fires their weapon, an RPC is sent to all players that the shooter fires their weapon.
 *      If the bullet hits for the server, send an RPC to all players that the hit player was hit, and destroy the bullet that hit it
 *      
 *      Pros: Cheat safe
 *      Cons: For the player shooting, people will be appearing like they are taking damage after a delay. For the players getting shot, they will appear to be taking damage after a delay. For the shooter, hitting bullets will appear to not be registering every now and again
 *      
 *
 * 
 *  - Dead reckoning on projectile collision, Server hit detection
 *      When a player fires their weapon, an RPC is sent to all players that the shooter fires their weapon.
 *      When the message arrives, and the reciever is the master client, dead reckon the collision of the bullet while simulating the graphics part normally
 *      If the bullet hits for the server, send an RPC to all players that the hit player was hit, and destroy the bullet that hit it
 *      
 *      Pros: Cheat safe, for the shooter hitting bullets will appear registering more often and more quickly
 *      Cons: For the player shooting, people will still be appearing like they are taking damage after a delay (maybe?). For the player getting shot, they will appear to be taking damage after a delay. 
 * 
 */
