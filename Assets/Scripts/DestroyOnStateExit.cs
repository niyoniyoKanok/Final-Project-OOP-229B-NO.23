using UnityEngine;

    public class DestroyOnStateExit : StateMachineBehaviour
    {

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject);
        }
    }

