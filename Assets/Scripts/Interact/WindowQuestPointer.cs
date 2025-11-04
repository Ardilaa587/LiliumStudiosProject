using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowQuestPointer : MonoBehaviour
{
    private Camera uiCamera = null;

    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite crossSprite;

    private Image pointerImage;
    private Transform targetTransform;
    private RectTransform pointerRectTransform;
    private RectTransform parentRectTransform;

    private void Awake()
    {
        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        pointerImage = transform.Find("Pointer").GetComponent<Image>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();

        Hide();
    }

    private void Update()
    {
        if (targetTransform == null)
        {
            Hide();
            return;
        }

        Vector3 targetPosition = targetTransform.position;

        float borderSize = 100f;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);

        bool isOffScreen = targetPositionScreenPoint.x <= borderSize ||
                           targetPositionScreenPoint.x >= Screen.width - borderSize ||
                           targetPositionScreenPoint.y <= borderSize ||
                           targetPositionScreenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            RotatePointerTowardsTargetPosition();
            pointerImage.sprite = arrowSprite;

            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;

            cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, borderSize, Screen.width - borderSize);
            cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, borderSize, Screen.height - borderSize);

            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,
                cappedTargetScreenPosition,
                uiCamera,
                out localPointerPosition
            );
            pointerRectTransform.localPosition = localPointerPosition;
        }
        else
        {
            pointerImage.sprite = crossSprite;

            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,
                targetPositionScreenPoint,
                uiCamera,
                out localPointerPosition
            );
            pointerRectTransform.localPosition = localPointerPosition;

            pointerRectTransform.localEulerAngles = Vector3.zero;
        }
    }

    private void RotatePointerTowardsTargetPosition()
    {
        if (targetTransform == null) return;

        Vector3 fromPosition = Camera.main.transform.position;
        Vector3 toPosition = targetTransform.position;

        Vector2 dir = new Vector2(toPosition.x, toPosition.y) - new Vector2(fromPosition.x, fromPosition.y);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle -= 90f;

        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        this.targetTransform = null;
    }

    public void SetTarget(Transform targetTransform)
    {
        gameObject.SetActive(true);
        this.targetTransform = targetTransform;
    }
}
