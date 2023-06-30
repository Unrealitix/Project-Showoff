using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
	public class SlideshowChildren : MonoBehaviour
	{
		[SerializeField] private float delayBetweenSlides = 5;
		[SerializeField] private Slider progressSlider;

		private List<GameObject> _children;

		[ShowNonSerializedField] private int _index;
		[ShowNonSerializedField] private float _timer;

		private void Awake()
		{
			_children = new List<GameObject>();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (!child.GetComponent<Slider>())
					_children.Add(child.gameObject);
			}

			progressSlider.maxValue = delayBetweenSlides;

			_index = 0;
			ShowChild(_index);
		}

		private void Update()
		{
			_timer += Time.deltaTime;
			progressSlider.value = _timer;
			if (_timer >= delayBetweenSlides)
			{
				_timer = 0;
				NextChild();
			}
		}

		private void NextChild()
		{
			_index++;
			if (_index >= _children.Count)
				_index = 0;

			ShowChild(_index);
		}

		private void ShowChild(int i)
		{
			foreach (GameObject child in _children)
				child.SetActive(false);

			_children[i].SetActive(true);
		}
	}
}
