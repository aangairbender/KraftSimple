using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public abstract class View<VM> : MonoBehaviour
    {
        private VM vm;

        protected abstract void OnViewModelSet();
        protected abstract void OnViewModelUnset();

        public VM ViewModel
        {
            get => vm;
            set
            {
                if (vm != null)
                {
                    OnViewModelUnset();
                }
                vm = value;
                if (vm != null)
                {
                    OnViewModelSet();
                }
            }
        }
    }
}
