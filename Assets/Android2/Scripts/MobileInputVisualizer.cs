using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class MobileInputVisualizer : MonoBehaviour
{
    public Canvas canvas;              // ★ Canvas を参照
    public Transform arrow;            // 矢印オブジェクト
    public PlayerController player;    // PlayerController

    private Vector2 startCanvasPos;    // Canvas座標
    private Vector2 currentCanvasPos;
    private bool isDragging = false;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        TouchSimulation.Disable();
    }

    void Update()
    {
        // ★ Player が止まっていない間は絶対に打てない
        if (!player.IsStopped())
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        HandleTouch();   // ★ スマホ
        HandleMouse();   // ★ PC
    }

    // ================================
    // ★ スマホ（タッチ）入力
    // ================================
    void HandleTouch()
    {
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

        if (touches.Count == 0)
            return;

        var touch = touches[0];

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            touch.screenPosition,
            canvas.worldCamera,
            out canvasPos
        );

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            startCanvasPos = canvasPos;
            isDragging = true;
        }
        else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && isDragging)
        {
            currentCanvasPos = canvasPos;
            UpdateArrow();
        }
        else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            EndDrag();
        }
    }

    // ================================
    // ★ PC（マウス）入力
    // ================================
    void HandleMouse()
    {
        if (Mouse.current == null) return;

        // 押した瞬間
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                Mouse.current.position.ReadValue(),
                canvas.worldCamera,
                out canvasPos
            );

            startCanvasPos = canvasPos;
            isDragging = true;
        }

        // ドラッグ中
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                Mouse.current.position.ReadValue(),
                canvas.worldCamera,
                out canvasPos
            );

            currentCanvasPos = canvasPos;
            UpdateArrow();
        }

        // 離した瞬間
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            EndDrag();
        }
    }

    // ================================
    // ★ ドラッグ終了処理（共通）
    // ================================
    void EndDrag()
    {
        isDragging = false;
        arrow.gameObject.SetActive(false);

        Vector3 startWorld = CanvasToWorld(startCanvasPos);
        Vector3 endWorld = CanvasToWorld(currentCanvasPos);

        Vector2 dir = (startWorld - endWorld).normalized;
        player.Shoot(dir);
    }

    // ================================
    // ★ Canvas座標 → ワールド座標
    // ================================
    Vector3 CanvasToWorld(Vector2 canvasPos)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            canvasPos,
            canvas.worldCamera,
            out worldPos
        );
        return worldPos;
    }

    // ================================
    // ★ 矢印描画（Canvas基準）
    // ================================
    void UpdateArrow()
    {
        if (arrow == null) return;

        Vector3 startWorld = CanvasToWorld(startCanvasPos);
        Vector3 endWorld = CanvasToWorld(currentCanvasPos);

        Vector2 dir = (startWorld - endWorld).normalized;

        // ① 矢印の位置を Player に固定
        arrow.position = player.transform.position;

        // ② 方向に回転
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle);

        // ③ 引っ張り距離で長さ調整
        float distance = Vector2.Distance(startCanvasPos, currentCanvasPos);
        arrow.localScale = new Vector3(distance * 0.015f, 1, 1);

        arrow.gameObject.SetActive(true);
    }
}
