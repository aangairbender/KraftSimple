using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public abstract class View<VM> : MonoBehaviour
        where VM : ViewModel
    {
        private VM vm;
        private Dictionary<string, Action> propertyChangedHandlers = new();

        protected abstract void SetupViewModelBindings();
        //protected abstract void OnViewModelSet();
        //protected abstract void OnViewModelUnset();

        protected void Bind<T>(Expression<Func<VM, T>> property, Action handler)
        {
            var propertyName = property.GetMemberInfo().Name;
            if (propertyChangedHandlers.ContainsKey(propertyName))
            {
                throw new ArgumentException($"Handler for the property {propertyName} already registered");
            }
            propertyChangedHandlers.Add(propertyName, handler);
        }

        public VM ViewModel
        {
            get => vm;
            set
            {
                if (vm != null)
                {
                    vm.PropertyChanged -= Vm_PropertyChanged;
                    //OnViewModelUnset();
                }
                vm = value;
                if (vm != null)
                {
                    //OnViewModelSet();
                    InvokeAllHandlers();
                    vm.PropertyChanged += Vm_PropertyChanged;
                }
            }
        }

        private void InvokeAllHandlers()
        {
            foreach (var handler in propertyChangedHandlers.Values)
            {
                handler.Invoke();
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "" || e.PropertyName == null)
            {
                InvokeAllHandlers();
            } else if (propertyChangedHandlers.ContainsKey(e.PropertyName))
            {
                propertyChangedHandlers[e.PropertyName].Invoke();
            }
        }

        protected virtual void Awake()
        {
            SetupViewModelBindings();
        }
    }
}
