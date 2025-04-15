using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SetBoolBehaviour : StateMachineBehaviour
{
    public string boolName;
    public bool updateOnState;
    public bool updateOnStateMachine;
    public bool valueOnEnter, valueOnExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(updateOnState){
            animator.SetBool(boolName, valueOnEnter);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(updateOnState){
            animator.SetBool(boolName, valueOnExit);
        }
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if(updateOnStateMachine){
            animator.SetBool(boolName, valueOnEnter);
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {   
        if(updateOnStateMachine){
            animator.SetBool(boolName, valueOnExit);
        }
    }

}
