using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyBulletSpawner : MonoBehaviour
{
    public int num_col;
    public float speed, lifetime, size;
    public float baseEmitTimer;
    public Sprite sprite;
    public Color color;
    public Material material;
    public LayerMask collisionLayer;

    [SerializeField]
    AnimationCurve velocityOverLifetimeCurve;
    
    private float _emitTimer;

    public List<ParticleSystem> _particleSystems;
    private void Start()
    {
        InitializeParticleSystems();
    }

    private void InitializeParticleSystems()
    {
        _particleSystems = new List<ParticleSystem>();
        for (float angle = 0; angle<360; angle+= 360 / num_col)
        {
            // default particle, maybe serialize it
            Material particleMat = material;

            GameObject go = new GameObject("ParticleSystem");
            go.tag = "Enemy";
            go.transform.Rotate(angle, 90, 0); //default face upwards
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            ParticleSystem system = go.AddComponent<ParticleSystem>();
            ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
            go.GetComponent<ParticleSystemRenderer>().material = particleMat;
            
            ParticleSystem.MainModule mainModule = system.main;
            mainModule.startColor = color;
            mainModule.startSize = size;
            mainModule.startSpeed = speed;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.TextureSheetAnimationModule tsam = system.textureSheetAnimation;
            ParticleSystem.EmissionModule emission = system.emission;
            ParticleSystem.ShapeModule shape = system.shape;
            ParticleSystem.CollisionModule collision = system.collision;
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = system.velocityOverLifetime;
                
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision2D;
            collision.lifetimeLoss = 1;
            collision.collidesWith = collisionLayer;
            collision.sendCollisionMessages = true;

            velocityOverLifetime.speedModifier = new ParticleSystem.MinMaxCurve(1, velocityOverLifetimeCurve);
            shape.shapeType = ParticleSystemShapeType.Sprite;
            shape.sprite = null;
            tsam.mode = ParticleSystemAnimationMode.Sprites;
            tsam.AddSprite(sprite);
            renderer.sortingLayerName = "Bullet";


            emission.enabled = false; //disabled we manually emit
            velocityOverLifetime.enabled = true;
            shape.enabled = true;
            tsam.enabled = true;
            collision.enabled = true;
            

            _particleSystems.Add(system);
        }
        
    }

    private void Emit()
    {
        foreach(ParticleSystem ps in _particleSystems)
        {
            ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
            ep.startColor = color;
            ep.startSize = size;
            ep.startLifetime = lifetime;
            ps.Emit(ep, 1);
        }
    }

    private void FixedUpdate()
    {
        _emitTimer -= Time.fixedDeltaTime;
        if(_emitTimer < 0 )
        {
            Emit();
            _emitTimer = baseEmitTimer;
        }
    }
}
