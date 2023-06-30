using Generated;
using static UnityEngine.InputSystem.InputAction;

namespace UI.Menu
{
	public class SwitchControlsView : EventViewNavigation
	{
		private Controls _controls;

		private void OnEnable()
		{
			_controls = new Controls();
			_controls.Enable();
			_controls.Hover.SeeControls.performed += SwitchControls;
		}

		private void SwitchControls(CallbackContext obj)
		{
			SwitchPanel();
		}

		private void OnDisable()
		{
			_controls.Hover.SeeControls.performed -= SwitchControls;
			_controls.Disable();
		}
	}
}
