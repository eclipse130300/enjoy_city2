using Bolt;
using UnityEngine;

namespace SocialGTA.Network {

    public enum InputType {
        Mobil = 0,
        Keyboard = 1,
        Joystick = 2
    }

    public class PlayerBase : EntityEventListener<IPlayerState> {

        #region Variables

        [Header("Character Settings")]
        [SerializeField] float _sensetiveMyltipler = 0.3f;
        [SerializeField] InputType inputType = InputType.Mobil;

        const float MOUSE_SENSITIVITY = 2f;

        bool _forward;
        bool _backward;
        bool _left;
        bool _right;
        bool _jump;

        float _yaw;
        float _pitch;

        PlayerMotor _motor;

        private Vector2 _deltaPos;
        private GameCanvasController _gameCanvas;
        
        #endregion

        #region Standart Functions

        private void Awake () {
            _motor = GetComponent<PlayerMotor>();
        }

        private void Update () {
            PollKeys(true);
        }

        #endregion

        #region Bolt Functions

        public override void Attached() {
            _gameCanvas = GameCanvasController.instance;
            state.SetTransforms(state.PlayerTransform, transform);
        }

        public override void SimulateController() {
            PollKeys(false);
            
            IPlayerInputCommandInput input = PlayerInputCommand.Create();

            input.Forward = _forward;
            input.Backward = _backward;
            input.Left = _left;
            input.Right = _right;
            input.Jump = _jump;
            input.Yaw = _yaw;
            input.Pitch = _pitch;

            entity.QueueInput(input);
        }

        public override void ExecuteCommand (Command command, bool resetState) {
            PlayerInputCommand cmd = (PlayerInputCommand)command;

            if (resetState) {
                // we got a correction from the server, reset (this only runs on the client)
                _motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, cmd.Result.JumpFrames);
            } else {
                // apply movement (this runs on both server and client)
                PlayerMotor.State motorState = _motor.Move(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.Left, cmd.Input.Right, cmd.Input.Jump, cmd.Input.Yaw);

                // copy the motor state to the commands result (this gets sent back to the client)
                cmd.Result.Position = motorState.position;
                cmd.Result.Velocity = motorState.velocity;
                cmd.Result.IsGrounded = motorState.isGrounded;
                cmd.Result.JumpFrames = motorState.jumpFrames;

                if (cmd.IsFirstExecution)
                    AnimatePlayer(cmd);
            }
        }

        #endregion

        #region Custom Functions

        private void PollKeys (bool mouse) {

            switch (inputType) {
                case InputType.Keyboard:
                    _forward = Input.GetKey(KeyCode.W);// ? 1 : 0;
                    _backward = Input.GetKey(KeyCode.S);// ? -1 : 0;
                    _left = Input.GetKey(KeyCode.A);// ? -1 : 0;
                    _right = Input.GetKey(KeyCode.D);// ? 1 : 0;
                    _jump = Input.GetKeyDown(KeyCode.Space);

                    if (mouse) {
                        _yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSITIVITY);
                        _yaw %= 360f;

                        _pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSITIVITY);
                        _pitch = Mathf.Clamp(_pitch, -85f, +85f);
                    }
                break;
                case InputType.Mobil:
                    _forward = _gameCanvas.Vertical > 0.5;// ? _gameCanvas.Vertical : 0;
                    _backward = _gameCanvas.Vertical < -0.5;// ? _gameCanvas.Vertical : 0;
                    _left = _gameCanvas.Horizontal < -0.5f;// ? _gameCanvas.Horizontal : 0;
                    _right = _gameCanvas.Horizontal > 0.5f;// ? _gameCanvas.Horizontal : 0;
                    _jump = _gameCanvas.IsJump;

                    if (mouse) {
                        if (Input.touchCount > 0) {
                            _deltaPos = _gameCanvas.TouchDirecition;

                            _yaw += (_deltaPos.x * _sensetiveMyltipler * MOUSE_SENSITIVITY);
                            _yaw %= 360f;

                            _pitch += (-_deltaPos.y * _sensetiveMyltipler * MOUSE_SENSITIVITY);
                            _pitch = Mathf.Clamp(_pitch, -85f, +85f);
                        }
                    }
               break;
            }
        }

        private void AnimatePlayer(PlayerInputCommand cmd) {
            //float moveZ = cmd.Input.Forward + cmd.Input.Backward;
            //float moveX = cmd.Input.Left + cmd.Input.Right;

            //state.MoveZ = moveZ;
            //state.MoveX = moveX;

            if (cmd.Input.Forward ^ cmd.Input.Backward) {
                state.MoveZ = cmd.Input.Forward ? 1 : -1;
            } else {
                state.MoveZ = 0;
            }

            // LEFT <> RIGHT movement
            if (cmd.Input.Left ^ cmd.Input.Right) {
                state.MoveX = cmd.Input.Right ? 1 : -1;
            } else {
                state.MoveX = 0;
            }

            //Debug.Log(string.Format("X: {0}, Z: {1}", moveX, moveZ));

            if (_motor.jumpStartedThisFrame)
                state.Jump();
        }

        #endregion
    }
}
