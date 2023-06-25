using Generated;
using static UnityEngine.InputSystem.InputAction;

namespace UI
{
	public class SwitchBackFromControlsView : EventViewNavigation
	{
		private Controls _controls;

		private void Start()
		{
			_controls = new Controls();
			_controls.Enable();
			_controls.Hover.GoBackFromControls.performed += OnGoBackFromControls;
		}

		private void OnGoBackFromControls(CallbackContext obj)
		{
			SwitchPanel();
		}
	}
}
