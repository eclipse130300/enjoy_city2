
//using Bolt.AdvancedTutorial;
using UnityEngine;

public class TutorialPlayerController {// : Bolt.EntityBehaviour<ITutorialPlayerState>

    #region Variable

    const float MOUSE_SENSITIVITY = 2f;

    bool _forward;
    bool _backward;
    bool _left;
    bool _right;
    bool _jump;

    // Fire
    bool _fire;
    bool _aiming;

    float _yaw;
    float _pitch;

    //PlayerMotor _motor;

    [SerializeField]
    //TutorialWeapon[] weapons;

    #endregion

    #region Standart Functions

    private void Awake () {
        //_motor = GetComponent<PlayerMotor>();
    }

    private void Update () {
        PoolKeys(true);
    }

    #endregion

    #region Bolt Functions

    public  void Attached () { //override
        //state.SetTransforms(state.Transform, transform);
        //state.SetAnimator(GetComponentInChildren<Animator>());

        //state.Animator.SetLayerWeight(0, 1);
        //state.Animator.SetLayerWeight(1, 1);

        /*state.OnFire = () =>
        {
            weapons[0].DisplayEffects(entity);
        };*/
    }

    public  void SimulateController () {//override
        PoolKeys(false);

        /*ITutorialPlayerCommandInput input = TutorialPlayerCommand.Create();

        input.Forward = _forward;
        input.Backward = _backward;
        input.Left = _left;
        input.Right = _right;
        input.Jump = _jump;
        input.Yaw = _yaw;
        input.Pitch = _pitch;

        // Кнопки мыши
        input.fire = _fire;
        input.aiming = _aiming;

        entity.QueueInput(input);
        */
    }

    // Функция выполняет коррекцию команды. Данные о коррекции делает сервер
    /*public override void ExecuteCommand (Command command, bool resetState) {
        
        TutorialPlayerCommand cmd = (TutorialPlayerCommand)command;

        if (resetState) {
            // Мы получили исправление от сервера(Reset работает долько на клиенте)
            _motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, cmd.Result.JampFrames);
        } else {
            // Применяем перемещение, это выполняется как на сервере, так и на клиенте
            PlayerMotor.State motorState = _motor.Move(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.Left, cmd.Input.Right, cmd.Input.Jump, cmd.Input.Yaw);

            // Скопируем состояние мотора, которое будет отправленно обратно клиенту
            cmd.Result.Position = motorState.position;
            cmd.Result.Velocity = motorState.velocity;
            cmd.Result.IsGrounded = motorState.isGrounded;
            cmd.Result.JampFrames = motorState.jumpFrames;

            // Animation
            if (cmd.IsFirstExecution) {
                AnimatePlayer(cmd);

                state.pitch = cmd.Input.Pitch;
            }
        }

        if (cmd.IsFirstExecution) {
            AnimatePlayer(cmd);

            state.pitch = cmd.Input.Pitch;

            if(cmd.Input.aiming && cmd.Input.fire) {
                FireWeapon(cmd);
            }
        }
        
}*/

    #endregion

    #region Custom Functions

    void PoolKeys(bool mouse) {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);
        _jump = Input.GetKeyDown(KeyCode.Space);

        // Кнопки мыши
        _fire = Input.GetMouseButton(0);
        _aiming = Input.GetMouseButton(1);

        if (mouse) {
            _yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSITIVITY);
            _yaw %= 360f;

            _pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSITIVITY);
            _pitch = Mathf.Clamp(_pitch, -85f, +85f);
        }
    }

    void AnimatePlayer () {//TutorialPlayerCommand cmd
        /*
        if (cmd.Input.Forward ^ cmd.Input.Backward) {
            state.MoveZ = cmd.Input.Forward ? 1 : -1;
        } else {
            state.MoveZ = 0;
        }

        if (cmd.Input.Left ^ cmd.Input.Right) {
            state.MoveX = cmd.Input.Right ? 1 : -1;
        } else {
            state.MoveX = 0;
        }

        // JUMP
        if (_motor.jumpStartedThisFrame) {
            state.Jump();
        }
        */
    }

    /*void FireWeapon(TutorialPlayerCommand cmd) {
        if (weapons[0].FireFrame + weapons[0].FireInterval <= BoltNetwork.ServerFrame) {
            weapons[0].FireFrame = BoltNetwork.ServerFrame;
            state.Fire();
        }
    }*/

    #endregion

}
