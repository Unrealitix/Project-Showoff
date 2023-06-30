using System.Collections;
using Physics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UI.Menu
{
	public class PlayerJoin : MonoBehaviour
	{
		[SerializeField] private TMP_Text player1;
		private string _player1Template;
		[SerializeField] private TMP_Text player2;
		private string _player2Template;

		[SerializeField] private int countdownSeconds = 5;

		[SerializeField] private TMP_Text countdown;
		private string _countdownTemplate;

		[SerializeField] private Transform player1Spawn;
		[SerializeField] private Transform player2Spawn;

		public UnityEvent onStart;
		private Coroutine _countdownCoroutine;

		private PlayerInputManager _playerInputManager;

		private void Awake()
		{
			_playerInputManager = GetComponent<PlayerInputManager>();
			_playerInputManager.onPlayerJoined += OnPlayerJoined;

			_player1Template = player1.text;
			_player2Template = player2.text;
			_countdownTemplate = countdown.text;

			player1.text = string.Format(_player1Template, "Press the Join button to start");
			player2.text = string.Format(_player2Template, "Press the Join button to join");
			countdown.gameObject.SetActive(false);

			Cursor.visible = false;
		}

		private void OnPlayerJoined(PlayerInput obj)
		{
			Debug.Log($"Player {obj.playerIndex} joined");
			switch (obj.playerIndex)
			{
				case 0:
					player1.text = string.Format(_player1Template, "Connected");
					obj.GetComponent<ShipControls>().Spawn(player1Spawn, onStart);
					_countdownCoroutine = StartCoroutine(Countdown());
					break;
				case 1:
					player2.text = string.Format(_player2Template, "Connected");
					obj.GetComponent<ShipControls>().Spawn(player2Spawn, onStart);
					StopCoroutine(_countdownCoroutine);
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
			//disable main menu
			Canvas canvas = GetComponent<Canvas>();
			canvas.enabled = false;
			canvas.worldCamera.gameObject.SetActive(false);

			//prevent new players from joining
			_playerInputManager.DisableJoining();
			onStart.Invoke();
		}
	}
}
