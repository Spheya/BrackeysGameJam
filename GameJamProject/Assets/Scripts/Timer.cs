using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // TODO: Rewind everything in the scene

        Destroy(animator.gameObject.transform.parent.gameObject);
    }
}
