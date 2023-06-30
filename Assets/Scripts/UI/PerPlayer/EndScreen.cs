using Checkpoints;
using TMPro;
using UnityEngine;

namespace UI.PerPlayer
{
	public class EndScreen : MonoBehaviour
	{
		[SerializeField] private LapAndTimer lapAndTimer;
		[SerializeField] private TMP_Text timerText;
		[SerializeField] private TMP_Text placeText;
		[SerializeField] private RectTransform tagContainer;

		private string _placeTemplate;

		private void Awake()
		{
			_placeTemplate = placeText.text;

			lapAndTimer.onFinish.AddListener(ShowEndScreen);
			gameObject.SetActive(false);
		}

		private void ShowEndScreen(float totalTime)
		{
			timerText.text = LapAndTimer.ShowTimer(totalTime);

			int place = lapAndTimer.GetComponent<CheckpointTracker>().Place;

			placeText.text = string.Format(_placeTemplate, place, place == 1 ? "st" : "nd");

			tagContainer.gameObject.SetActive(place == 1);

			gameObject.SetActive(true);
		}
	}
}
