using System.Collections.Generic;
using UnityEngine;

namespace Kalkatos.ObjectPool
{
    public class ObjectPool : MonoBehaviour
    {
        private static ObjectPool instance;
        public static ObjectPool Instance
		{
            get { if (instance == null) instance = new GameObject("ObjectPool").AddComponent<ObjectPool>(); return instance; }
		}
        public static bool Exists => instance != null;

        private Dictionary<Component, IPooler> poolerDict = new Dictionary<Component, IPooler>();

		public static Pooler<T> GetPooler<T> (T compRef) where T : Component
        {
            return GetPooler(compRef, 20);
        }

        public static Pooler<T> GetPooler<T> (T compRef, int quantity) where T : Component
        {
            if (Instance.poolerDict.ContainsKey(compRef))
                return (Pooler<T>)Instance.poolerDict[compRef];
            Transform newPoolerObj = new GameObject(compRef.name + "-Pooler").transform;
            newPoolerObj.SetParent(Instance.transform);
            Pooler<T> newPooler = new Pooler<T>(newPoolerObj);
            newPooler.Setup(compRef, quantity);
            Instance.poolerDict.Add(compRef, newPooler);
            return newPooler;
        }

        private static void AddObject (Component compRef, Component comp)
        {
            if (Instance.poolerDict.ContainsKey(compRef))
                Instance.poolerDict[compRef].Add(comp);
        }

        private void Update ()
        {
			foreach (var item in poolerDict)
                item.Value.Update();
        }

        private void OnDestroy ()
        {
            foreach (var item in poolerDict)
                item.Value.OnDestroy();
        }

        public interface IPooler
		{
            void Add (Component comp);
            void Update ();
            void OnDestroy ();
		}

        public class Pooler<T> : IPooler where T : Component
        {
            private int instantiations;
            private List<T> list = new List<T>();
            private List<Component> unsanitizedList = new List<Component>();
            private T compRef;
            private Transform parentTransform;

            public Pooler(Transform parentToUse)
            {
                parentTransform = parentToUse;
            }

			public void Update ()
			{
                if (unsanitizedList.Count > 0)
				{
                    T comp = (T)unsanitizedList[0];
                    unsanitizedList.RemoveAt(0);
                    comp.transform.SetParent(parentTransform);
                    list.Add(comp);
				}
			}

            public void OnDestroy ()
            {
                if (instantiations > 0)
                    Debug.Log($"[Object Pool] Instantiations for {compRef.name}: {instantiations}");
            }

            public void Add (Component comp)
            {
                unsanitizedList.Add(comp);
            }

            public void Setup (T compRef, int quantity)
            {
                this.compRef = compRef;
                quantity = Mathf.Max(quantity, 10);
                for (int i = 0; i < quantity; i++)
                    list.Add(CreateObject(compRef, parentTransform));
            }

            public T GetObject (Vector3 position, Quaternion rotation)
            {
                T comp;
                if (list.Count > 0)
				{
					comp = list[0];
					list.RemoveAt(0); 
				}
                else
				{
					comp = CreateObject(compRef, parentTransform);
                    instantiations++;
                }
                comp.transform.SetPositionAndRotation(position, rotation);
                comp.transform.SetParent(null);
                comp.gameObject.SetActive(true);
                return comp;
            }

            private T CreateObject (T compRef, Transform parent)
			{
                T newComp = Instantiate(compRef, parent);
                newComp.name = compRef.name;
                newComp.gameObject.SetActive(false);
                var recycler = newComp.gameObject.AddComponent<ObjectRecycler>();
                recycler.Setup(compRef, newComp);
                return newComp;
            }
        }

        private class ObjectRecycler : MonoBehaviour
        {
            private Component compRef;
            private Component poolComponent;

            public void Setup (Component reference, Component component)
			{
                compRef = reference;
                poolComponent = component;
			}

            private void OnDisable ()
            {
                if (Exists)
                    AddObject(compRef, poolComponent);
            }
        }

    }
}
