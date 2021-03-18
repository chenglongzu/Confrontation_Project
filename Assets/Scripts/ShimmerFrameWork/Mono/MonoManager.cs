using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShimmerFramework
{
    public class MonoManager : BaseManager<MonoManager>
    {

        private MonoController monoController;

        public MonoManager()
        {
            GameObject obj = new GameObject("MonoController");
            monoController = obj.AddComponent<MonoController>();
        }


        public void AddUpdateAction(UnityAction updateEvent)
        {
            monoController.AddUpdateAction(updateEvent);
        }

        public void RemoveUpdateAction(UnityAction updateEvent)
        {
            monoController.RemoveAction(updateEvent);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return monoController.StartCoroutine(routine);
        }
        public Coroutine StartCoroutine(string methodName, object value)
        {
            return monoController.StartCoroutine(methodName, value);
        }
        public Coroutine StartCoroutine(string methodName)
        {
            return monoController.StartCoroutine(methodName);
        }


        public GameObject InstantiationGameobject(GameObject gameObject)
        {
            return monoController.InstantiationGameobject(gameObject);
        }
    }
}