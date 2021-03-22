using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float distanceTravelled;

        public ThirdPersonCharController botController;

        private Vector3 moveJoystickOld, moveJoystickNew;

        private void Awake()
        {
            moveJoystickOld = Vector3.zero;
            moveJoystickNew = Vector3.zero;
        }

        void Start() {

            speed = botController.speed;

            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                moveJoystickOld = transform.position;
                distanceTravelled += speed * Time.deltaTime;
                moveJoystickNew = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                //transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                //transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                //GetComponent<CharacterController>().Move(pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction));
                //Debug.Log("HEY");
                float mag = Mathf.Clamp((moveJoystickNew - moveJoystickOld).magnitude, 0f, 1f);
                Vector3 dir = (moveJoystickNew - moveJoystickOld).normalized;
                botController.move = mag * dir;

            }
        }

        private void LateUpdate()
        {
            moveJoystickNew = transform.position;
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}