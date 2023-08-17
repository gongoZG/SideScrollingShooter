using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Player Input")]
/// <summary>
/// 玩家输入类，用于处理玩家输入，通过继承 InputActions 中的接口来实现输入的处理
/// </summary>
public class PlayerInput : 
    ScriptableObject,  // 用于创建可序列化的对象
    InputActions.IGamePlayActions,  // 用于创建回调函数
    InputActions.IPauseMenuActions, 
    InputActions.IGameOverScreenActions
{

    #region Actions
        // 玩家所用的输入相应均由委托实现，通过委托的调用来实现对应的功能
        InputActions inputActions;
        public event UnityAction<Vector2> onMove = delegate {};  // 初始值设为空委托，不会出现空值报错
        public event UnityAction onStopMove = delegate {};
        public event UnityAction onFire = delegate {};
        public event UnityAction onStopFire = delegate {};
        public event UnityAction onDodge = delegate {};
        public event UnityAction onOverDrive = delegate {};
        public event UnityAction onPause = delegate {};
        public event UnityAction onUnpause = delegate {};
        public event UnityAction onLaunchMissle = delegate {};
        public event UnityAction onGameOver = delegate {};

    #endregion
    
    #region GameRunning
        
        private void OnEnable() {
            inputActions = new InputActions();
            inputActions.GamePlay.AddCallbacks(this);  // 添加回调函数
            inputActions.PauseMenu.AddCallbacks(this);  // 添加回调函数
            inputActions.GameOverScreen.AddCallbacks(this);
        }

        private void OnDisable() {
            DisableAllInputs();
        }

    #endregion

    #region ActionMap
        
        private void SwitchActionMap(InputActionMap actionMap, bool isUIInput) {
            inputActions.Disable();  // 禁用当前动作表
            actionMap.Enable();  // 启用指定动作表

            if (isUIInput) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // 在暂停时要将输入的更新方式切换为动态输入，否则会造成接受不到输入的问题
        public void Switch2DynamicUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        public void Switch2FixedUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

        public void DisableAllInputs() => inputActions.Disable();
        public void EnableGameplayInput() => SwitchActionMap(inputActions.GamePlay, false); 
        public void EnablePauseMenuInput() => SwitchActionMap(inputActions.PauseMenu, true);
        public void EnableGameOverInput() => SwitchActionMap(inputActions.GameOverScreen, true);
        

    #endregion

    #region Delegate function
        
        public void OnMove(InputAction.CallbackContext context) {
            // Performed 相当于 getKeyCode 
            // Started   相当于 getKeyCodeDown
            // Canceled  相当于 getKeyCodeUp
            // 或 context.performed
            if (context.phase == InputActionPhase.Performed) {
                onMove.Invoke(context.ReadValue<Vector2>());
            }
            if (context.phase == InputActionPhase.Canceled) {
                onStopMove.Invoke();
            }
        }

        public void OnFire(InputAction.CallbackContext context) {
            if (context.phase == InputActionPhase.Performed) {
                onFire.Invoke();
            }
            if (context.phase == InputActionPhase.Canceled) {
                onStopFire.Invoke();
            }
        }

        public void OnDodge(InputAction.CallbackContext context) {
            if (context.performed) {
                onDodge.Invoke();
            }
        }

        public void OnOverDrive(InputAction.CallbackContext context) {
            if (context.performed) {
                onOverDrive.Invoke();
            }
        }

        public void OnPause(InputAction.CallbackContext context) {
            if (context.performed) {
                onPause.Invoke();
            }
        }

        public void OnUnpause(InputAction.CallbackContext context) {
            if (context.performed) {
                onUnpause.Invoke();
            }
        }

        public void OnLaunchMissle(InputAction.CallbackContext context) {
            if (context.performed) {
                onLaunchMissle.Invoke();
            }
        }

        public void OnGameOver(InputAction.CallbackContext context) {
            if (context.performed) {
                onGameOver.Invoke();
            }
        }

    #endregion
    
}
