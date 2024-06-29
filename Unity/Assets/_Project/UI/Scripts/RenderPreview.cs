using _Project.Ray_Tracer.Scripts;
using _Project.Ray_Tracer.Scripts.Utility;
using _Project.Scripts;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace _Project.UI.Scripts
{
    /// <summary>
    /// A UI class that displays an <see cref="RTImage"/>.
    /// </summary>
    public class RenderPreview : MonoBehaviour
    {
        private RTImage rayTracerImage;
        private RayManager rayManager;

        private int imageWidth;
        private int imageHeight;

        [SerializeField]
        private Image uiImage;

        [SerializeField]
        private RectTransform imageBounds; // The UI image is constrained to these bounds.
        [SerializeField]
        private Image hoverImage;
        [SerializeField]
        private Image selectImage;

        [SerializeField]
        private Button expandCollapseButton;
        [SerializeField]
        private Image expandCollapseImage;

        [SerializeField]
        private Sprite expandedIcon;
        [SerializeField]
        private Sprite collapsedIcon;

        [Serializable]
        public class Event : UnityEvent { };
        public Event OnPixelSelected, OnPixelDeselected;

        private RectTransform windowSize;
        private bool expanded = false;

        private float pixelsPerUnit;
        private Vector2Int hoveredPixel;
        private Vector2Int selectedPixel;

        public NearFarInteractor nearFarInteractor { get; set; }

        public bool PreviewWindowHovered { get; set; }
        public bool ImageHovered { get; set; }

        private void UpdatePreview()
        {
            // We expand the image until we hit the bounds in either the width or height.
            float pixelsPerUnitInWidth = imageBounds.rect.width / rayTracerImage.Width;
            float pixelsPerUnitInHeight = imageBounds.rect.height / rayTracerImage.Height;
            pixelsPerUnit = Mathf.Min(pixelsPerUnitInWidth, pixelsPerUnitInHeight);

            // Destroy the old sprite to prevent a memory leak.
            if (uiImage.sprite != null)
                Destroy(uiImage.sprite);

            uiImage.sprite = rayTracerImage.GetSprite(pixelsPerUnit);

            // Make sure the image UI element is scaled correctly.
            uiImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                rayTracerImage.Width * pixelsPerUnit);
            uiImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                rayTracerImage.Height * pixelsPerUnit);

            // The hover and select images are the size of one pixel.
            hoverImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pixelsPerUnit);
            hoverImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pixelsPerUnit);
            selectImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pixelsPerUnit);
            selectImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pixelsPerUnit);
        }

        private void CheckDimensionsChanged()
        {
            // If the image dimensions have changed we need to invalidate our selected pixel as it may no longer exist.
            if (rayTracerImage.Width != imageWidth || rayTracerImage.Height != imageHeight)
            {
                selectImage.enabled = false;
                rayManager.DeselectRay();
            }

            imageWidth = rayTracerImage.Width;
            imageHeight = rayTracerImage.Height;
        }

        private void ExpandCollapse()
        {
            if (expanded)
            {
                expandCollapseImage.sprite = collapsedIcon;
                windowSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, windowSize.rect.width / 3);
                windowSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, windowSize.rect.height / 3);
                UIManager.Get().RemoveEscapable(ExpandCollapse);
            }
            else
            {
                expandCollapseImage.sprite = expandedIcon;
                windowSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, windowSize.rect.width * 3);
                windowSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, windowSize.rect.height * 3);
                UIManager.Get().AddEscapable(ExpandCollapse);
            }

            UpdatePreview();
            expanded = !expanded;

            // Recalculate the position of the selection indicator after expanding/collapsing the preview.
            if (selectImage.enabled)
            {
                float pixelCenterX = pixelsPerUnit * selectedPixel.x + pixelsPerUnit / 2.0f;
                float pixelCenterY = pixelsPerUnit * selectedPixel.y + pixelsPerUnit / 2.0f;
                float transformX = pixelCenterX - uiImage.rectTransform.rect.width / 2.0f;
                float transformY = pixelCenterY - uiImage.rectTransform.rect.height / 2.0f;
                selectImage.rectTransform.anchoredPosition = new Vector2(transformX, transformY);
            }
        }

        private void OnDisable()
        {
            if (expanded)
                ExpandCollapse();
        }

        private void Awake()
        {
            expandCollapseButton.onClick.AddListener(ExpandCollapse);
        }

        private void Start()
        {
            rayTracerImage = RTSceneManager.Get().Image;
            rayTracerImage.OnImageChanged += UpdatePreview;
            rayTracerImage.OnImageChanged += CheckDimensionsChanged;

            rayManager = RayManager.Get();

            windowSize = GetComponent<RectTransform>();

            hoverImage.enabled = false;
            selectImage.enabled = false;

            XRUIControlManager xrUIControlManager = transform.root.gameObject.GetComponent<XRUIControlManager>();
            var leftActivation = XRUIControlManager.GetInputAction(xrUIControlManager.leftActivateReference);
            if (leftActivation != null)
                leftActivation.started += _ => toggleHoveredPixel();

            var rightActivation = XRUIControlManager.GetInputAction(xrUIControlManager.rightActivateReference);
            if (rightActivation != null)
                rightActivation.started += _ => toggleHoveredPixel();

            UpdatePreview();
        }

        public void uiHoverEntered(UIHoverEventArgs args)
        {
            if (args.uiObject.transform.gameObject.name.Contains("Image"))
                ImageHovered = true;
        }

        public void uiHoverExited(UIHoverEventArgs args)
        {
            ImageHovered = false;
            hoverImage.enabled = false;
        }

        private void toggleHoveredPixel()
        {
            if (!ImageHovered)
                return;

            // If the hovered pixel is already selected we deselect it.
            if (hoveredPixel == selectedPixel && selectImage.enabled)
            {
                selectImage.enabled = false;
                rayManager.DeselectRay();
                OnPixelDeselected?.Invoke();
            }
            // Select the hovered pixel.
            else
            {
                selectedPixel = hoveredPixel;
                selectImage.rectTransform.anchoredPosition = hoverImage.rectTransform.anchoredPosition;
                selectImage.enabled = true;
                rayManager.SelectRay(selectedPixel);
                OnPixelSelected?.Invoke();
            }
        }

        private void Update()
        {
            //if (!PreviewWindowHovered && Input.GetMouseButtonDown(0) && expanded) ExpandCollapse();

            if (!ImageHovered)
            {
                hoverImage.enabled = false;
                return;
            }
            if (nearFarInteractor == null)
                return;

            if (!nearFarInteractor.TryGetCurrentUIRaycastResult(out RaycastResult hit))
                return;

            // Get the interactor position with respect to the UI image's transform.
            Vector3 localHitPos = uiImage.rectTransform.InverseTransformPoint(hit.worldPosition);

            // The UI image is anchored in the center, but we want the coordinates anchored bottom left.
            localHitPos.x += uiImage.rectTransform.rect.width / 2.0f;
            localHitPos.y += uiImage.rectTransform.rect.height / 2.0f;

            // Determine the pixel that we are hovering over.
            int xCoordinate = Mathf.FloorToInt(localHitPos.x / pixelsPerUnit);
            int yCoordinate = Mathf.FloorToInt(localHitPos.y / pixelsPerUnit);
            hoveredPixel = new Vector2Int(xCoordinate, yCoordinate);

            // Snap the interactor position to the pixel center.
            float pixelCenterX = pixelsPerUnit * xCoordinate + pixelsPerUnit / 2.0f;
            float pixelCenterY = pixelsPerUnit * yCoordinate + pixelsPerUnit / 2.0f;
            Vector2 interactorSnapped = new Vector2(pixelCenterX, pixelCenterY);

            // The hover image is anchored in the center, so we convert back.
            interactorSnapped.x -= uiImage.rectTransform.rect.width / 2.0f;
            interactorSnapped.y -= uiImage.rectTransform.rect.height / 2.0f;
            hoverImage.rectTransform.anchoredPosition = interactorSnapped;

            hoverImage.enabled = true;
        }
    }
}
