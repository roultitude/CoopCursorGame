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
    bool usingParticleSystemOverrides, usingVelocityCurveOverride;
    [SerializeField]
    ParticleSystem psPrefab;

    [SerializeField]
    AnimationCurve velocityOverLifetimeCurve;
    
    private float emitTimer;

    public List<ParticleSystem> particleSystems;
    private void Start()
    {
        InitializeParticleSystems();
    }

    private void InitializeParticleSystems()
    {
        particleSystems = new List<ParticleSystem>();
        for (float angle = 0; angle<360; angle+= 360 / num_col)
        {
            // default particle, maybe serialize it
            Material particleMat = material;

            ParticleSystem system = Instantiate(psPrefab, transform);
            GameObject go = system.gameObject;
            go.tag = "Enemy";
            go.transform.Rotate(angle,0,0);
            go.transform.localPosition = Vector3.zero;
            ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
            go.GetComponent<ParticleSystemRenderer>().material = particleMat;
            if (usingParticleSystemOverrides)
            {
                ParticleSystem.MainModule mainModule = system.main;
                mainModule.startColor = color;
                mainModule.startSize = size;
                mainModule.startSpeed = speed;
                mainModule.startLifetime = lifetime;
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


                //emission.enabled = false; //disabled we manually emit
                velocityOverLifetime.enabled = true;
                shape.enabled = true;
                tsam.enabled = true;
                collision.enabled = true;
            }
            else if (usingVelocityCurveOverride) // we have this separate because the animation curve on PS velocity speed modifier is kinda weirdge we prefer using this
            {
                ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = system.velocityOverLifetime;
                velocityOverLifetime.speedModifier = new ParticleSystem.MinMaxCurve(1, velocityOverLifetimeCurve);
                velocityOverLifetime.enabled = true;
            }

            particleSystems.Add(system);
        }
        
    }

    private void Emit()
    {
        foreach(ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }

    private void FixedUpdate()
    {
        emitTimer -= Time.fixedDeltaTime;
        if(emitTimer < 0 )
        {
            Emit();
            emitTimer = baseEmitTimer;
        }
    }
}
