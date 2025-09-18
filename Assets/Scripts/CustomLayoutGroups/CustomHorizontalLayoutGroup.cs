using Sirenix.OdinInspector;
using UnityEngine;


namespace CustomLayoutGroups
{
    public class CustomHorizontalLayoutGroup : CustomLayoutGroup
    {
        [SerializeField] private HorizontalElementsAlignment horizontalElementsAlignment;

        [SerializeField, ShowIf("@layoutGroupMode == LayoutGroupMode.GameObject")]
        private bool useElementMeshWidth;

        [SerializeField, ShowIf("@layoutGroupMode == LayoutGroupMode.GameObject && !useElementMeshWidth")]
        private float elementWidth = 1f;


        protected override void UILayoutWithFixedSpacing()
        {
            RectTransform rectTransform = transform as RectTransform;
            float currentX = 0;

            switch (horizontalElementsAlignment)
            {
                case HorizontalElementsAlignment.Center:
                {
                    float totalWidth = 0;

                    foreach (RectTransform layoutElement in layoutElementsUI)
                    {
                        totalWidth += layoutElement.rect.width;
                    }

                    float totalSpacing = spacing * (layoutElementsUI.Count - 1);
                    totalWidth += totalSpacing;

                    currentX = -totalWidth / 2f;
                    break;
                }
                case HorizontalElementsAlignment.Left:
                    currentX = rectTransform != null ? -rectTransform.rect.width / 2 : 0;
                    break;
                case HorizontalElementsAlignment.Right:
                    currentX = rectTransform != null ? rectTransform.rect.width / 2 : 0;
                    break;
            }

            foreach (RectTransform layoutElement in layoutElementsUI)
            {
                float rectWidth = layoutElement.rect.width;

                float elementX = horizontalElementsAlignment == HorizontalElementsAlignment.Right
                    ? currentX - rectWidth / 2f
                    : currentX + rectWidth / 2f;

                Vector2 elementLocalPosition = new Vector2(elementX, 0);
                MoveElement(layoutElement, elementLocalPosition);

                switch (horizontalElementsAlignment)
                {
                    case HorizontalElementsAlignment.Right:
                    {
                        currentX -= rectWidth + spacing;
                        break;
                    }
                    default:
                    {
                        currentX += rectWidth + spacing;
                        break;
                    }
                }
            }
        }


        protected override void GameObjectLayoutWithFixedSpacing()
        {
            float currentX = 0;

            switch (horizontalElementsAlignment)
            {
                case HorizontalElementsAlignment.Center:
                {
                    float totalWidth = 0;

                    foreach (GameObject layoutElement in layoutElementsGameObjects)
                    {
                        if (useElementMeshWidth)
                        {
                            Renderer rend = layoutElement.GetComponent<Renderer>();

                            if (rend == null)
                            {
                                rend = layoutElement.GetComponentInChildren<Renderer>();

                                if (rend == null)
                                {
                                    Debug.LogError(
                                        $"No renderer found in {layoutElement.name} or its children. Try to set 'useElementMeshWidth' to false and set custom element width");
                                    return;
                                }
                            }

                            totalWidth += rend.bounds.size.x;
                        }
                        else
                        {
                            totalWidth += elementWidth;
                        }
                    }

                    float totalSpacing = spacing * (layoutElementsGameObjects.Count - 1);
                    totalWidth += totalSpacing;

                    currentX = -totalWidth / 2f;
                    break;
                }
                case HorizontalElementsAlignment.Left:
                    currentX = -layoutSize.x / 2;
                    break;
                case HorizontalElementsAlignment.Right:
                    currentX = layoutSize.x / 2;
                    break;
            }

            foreach (GameObject layoutElement in layoutElementsGameObjects)
            {
                float width;

                if (useElementMeshWidth)
                {
                    Renderer rend = layoutElement.GetComponent<Renderer>();

                    if (rend == null)
                    {
                        rend = layoutElement.GetComponentInChildren<Renderer>();

                        if (rend == null)
                        {
                            Debug.LogError(
                                $"No renderer found in {layoutElement.name} or its children. Try to set 'useElementMeshWidth' to false and set custom element width");
                            return;
                        }
                    }

                    width = rend.bounds.size.x;
                }
                else
                {
                    width = elementWidth;
                }

                float elementX = horizontalElementsAlignment == HorizontalElementsAlignment.Right
                    ? currentX - width / 2f
                    : currentX + width / 2f;

                Vector3 elementLocalPosition = new Vector3(elementX, 0, 0);
                MoveElement(layoutElement, elementLocalPosition);

                switch (horizontalElementsAlignment)
                {
                    case HorizontalElementsAlignment.Right:
                    {
                        currentX -= width + spacing;
                        break;
                    }
                    default:
                    {
                        currentX += width + spacing;
                        break;
                    }
                }
            }
        }


        protected override void UILayoutWithContainerSize()
        {
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform == null)
                return;

            float availableWidth = rectTransform.rect.width;
            bool isSingleElement = layoutElementsUI.Count == 1;

            float totalElementsWidth = 0;

            foreach (RectTransform layoutElement in layoutElementsUI)
            {
                totalElementsWidth += layoutElement.rect.width;
            }

            float effectiveSpacing = layoutElementsUI.Count > 1
                ? (availableWidth - totalElementsWidth) / (layoutElementsUI.Count - 1)
                : 0;

            float currentX = horizontalElementsAlignment switch
            {
                HorizontalElementsAlignment.Center when isSingleElement => 0,
                HorizontalElementsAlignment.Center => -availableWidth / 2f,
                HorizontalElementsAlignment.Left => -availableWidth / 2f,
                HorizontalElementsAlignment.Right => availableWidth / 2f,
                _ => -availableWidth / 2f
            };

            foreach (RectTransform layoutElement in layoutElementsUI)
            {
                float rectWidth = layoutElement.rect.width;

                float elementX = horizontalElementsAlignment switch
                {
                    HorizontalElementsAlignment.Center when isSingleElement => 0,
                    HorizontalElementsAlignment.Right => currentX - rectWidth / 2f,
                    _ => currentX + rectWidth / 2f
                };

                Vector2 elementLocalPosition = new Vector2(elementX, 0);
                MoveElement(layoutElement, elementLocalPosition);

                currentX += horizontalElementsAlignment == HorizontalElementsAlignment.Right
                    ? -(rectWidth + effectiveSpacing)
                    : rectWidth + effectiveSpacing;
            }
        }


        protected override void GameObjectLayoutWithContainerSize()
        {
            float availableWidth = layoutSize.x;
            bool isSingleElement = layoutElementsGameObjects.Count == 1;

            float totalElementsWidth = 0;

            foreach (GameObject layoutElement in layoutElementsGameObjects)
            {
                if (useElementMeshWidth)
                {
                    Renderer rend = layoutElement.GetComponent<Renderer>();

                    if (rend == null)
                    {
                        rend = layoutElement.GetComponentInChildren<Renderer>();

                        if (rend == null)
                        {
                            Debug.LogError(
                                $"No renderer found in {layoutElement.name} or its children. Try to set 'useElementMeshWidth' to false and set custom element width");
                            return;
                        }
                    }

                    totalElementsWidth += rend.bounds.size.x;
                }
                else
                {
                    totalElementsWidth += elementWidth;
                }
            }

            float effectiveSpacing = layoutElementsGameObjects.Count > 1
                ? (availableWidth - totalElementsWidth) / (layoutElementsGameObjects.Count - 1)
                : 0;

            float currentX = horizontalElementsAlignment switch
            {
                HorizontalElementsAlignment.Center when isSingleElement => 0,
                HorizontalElementsAlignment.Center => -availableWidth / 2f,
                HorizontalElementsAlignment.Left => -availableWidth / 2f,
                HorizontalElementsAlignment.Right => availableWidth / 2f,
                _ => -availableWidth / 2f
            };

            foreach (GameObject layoutElement in layoutElementsGameObjects)
            {
                float width;

                if (useElementMeshWidth)
                {
                    Renderer rend = layoutElement.GetComponent<Renderer>();

                    if (rend == null)
                    {
                        rend = layoutElement.GetComponentInChildren<Renderer>();

                        if (rend == null)
                        {
                            Debug.LogError(
                                $"No renderer found in {layoutElement.name} or its children. Try to set 'useElementMeshWidth' to false and set custom element width");
                            return;
                        }
                    }

                    width = rend.bounds.size.x;
                }
                else
                {
                    width = elementWidth;
                }

                float elementX = horizontalElementsAlignment switch
                {
                    HorizontalElementsAlignment.Center when isSingleElement => 0,
                    HorizontalElementsAlignment.Right => currentX - width / 2f,
                    _ => currentX + width / 2f
                };

                Vector3 elementLocalPosition = new Vector3(elementX, 0, 0);
                MoveElement(layoutElement, elementLocalPosition);

                currentX += horizontalElementsAlignment == HorizontalElementsAlignment.Right
                    ? -(width + effectiveSpacing)
                    : width + effectiveSpacing;
            }
        }


        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.blue;

            if (!useElementMeshWidth)
            {
                foreach (GameObject layoutElement in layoutElementsGameObjects)
                {
                    Gizmos.DrawWireCube(layoutElement.transform.position, new Vector3(elementWidth, 1, 0));
                }
            }
        }
    }
}
