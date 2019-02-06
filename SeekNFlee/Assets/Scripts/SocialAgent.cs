using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialAgent : MonoBehaviour {

    private Agent agent;
    public Vector3 targetPosition;

    // Use this for initialization
    void Awake() {
        targetPosition = transform.position;
        agent = GetComponent<Agent>();
    }

    // Update is called once per frame
    void Update() {
        SocialLogic();

        //foreach(Vector3 key in WorldState.GetInstance().socialCircles.Keys) {
        //    Debug.Log("Key:" + key + " = "
        //        + WorldState.GetInstance().socialCircles[key]);
        //}
    }

    float leaveCircleTimer;
    [SerializeField]
    private float raduisDetectSocial;
    void SocialLogic() {
        // wander

        // find social agent
        if (agent.agentType == Agent.AgentType.SOCIAL) {
            if (!isInCircle) {
                //Debug.Log("Checking social agents");
                foreach (Agent a in WorldState.GetInstance().agents) {
                    if (a.agentType == Agent.AgentType.SOCIAL && a != agent) {
                        if (Vector3.Distance(a.transform.position, transform.position) < raduisDetectSocial) {
                            //Debug.Log("social agents found");
                            if (!a.GetComponent<SocialAgent>().isInCircle) {
                                CreateSocialCircle(a.GetComponent<SocialAgent>());
                            }
                            //else {
                            //    EnterSocialCircle(a.GetComponent<SocialAgent>());
                            //}
                            leaveCircleTimer = Time.time;
                        }
                    }
                }
                foreach(Vector3 positions in WorldState.GetInstance().socialCircles.Keys) {
                    if(Vector3.Distance(transform.position, positions) < raduisDetectSocial) {
                        EnterSocialCircle(positions);
                    }
                }
                //Debug.Log("social agents not found");
                if (!isInCircle) {
                    // If social agent is not in circle, wander
                    //Debug.Log("Calling wandering agent");
                    GetComponent<WanderingAgent>().WanderLogic();
                }
            }
            else if (isInCircle) {
                if (Time.time - leaveCircleTimer > 0.5) {
                    if (Random.value < 0.08f) {
                        LeaveSocialCircle();
                        isSocial = false;
                        isInCircle = false;
                    }
                    else {
                        leaveCircleTimer = Time.time;
                        InSocialGroup();
                    }

                }
            }
        }
        //if (centerOfSocial != null && centerOfSocial != Vector3.zero)
        //    Debug.Log("Center of social:" + centerOfSocial);
        
    }

    public bool isInCircle;
    public bool isSocial;
    Vector3 centerOfSocial;

    void GetSocialDistances() {
        float smallestDistance = 100; int index = -1;
        List<Agent> a = WorldState.GetInstance().agents;
        for (int i = 0; i < a.Count; i++) {
            //foreach(Agent a in WorldState.GetInstance().agents) {
            if (a[i].agentType == Agent.AgentType.TRAVELLING) {
                if (smallestDistance > Vector3.Distance(a[i].transform.position, transform.position)) {
                    index = i;
                    smallestDistance = Vector3.Distance(a[i].transform.position, transform.position);
                }
            }
        }
    }

    void InSocialGroup() {
        //if(Vector3.Distance(transform.position, centerOfSocial) > raduisDetectSocial) {
        //    agent.ApplyDesiredVelocity(centerOfSocial- transform.position);
        //}
        //else {
            agent.StandStill();
        //}
        
    }

    #region SocialCirleLogic

    public void EnterSocialCircle(SocialAgent sAgent) {
        if (!isInCircle && isSocial) {
            Debug.Log("Entering social circle");
            WorldState.GetInstance().socialCircles[sAgent.GetSocialCircle()].Add(this);
            centerOfSocial = sAgent.GetSocialCircle();
            isInCircle = true;
            isSocial = true;
        }

    }
    public void EnterSocialCircle(Vector3 middle) {
        if (!isInCircle && isSocial) {
            WorldState.GetInstance().socialCircles[middle].Add(this);
            centerOfSocial = GetSocialCircle();
            isInCircle = true;
            isSocial = true;
        }

    }

    public void CreateSocialCircle(Agent otherAgent) {
        CreateSocialCircle(otherAgent.GetComponent<Agent>());
    }

    public void CreateSocialCircle(SocialAgent otherAgent) {
        if (!isInCircle && isSocial) {

            //Debug.Log("social circle");
            if (WorldState.GetInstance().socialCircles.ContainsKey((otherAgent.transform.position + transform.position) / 2)) {

                //Debug.Log("Joining social circle");
                WorldState.GetInstance().socialCircles[(otherAgent.transform.position + transform.position) / 2].Add(this);
                //isInCircle = true;
                //isSocial = true;
            }
            else {
                //Debug.Log("Creating social circle");
                centerOfSocial = (otherAgent.transform.position + transform.position) / 2;
                WorldState.GetInstance().socialCircles.Add( centerOfSocial, new List<SocialAgent>() { this });
                otherAgent.EnterSocialCircle(centerOfSocial);

            }
            isInCircle = true;
            isSocial = true;

            //otherAgent.isInCircle = true;
            //otherAgent.isSocial = true;

        }
    }

    public void LeaveSocialCircle() {
        isInCircle = false;
        isSocial = false;
        centerOfSocial = Vector3.zero;
        WorldState ws = WorldState.GetInstance();
        foreach (Vector3 pos in WorldState.GetInstance().socialCircles.Keys) {
            if (ws.socialCircles[pos].Contains(this)) {
                ws.socialCircles[pos].Remove(this);
                if (ws.socialCircles[pos].Count == 1) {
                    ws.socialCircles[pos][0].LeaveSocialCircle();
                }
            }
        }
        GetComponent<Agent>().TempWanderer();
    }

    public Vector3 GetSocialCircle() {
        if (isInCircle && isSocial) {
            return centerOfSocial;
        }
        return Vector3.zero;
    }


    #endregion


    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.black;
    //    Gizmos.DrawWireSphere(transform.position, raduisDetectSocial);
    //}
}
