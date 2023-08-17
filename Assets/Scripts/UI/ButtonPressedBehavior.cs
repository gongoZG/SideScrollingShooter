using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载在按钮动画机的 pressed 动画上的 Behavior 脚本，负责实现按钮的功能
/// </summary>
public class ButtonPressedBehavior : StateMachineBehaviour
{
    public static Dictionary<string, System.Action> buttonFunctionTable;

    private void Awake() {
        buttonFunctionTable = new Dictionary<string, System.Action>();
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /// <summary>
    /// 在按钮进入按下状态（播放按下动画的那一帧）时禁用其他的所有按钮，防止重复按键
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UIInput.Instance.DisableAllUIInputs();  // 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// <summary>
    /// 在按钮按下动画播放完后才调用按钮的功能函数，确保动画播放完成
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        buttonFunctionTable[animator.gameObject.name].Invoke();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
