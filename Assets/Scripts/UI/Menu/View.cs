using UnityEngine;

namespace UI.Menu
{
    /// <summary>
    /// The idea of any View subclass is that it wraps all (well most anyway) details of the underlying components
    /// from the rest of the application. You create a view class, specify the components it requires to function,
    /// hook those up in the inspector and the rest of the application simply accesses the specific view instance
    /// to do its work.
    ///
    /// To implement the View switcher we need to write an editor class and the editor class requires a component,
    /// hence this empty example component class.
    /// </summary>
    public class View : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
