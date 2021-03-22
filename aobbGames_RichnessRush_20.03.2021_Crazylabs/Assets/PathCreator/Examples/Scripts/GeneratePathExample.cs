using UnityEngine;

namespace PathCreation.Examples {
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]
    public class GeneratePathExample : MonoBehaviour {

        public bool closedLoop = true;
        public Transform[] waypoints;

        private void Awake()
        {
            waypoints = new Transform[BotSimpleAI.getInstance().aiPathWaypoints.Count];
        }

        void Start () {

            for (int i=0; i < BotSimpleAI.getInstance().aiPathWaypoints.Count ;i++)
            {
                GameObject holder = new GameObject();
                holder.transform.position = BotSimpleAI.getInstance().aiPathWaypoints[i];
                waypoints[i] = holder.transform;
            }

            if (waypoints.Length > 0) {
                // Create a new bezier path from the waypoints.
                BezierPath bezierPath = new BezierPath (waypoints, closedLoop, PathSpace.xyz);
                GetComponent<PathCreator>().bezierPath = bezierPath;
            }
        }
    }
}