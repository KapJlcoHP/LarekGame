using UnityEngine;
using UnityEngine.Events;

public abstract class UIWindow : MonoBehaviour
{
    [Header("Окно")]
    [SerializeField] private bool isPopup = false;         // блокирует ли ввод под собой
    [SerializeField] private bool closeOnEscape = true;    // закрывается ли по Esc

    [Header("События (для анимаций/отладки)")]
    public UnityEvent OnWindowShown;
    public UnityEvent OnWindowHidden;

    public bool IsPopup => isPopup;
    public bool CloseOnEscape => closeOnEscape;
    public bool IsVisible { get; private set; }

   protected WindowManager windowManager;

    // Вызывается один раз при инициализации UIRoot'ом
    public virtual void Init(WindowManager wm)
    {
        windowManager = wm;
    }

    // Показать окно (по умолчанию просто включает GameObject)
    public virtual void Show()
    {
        gameObject.SetActive(true);
        IsVisible = true;
        OnWindowShown?.Invoke();
    }

    // Скрыть окно
    public virtual void Hide()
    {
        gameObject.SetActive(false);
        IsVisible = false;
        OnWindowHidden?.Invoke();
    }

    // Переключить видимость
    public void Toggle()
    {
        if (IsVisible) Hide();
        else Show();
    }

    // В наследниках можно добавить подписки на события в OnEnable/OnDisable
    protected virtual void OnDestroy()
    {
        // Переопределяется при необходимости
    }
}