using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace CustomLayoutGroups
{
    public abstract class CustomLayoutGroup : MonoBehaviour
    {
        [SerializeField] protected LayoutGroupMode layoutGroupMode;

        [SerializeField, ShowIf("@layoutGroupMode == LayoutGroupMode.GameObject")]
        protected Vector2 layoutSize = Vector2.one;

        [SerializeField] protected bool useLayoutSize;
        [SerializeField] protected bool autoUpdate = true;
        [SerializeField] protected bool autoAddChildElements;

        [SerializeField, ShowIf("@!useLayoutSize")]
        protected float spacing = 0.1f;

        [SerializeField] protected List<RectTransform> layoutElementsUI = new List<RectTransform>();
        [SerializeField] protected List<GameObject> layoutElementsGameObjects = new List<GameObject>();


        private void OnValidate()
        {
            AddChildElements();
            UpdateLayout();
        }



        private void OnTransformChildrenChanged()
        {
            AddChildElements();
        }


        public void AddElement(RectTransform element)
        {
            if (element != null && !layoutElementsUI.Contains(element))
            {
                element.SetParent(transform);
                layoutElementsUI.Add(element);

                if (autoUpdate)
                {
                    UpdateLayout();
                }
            }
        }


        public void AddElement(GameObject element)
        {
            if (element != null && !layoutElementsGameObjects.Contains(element))
            {
                element.transform.SetParent(transform);
                layoutElementsGameObjects.Add(element);

                if (autoUpdate)
                {
                    UpdateLayout();
                }
            }
        }


        public void RemoveElement(RectTransform element)
        {
            if (element != null && layoutElementsUI.Contains(element))
            {
                if (element.parent == transform)
                {
                    element.SetParent(null);
                }

                layoutElementsUI.Remove(element);

                if (autoUpdate)
                {
                    UpdateLayout();
                }
            }
        }


        public void RemoveElement(GameObject element)
        {
            if (element != null && layoutElementsGameObjects.Contains(element))
            {
                if (element.transform.parent == transform)
                {
                    element.transform.SetParent(null);
                }

                layoutElementsGameObjects.Remove(element);

                if (autoUpdate)
                {
                    UpdateLayout();
                }
            }
        }


        [Button]
        public void UpdateLayout()
        {
            if (layoutGroupMode == LayoutGroupMode.GameObject)
            {
                if (layoutElementsGameObjects == null || layoutElementsGameObjects.Count == 0) return;
            }
            else
            {
                if (layoutElementsUI == null || layoutElementsUI.Count == 0) return;
            }


            if (!useLayoutSize)
            {
                if (layoutGroupMode == LayoutGroupMode.GameObject)
                {
                    GameObjectLayoutWithFixedSpacing();
                }
                else
                {
                    UILayoutWithFixedSpacing();
                }
            }
            else
            {
                if (layoutGroupMode == LayoutGroupMode.GameObject)
                {
                    GameObjectLayoutWithContainerSize();
                }
                else
                {
                    UILayoutWithContainerSize();
                }
            }
        }


        public void ClearElements()
        {
            layoutElementsGameObjects.Clear();
            layoutElementsUI.Clear();
        }


        private void AddChildElements()
        {
            if (autoAddChildElements)
            {
                if (layoutGroupMode == LayoutGroupMode.GameObject)
                {
                    layoutElementsGameObjects.Clear();

                    foreach (Transform child in transform)
                    {
                        if (!layoutElementsGameObjects.Contains(child.gameObject))
                        {
                            layoutElementsGameObjects.Add(child.gameObject);
                        }
                    }
                }
                else
                {
                    layoutElementsUI.Clear();

                    foreach (RectTransform child in transform)
                    {
                        if (!layoutElementsUI.Contains(child))
                        {
                            layoutElementsUI.Add(child);
                        }
                    }
                }
            }
        }


        protected abstract void UILayoutWithFixedSpacing();

        protected abstract void GameObjectLayoutWithFixedSpacing();

        protected abstract void UILayoutWithContainerSize();

        protected abstract void GameObjectLayoutWithContainerSize();


        protected void MoveElement(RectTransform layoutElement, Vector2 targetLocalPosition)
        {
            if (layoutElement.TryGetComponent(out ILayoutElementMover layoutElementAnimator))
            {
                layoutElementAnimator.MoveElement(targetLocalPosition);
            }
            else
            {
                layoutElement.anchoredPosition = targetLocalPosition;
            }
        }


        protected void MoveElement(GameObject layoutElement, Vector3 targetLocalPosition)
        {
            if (layoutElement.TryGetComponent(out ILayoutElementMover layoutElementMover))
            {
                layoutElementMover.MoveElement(targetLocalPosition);
            }
            else
            {
                layoutElement.transform.localPosition = targetLocalPosition;
            }
        }


        protected virtual void OnDrawGizmos()
        {
            if (layoutGroupMode == LayoutGroupMode.GameObject)
            {
                Gizmos.color = Color.red;

                Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

                Gizmos.matrix = matrix;

                Gizmos.DrawWireCube(Vector3.zero, layoutSize);

                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}
