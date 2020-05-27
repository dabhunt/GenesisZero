using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VacuumParticles : MonoBehaviour {
    public float suctionVel = 5;
    public float shapeZ = 10;
    public float shapeRad = 1;
    [Tooltip("The power to which uD is raised, causing the Sin variances to have " +
             "less effect in the middle of the cone the higher it is.")]
    public float uDEccentricity = 2;
    public Vector2 sinFrequency = new Vector2(1,1);
    public Vector2 sinMultiplier = Vector2.one;

    ParticleSystem partSys;
    ParticleSystem.MainModule partSysMain;
    ParticleSystem.Particle[] particles;
    ParticleSystem.ShapeModule shape;

	// Use this for initialization
	void Start () {
        // Get the ParticleSystem
        partSys = GetComponent<ParticleSystem>();
        partSysMain = partSys.main;
        partSysMain.simulationSpace = ParticleSystemSimulationSpace.Local;
        shape = partSys.shape;

        particles = new ParticleSystem.Particle[partSysMain.maxParticles];


	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (shapeZ != shape.position.z || shapeRad != shape.radius) {
            shape.position = Vector3.forward * shapeZ;
            shape.radius = shapeRad;
        }

        partSysMain.startLifetime = shapeZ/suctionVel;


        int numParts = partSys.GetParticles(particles);

        Vector3 pDelta, vel;
        float uD;
        for (int i=0; i<numParts; i++) {
            // Destroy particles that pass the origin
            if (particles[i].position.z <= 0) {
                particles[i].remainingLifetime = 0;
                continue;
            }

            pDelta = particles[i].position; // Could be - origin, but origin should be [0,0,0]
            vel = -pDelta.normalized;
            vel *= -suctionVel / vel.z; // Make the z component equal to suctionVel
            // Add variance based on Sin waves
            uD = particles[i].position.z / shapeZ; // uD as a percent of initial distance
            uD = Mathf.Pow(uD, uDEccentricity);
            vel.x += sinMultiplier.x * uD * Mathf.Sin(Time.time * sinFrequency.x + pDelta.z) * suctionVel;
            vel.y += sinMultiplier.y * uD * Mathf.Sin(Time.time * sinFrequency.y + pDelta.z) * suctionVel;

            particles[i].velocity = vel;
        }

        partSys.SetParticles(particles, numParts);
	}
}
