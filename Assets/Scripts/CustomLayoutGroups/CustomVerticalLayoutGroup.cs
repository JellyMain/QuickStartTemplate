using Sirenix.OdinInspector;
using UnityEngine;


namespace CustomLayoutGroups
{
    public class CustomVerticalLayoutGroup : CustomLayoutGroup
    {
        [SerializeField] private VerticalElementsAlignment verticalElementsAlignment;

        [SerializeField, ShowIf("@layoutGroupMode == LayoutGroupMode.GameObject")]
        private bool useElementMeshHeight;

        [SerializeField, ShowIf("@layoutGroupMode == LayoutGroupMode.GameObject && !useElementMeshHeight")]
        private float elementHeight = 1f;


        protected override void UILayoutWithFixedSpacing()
        {
            RectTransform rectTransform = transform as RectTransform;
            float currentY = 0;

            switch (verticalElementsAlignment)
            {
                case VerticalElementsAlignment.Center:
                {
                    float totalHeight = 0;

                    foreach (RectTransform layoutElement in layoutElementsUI)
                    {
                        totalHeight += layoutElement.rect.height;
                    }

                    float totalSpacing = spacing * (layoutElementsUI.Count - 1);
                    totalHeight += totalSpacing;

                    currentY = -totalHeight / 2f;
                    break;
                }
                case VerticalElementsAlignment.Lower:
                    currentY = rectTransform != null ? -rectTransform.rect.height / 2 : 0;
                    break;
                case VerticalElementsAlignment.Upper:
                    currentY = rectTransform != null ? rectTransform.rect.height / 2 : 0;
                    break;
            }

            foreach (RectTransform layoutElement in layoutElementsUI)
            {
                float rectHeight = layoutElement.rect.height;

                float elementY = verticalElementsAlignment == VerticalElementsAlignment.Upper
                    ? currentY - rectHeight / 2f
                    : currentY + rectHeight / 2f;

                Vector2 elementLocalPosition = new Vector2(0, elementY);
                MoveElement(layoutElement, elementLocalPosition);

                switch (verticalElementsAlignment)
                {
                    case VerticalElementsAlignment.Upper:
                    {
                        currentY -= rectHeight + spacing;
                        break;
                    }
                    default:
                    {
                        currentY += rectHeight + spacing;
                        break;
                    }
                }
            }
        }


        protected override void GameObjectLayoutWithFixedSpacing()
        {
            float currentY = 0;

            switch (verticalElementsAlignment)
            {
                case VerticalElementsAlignment.Center:
                {
                    float totalHeight = 0;

                    foreach (GameObject layoutElement in layoutElementsGameObjects)
                    {
                        if (useElementMeshHeight)
                        {
                            Renderer rend = layoutElement.GetComponent<Renderer>();

                            if (rend == null)
                            {
                                rend = layoutElement.GetComponentInChildren<Renderer>();

                                if (rend == null)
                                {
                                    Debug.LogError(
                                        $"No renderer found in {layoutElement.name} or its children.Try to set 'useElementMeshHeight' to false and set custom element height");
                                    return;
                                }
                            }

                            totalHeight += rend.bounds.size.y;
                        }
                        else
                        {
                            totalHeight += elementHeight;
                        }
                    }

                    float totalSpacing = spacing * (layoutElementsGameObjects.Count - 1);
                    totalHeight += totalSpacing;


                    currentY = -totalHeight / 2f;
                    break;
                }
                case VerticalElementsAlignment.Lower:
                    currentY = -layoutSize.y / 2;
                    break;
                case VerticalElementsAlignment.Upper:
                    currentY = layoutSize.y / 2;
                    break;
            }


            foreach (GameObject layoutElement in layoutElementsGameObjects)
            {
                float height;

                if (useElementMeshHeight)
                {
                    Renderer rend = layoutElement.GetComponent<Renderer>();

                    if (rend == null)
                    {
                        rend = layoutElement.GetComponentInChildren<Renderer>();

                        if (rend == null)
                        {
                            Debug.LogError(
                                $"No renderer found in {layoutElement.name} or its children.Try to set 'useElementMeshHeight' to false and set custom element height");
                            return;
                        }
                    }

                    height = rend.bounds.size.y;
                }
                else
                {
                    height = elementHeight;
                }


                float elementY = verticalElementsAlignment == VerticalElementsAlignment.Upper
                    ? currentY - height / 2f
                    : currentY + height / 2f;

                Vector3 elementLocalPosition = new Vector3(0, elementY, 0);
                MoveElement(layoutElement, elementLocalPosition);

                switch (verticalElementsAlignment)
                {
                    case VerticalElementsAlignment.Upper:
                    {
                        currentY -= height + spacing;
                        break;
                    }
                    default:
                    {
                        currentY += height + spacing;
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

            float availableHeight = rectTransform.rect.height;
            bool isSingleElement = layoutElementsUI.Count == 1;

            float totalElementsHeight = 0;

            foreach (RectTransform layoutElement in layoutElementsUI)
            {
                totalElementsHeight += layoutElement.rect.height;
            }

            float effectiveSpacing = layoutElementsUI.Count > 1
                ? (availableHeight - totalElementsHeight) / (layoutElementsUI.Count - 1)
                : 0;

            float currentY = verticalElementsAlignment switch
            {
                VerticalElementsAlignment.Center when isSingleElement => 0,
                VerticalElementsAlignment.Center => -availableHeight / 2f,
                VerticalElementsAlignment.Lower => -availableHeight / 2f,
                VerticalElementsAlignment.Upper => availableHeight / 2f,
                _ => -availableHeight / 2f
            };

            foreach (RectTransform layoutElement in layoutElementsUI)
            {
                float rectHeight = layoutElement.rect.height;

                float elementY = verticalElementsAlignment switch
                {
                    VerticalElementsAlignment.Center when isSingleElement => 0,
                    VerticalElementsAlignment.Upper => currentY - rectHeight / 2f,
                    _ => currentY + rectHeight / 2f
                };

                Vector2 elementLocalPosition = new Vector2(0, elementY);
                MoveElement(layoutElement, elementLocalPosition);

                currentY += verticalElementsAlignment == VerticalElementsAlignment.Upper
                    ? -(rectHeight + effectiveSpacing)
                    : rectHeight + effectiveSpacing;
            }
        }


        protected override void GameObjectLayoutWithContainerSize()
        {
            float availableHeight = layoutSize.y;
            bool isSingleElement = layoutElementsGameObjects.Count == 1;

            float totalElementsHeight = 0;

            foreach (GameObject layoutElement in layoutElementsGameObjects)
            {
                if (useElementMeshHeight)
                {
                    Renderer rend = layoutElement.GetComponent<Renderer>();

                    if (rend == null)
                    {
                        rend = layoutElement.GetComponentInChildren<Renderer>();

                        if (rend == null)
                        {
                            Debug.LogError(
                                $"No renderer found in {layoutElement.name} or its children. Try to set 'useElementMeshHeight' to false and set custom element height");
                            return;
                        }
                    }

                    totalElementsHeight += rend.bounds.size.y;
                }
                else
                {
                    totalElementsHeight += elementHeight;
                }
            }

            float effectiveSpacing = layoutElementsGameObjects.Count > 1
                ? (availableHeight - totalElementsHeight) / (layoutElementsGameObjects.Count - 1)
                : 0;

            float currentY = verticalElementsAlignment switch
            {
                VerticalElementsAlignment.Center when isSingleElement => 0,
                VerticalElementsAlignment.Center => -availableHeight / 2f,
                VerticalElementsAlignment.Lower => -availableHeight / 2f,
                VerticalElementsAlignment.Upper => availableHeight / 2f,
                _ => -availableHeight / 2f
            };

            foreach (GameObject layoutElement in layoutElementsGameObjects)
            {
                float height;

                if (useElementMeshHeight)
                {
                    Renderer rend = layoutElement.GetComponent<Renderer>();

                    if (rend == null)
                    {
                        rend = layoutElement.GetComponentInChildren<Renderer>();

                        if (rend == null)
                        {
                            Debug.LogError(
                                $"No renderer found in {layoutElement.name} or its children. Try to set 'useElementMeshHeight' to false and set custom element height");
                            return;
                        }
                    }

                    height = rend.bounds.size.y;
                }
                else
                {
                    height = elementHeight;
                }

                float elementY = verticalElementsAlignment switch
                {
                    VerticalElementsAlignment.Center when isSingleElement => 0,
                    VerticalElementsAlignment.Upper => currentY - height / 2f,
                    _ => currentY + height / 2f
                };

                Vector3 elementLocalPosition = new Vector3(0, elementY, 0);
                MoveElement(layoutElement, elementLocalPosition);

                currentY += verticalElementsAlignment == VerticalElementsAlignment.Upper
                    ? -(height + effectiveSpacing)
                    : height + effectiveSpacing;
            }
        }


        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.green;

            if (!useElementMeshHeight)
            {
                foreach (GameObject layoutElement in layoutElementsGameObjects)
                {
                    Gizmos.DrawWireCube(layoutElement.transform.position, new Vector3(elementHeight, elementHeight, 0));
                }
            }
        }
    }
}
