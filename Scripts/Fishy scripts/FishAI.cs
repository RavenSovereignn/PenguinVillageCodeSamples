using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour
{
    [Header("References"), HideInInspector]
    public List<FishAI> allFishes;

    [Header("Stats")]
    public float maxSpeed;
    public float maxForce;

    Vector2 fishPos;
    Vector2 velocity = new Vector2();
    Vector2 acceleration = new Vector2();

    [Header("Interaction Distances")]
    public float localDist;
    public float groupingDist;

    [Header("Force Weightings")]
    public float avoidanceStrength = 0.6f;
    public float alignmentStrength = 0.2f;
    public float centeringStrength = 0.2f;

    private float cachedTime;

    [Header("debugging")]
    public Transform target;

    public List<Transform> path;
    public float pathRadius;

    public Vector2 Velocity { get { return velocity; } }


    void Start()
    {
        fishPos = transform.position;
    }

    public void FishUpdate(float deltaTime)
    {
        cachedTime = deltaTime;

        //MoveTowards(target.position);
        //FollowPath();

        Vector2 force = new Vector2();

        force += Avoidance() * avoidanceStrength;
        force += Centering() * centeringStrength;
        force += Alignment() * alignmentStrength;


        //force += MoveTowards(target.position) * 0.1f;

        ApplyForce(force);

        MovementStep();
    }

    private void MovementStep()
    {
        velocity += acceleration;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        fishPos += velocity * cachedTime;

        transform.position = fishPos;

        acceleration *= 0;

        Rotate(velocity);
    }

    private void Rotate(Vector2 moveInput)
    {
        float trig = Mathf.Atan2(-moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        Vector3 newRot = new Vector3(0, 0, trig + 90);
        transform.rotation = Quaternion.Euler(newRot);
    }

    private void ApplyForce(Vector2 force)
    {
        acceleration += force;
    }

    private Vector2 Steer(Vector2 desiredVel)
    {
        Vector2 steer = desiredVel - velocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    //predicts pos after a given time
    private Vector2 PredictPos(float time)
    {
        Vector2 currentDir = velocity.normalized;
        currentDir *= time;
        return fishPos + currentDir;
    }

    private Vector2 Avoidance()
    {
        Vector2 avoidance = new Vector2();
        int neighbourCount = 0;

        foreach(FishAI fish in allFishes) {
            if(fish == this) {
                continue;
            }

            float distance = Vector2.Distance(transform.position,fish.transform.position);

            if (distance < localDist) {
                //find vector pointing away from neighbour and scale appropriately  
                Vector2 diff = transform.position - fish.transform.position;
                diff.Normalize();
                diff /= distance;

                avoidance += diff;
                neighbourCount++;

            }
        }

        if(neighbourCount > 0) {
            avoidance /= neighbourCount;
        }

        avoidance = Steer(avoidance * maxSpeed);

        return avoidance;
    }

    private Vector2 Centering()
    {
        Vector2 centering = new Vector2();
        int neighbourCount = 0;

        foreach (FishAI fish in allFishes) {
            if (fish == this) {
                continue;
            }

            float distance = Vector2.Distance(transform.position, fish.transform.position);

            if (distance < groupingDist) {
                centering += (Vector2)fish.transform.position;
                neighbourCount++;
            }
        }

        if (neighbourCount > 0) {
            centering /= neighbourCount;
            return MoveTowards(centering);
        }
        else {
            return new Vector2();
        }
    }

    private Vector2 Alignment()
    {
        Vector2 align = new Vector2();
        int neighbourCount = 0;

        foreach(FishAI fish in allFishes) {
            if (fish == this) {
                continue;
            }

            float distance = Vector2.Distance(transform.position, fish.transform.position);

            if (distance < localDist) {
                align += fish.Velocity;
                neighbourCount++;
            }
        }

        if(neighbourCount > 0) {
            align /= neighbourCount;
            align = align.normalized * maxSpeed;
            align = Steer(align);

            return align;
        }
        else {
            return new Vector2();
        }
    }

    private Vector2 MoveTowards(Vector2 target, bool slowAtTarget = false)
    {
        Vector2 targetDirection = target - fishPos;

        float d = targetDirection.magnitude;
        targetDirection.Normalize();

        if(d < 5 && slowAtTarget) {
            float m = MapValue(d, 0, 5, 0, maxSpeed);
            targetDirection *= m;
        }
        else {
            targetDirection *= maxSpeed;
        }

        Vector2 steer = Steer(targetDirection);
        return steer;
    }

    //only follows left to right, if wanted to use this need to add other direction
    private void FollowPath()
    {
        Vector2 closestPathNormal = new Vector2();
        Vector2 closestPathDir = new Vector2();
        float closestPathDistance = 10000;

        Vector2 predictedPos = PredictPos(0.5f);

        for (int i = 0; i < path.Count -1; i++) {
            Vector2 p1 = path[i].position;
            Vector2 p2 = path[i + 1].position;

            Vector2 pathNormal = GetPathNormal(predictedPos, p1, p2);

            //out of path bounds
            if(pathNormal.x < p1.x || pathNormal.x > p2.x) {
                pathNormal = p2;
            }

            float pathDist = Vector2.Distance(predictedPos, pathNormal);

            if(pathDist < closestPathDistance) {
                closestPathNormal = pathNormal;
                closestPathDir = p2 - p1;
                closestPathDistance = pathDist;
            }
        }

        closestPathDir = closestPathDir.normalized * 2;

        Vector2 target = closestPathNormal + closestPathDir;

        float dist = Vector2.Distance(closestPathNormal, predictedPos);

        if(dist > pathRadius) {
            MoveTowards(target,false);
        }

    }
    
    private float MapValue(float value, float fromBot,float fromTop, float toBot, float toTop)
    {
        return toBot + (value - fromBot) * (fromTop - toBot) / (fromTop - fromBot);
    }

    private Vector2 GetPathNormal(Vector2 predictedPoint, Vector2 pathStart, Vector2 pathEnd)
    {
        Vector2 pointFromPath = predictedPoint - pathStart;
        Vector2 pathDir = (pathEnd - pathStart).normalized;

        Vector2 normalPoint = pathDir * Vector2.Dot(pointFromPath, pathDir);

        return pathStart + normalPoint;
    }

}
