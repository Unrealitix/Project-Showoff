using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
	[SerializeField] private Rigidbody followRigidbody;
	[SerializeField] private float multiplier = 1f;

	private TMP_Text _textComponent;
	private string _textTemplate;

	private void Awake()
	{
		_textComponent = GetComponent<TMP_Text>();
		_textTemplate = _textComponent.text;
	}

	private void Update()
	{
		string speed = (followRigidbody.velocity.magnitude * multiplier).ToString("F1"); //F1 = round to 1 decimal place
		_textComponent.text = string.Format(_textTemplate, speed);
	}
}
