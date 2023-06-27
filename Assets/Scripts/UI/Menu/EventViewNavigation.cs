using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(View))]
	public class EventViewNavigation : MonoBehaviour
	{
		[SerializeField] private View gotoView;

		private View _thisView;

		private void Awake()
		{
			if (gotoView == null)
				Debug.LogError("Goto View is null");
			_thisView = GetComponent<View>();
		}

		protected void SwitchPanel()
		{
			_thisView.Hide();
			gotoView.Show();
		}

		protected void HideThisPanel()
		{
			_thisView.Hide();
		}
	}
}
