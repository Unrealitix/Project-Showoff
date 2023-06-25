using System.Collections;
using Physics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
	public class PlayerJoin : MonoBehaviour
	{
		[SerializeField] private UnityEngine.Camera menuCamera;

		[SerializeField] private TMP_Text player1;
		private string _player1Template;
		[SerializeField] private TMP_Text player2;
		private string _player2Template;

		[SerializeField] private int countdownSeconds = 5;

		[SerializeField] private TMP_Text countdown;
		private string _countdownTemplate;

		[SerializeField] private Transform player1Spawn;
		[SerializeField] private Transform player2Spawn;

		private PlayerInputManager _playerInputManager;

		private void Awake()
		{
			_playerInputManager = GetComponent<PlayerInputManager>();
			_playerInputManager.onPlayerJoined += OnPlayerJoined;

			_player1Template = player1.text;
			_player2Template = player2.text;
			_countdownTemplate = countdown.text;

			player1.text = string.Format(_player1Template, "Press 🅁/Ⓧ to start");
			player2.text = string.Format(_player2Template, "Press 🅁/Ⓧ to join");
			countdown.gameObject.SetActive(false);
		}

		private void OnPlayerJoined(PlayerInput obj)
		{
			Debug.Log($"Player {obj.playerIndex} joined");
			switch (obj.playerIndex)
			{
				case 0:
					player1.text = string.Format(_player1Template, "Connected");
					obj.GetComponent<ShipControls>().Spawn(player1Spawn);
					StartCoroutine(Countdown());
					break;
				case 1:
					player2.text = string.Format(_player2Template, "Connected");
					obj.GetComponent<ShipControls>().Spawn(player2Spawn);
					StopCoroutine(Countdown());
					StartGame();
					break;
			}
		}

		private IEnumerator Countdown()
		{
			countdown.text = string.Format(_countdownTemplate, countdownSeconds);
			countdown.gameObject.SetActive(true);

			for (int i = 0; i < countdownSeconds; i++)
			{
				countdown.text = string.Format(_countdownTemplate, countdownSeconds - i);
				yield return new WaitForSeconds(1);
			}

			StartGame();
		}

		private void StartGame()
		{
			menuCamera.gameObject.SetActive(false);
			_playerInputManager.DisableJoining();
		}
	}
}