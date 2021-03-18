using UnityEngine;
using UnityEngine.Events;

namespace ShimmerFramework
{
    public class MonoController : SingletonAutoMono<MonoController>
    {

        public event UnityAction updateEvent;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {

            if (updateEvent != null)
            {
                updateEvent();
            }
        }

        public void AddUpdateAction(UnityAction updateEvent)
        {
            this.updateEvent += updateEvent;
        }

        public void RemoveAction(UnityAction updateEvent)
        {
            this.updateEvent -= updateEvent;
        }

        public GameObject InstantiationGameobject(GameObject gameObject)
        {
            return Instantiate<GameObject>(gameObject);
        }
    }
}