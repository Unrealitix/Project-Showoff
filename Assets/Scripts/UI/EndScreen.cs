using TMPro;
using UnityEngine;

namespace UI
{
	public class EndScreen : MonoBehaviour
	{
		[SerializeField] private LapAndTimer lapAndTimer;
		[SerializeField] private TMP_Text timerText;

		private void Awake()
		{
			lapAndTimer.onFinish.AddListener(ShowEndScreen);
			gameObject.SetActive(false);
		}

		private void ShowEndScreen(float totalTime)
		{
			timerText.text = LapAndTimer.ShowTimer(totalTime);
			gameObject.SetActive(true);
		}
	}
}
