using Generated;
using static UnityEngine.InputSystem.InputAction;

namespace UI.Menu
{
	public class SwitchToControlsView : EventViewNavigation
	{
		private Controls _controls;

		private void Start()
		{
			_controls = new Controls();
			_controls.Enable();
			_controls.Hover.GoToControls.performed += OnGoToControls;
		}

		private void OnGoToControls(CallbackContext obj)
		{
			SwitchPanel();
		}
	}
}
