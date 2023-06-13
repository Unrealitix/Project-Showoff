using System;
using System.Linq;
using Physics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class Speedometer : MonoBehaviour
	{
		[SerializeField] private Rigidbody followRigidbody;
		[SerializeField] private float multiplier = 1f;

		private TMP_Text _textComponent;
		private string _textTemplate;

		private Image _barImage;
		private float _maxSpeed;

		private void Awake()
		{
			//Text
			_textComponent = GetComponentsInChildren<TMP_Text>().First(text => text.name.Contains("speed", StringComparison.OrdinalIgnoreCase));
			_textTemplate = _textComponent.text;

			//Bar
			_barImage = GetComponentInChildren<Image>();
			_maxSpeed = followRigidbody.GetComponent<ShipControls>().MaxSpeed;
		}

		private void Update()
		{
			float speed = followRigidbody.velocity.magnitude;
			string text = (speed * multiplier).ToString("F0"); //F0 = round to 0 decimal places
			_textComponent.text = string.Format(_textTemplate, text);

			_barImage.fillAmount = speed / _maxSpeed;
		}
	}
}
