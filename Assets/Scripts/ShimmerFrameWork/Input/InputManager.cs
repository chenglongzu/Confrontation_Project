using UnityEngine;
using UnityEngine.SceneManagement;

#if NetWorking
using ShimmerNote;
using SocketDLL;
using SocketDLL.Message;
#endif

namespace ShimmerFramework
{
    public class InputManager : BaseManager<InputManager>
    {

        public void MyUpdate()
        {
#if NetWorking

            if (SceneManager.GetActiveScene().name != "GameArena")
                return;

            if (Input.GetKey(UnityEngine.KeyCode.A))
            {
                ClientCityPlayer cityPlayer = ClientArenaPlayerManager.GetInstance().GetCityPlayerByID(ClientArenaPlayerManager.GetInstance().CurrentID);
                ShimmerNote.CharacterController playerController = cityPlayer.Player.GetComponent<ShimmerNote.CharacterController>();
                playerController.CharacterMove(UnityEngine.KeyCode.A);

            }

            if (Input.GetKey(UnityEngine.KeyCode.S))
            {
                ClientCityPlayer cityPlayer = ClientArenaPlayerManager.GetInstance().GetCityPlayerByID(ClientArenaPlayerManager.GetInstance().CurrentID);
                ShimmerNote.CharacterController playerController = cityPlayer.Player.GetComponent<ShimmerNote.CharacterController>();
                playerController.CharacterMove(UnityEngine.KeyCode.S);

            }

            if (Input.GetKey(UnityEngine.KeyCode.W))
            {
                ClientCityPlayer cityPlayer = ClientArenaPlayerManager.GetInstance().GetCityPlayerByID(ClientArenaPlayerManager.GetInstance().CurrentID);
                ShimmerNote.CharacterController playerController = cityPlayer.Player.GetComponent<ShimmerNote.CharacterController>();
                playerController.CharacterMove(UnityEngine.KeyCode.W);
            }

            if (Input.GetKey(UnityEngine.KeyCode.D))
            {
                ClientCityPlayer cityPlayer = ClientArenaPlayerManager.GetInstance().GetCityPlayerByID(ClientArenaPlayerManager.GetInstance().CurrentID);
                ShimmerNote.CharacterController playerController = cityPlayer.Player.GetComponent<ShimmerNote.CharacterController>();
                playerController.CharacterMove(UnityEngine.KeyCode.D);
            }
#endif
        }

#if NetWorking
        private void SendMoveMessage(InputInfo inputInfo)
        {
            SocketMessage message = new SocketMessage();
            message.Head = MessageHead.CS_Input;
            message.Body = inputInfo;

            byte[] mess = SocketTools.Serialize(message);
            ClientManager.GetInstance().Send(mess);
        }
#endif
    }
}