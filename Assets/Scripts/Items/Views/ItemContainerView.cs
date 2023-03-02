using Assets.Scripts.Common;
using Assets.Scripts.Items.ViewModels;
using UnityEngine;

namespace Assets.Scripts.Items.Views
{
    public class ItemContainerView : View<ItemContainerVM>
    {
        protected override void OnViewModelSet()
        {
            this.gameObject.SetActive(ViewModel.Visible.Value);

            ViewModel.Visible.OnChanged += Visible_OnChanged;
        }

        protected override void OnViewModelUnset()
        {
            ViewModel.Visible.OnChanged -= Visible_OnChanged;
        }

        private void Visible_OnChanged(bool newValue, bool oldValue)
        {
            this.gameObject.SetActive(newValue);
        }
    }
}
