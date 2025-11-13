using UnityEngine;


public class PlayerStateLock : StateMachineBehaviour
{
  
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        PlayerController player = animator.GetComponent<PlayerController>();
        if (player != null)
        {
         
            player.SetMovementLock(true);
        }
    }

  
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
        PlayerController player = animator.GetComponent<PlayerController>();
        if (player != null)
        {
           
            player.SetMovementLock(false);
        }
    }
}