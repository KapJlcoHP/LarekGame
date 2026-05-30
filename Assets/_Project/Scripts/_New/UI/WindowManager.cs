using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{
    [Header("Ќастройки")]
    [SerializeField] private UIWindow[] registeredWindows;   // зарегистрированные окна (можно заполнить в инспекторе)
    [SerializeField] private CanvasGroup backgroundBlocker;  // опциональна€ панель дл€ блокировки ввода под модальными окнами

    private Stack<UIWindow> windowStack = new Stack<UIWindow>();
    private HashSet<UIWindow> registeredSet;

    private void Awake()
    {
        // —обираем окна, если не назначены в инспекторе (дочерние)
        if (registeredWindows == null || registeredWindows.Length == 0)
            registeredWindows = GetComponentsInChildren<UIWindow>(true);

        registeredSet = new HashSet<UIWindow>(registeredWindows);
    }

    // –егистраци€ окна (можно вызывать вручную)
    public void RegisterWindow(UIWindow window)
    {
        if (!registeredSet.Contains(window))
        {
            registeredSet.Add(window);
            // обновить массив, если нужно
        }
    }

    // ќткрыть конкретное окно (по ссылке)
    public void OpenWindow(UIWindow window)
    {
        if (window == null || !registeredSet.Contains(window))
        {
            Debug.LogWarning($"ќкно {window?.name} не зарегистрировано");
            return;
        }

        // ≈сли окно уже открыто и находитс€ на вершине стека Ч ничего не делаем (или можно подсветить)
        if (windowStack.Count > 0 && windowStack.Peek() == window)
            return;

        // ≈сли окно уже где-то в стеке (но не на вершине) Ч удал€ем его оттуда
        if (windowStack.Contains(window))
        {
            RemoveFromStack(window);
        }

        // ѕеред открытием: если новое окно модальное, блокируем фон
        if (window.IsPopup)
        {
            SetBackgroundBlocker(true);
        }

        window.Show();
        windowStack.Push(window);

        // ≈сли окно не модальное, скрываем предыдущее немодальное? ќбычно при открытии нового главного окна старое закрываетс€.
        // ћожно добавить политику: если window.IsPopup == false, то закрываем все остальные немодальные окна под ним.
        // ѕока оставим простое наложение.
    }

    // «акрыть конкретное окно
    public void CloseWindow(UIWindow window)
    {
        if (window == null || !windowStack.Contains(window))
            return;

        window.Hide();
        RemoveFromStack(window);

        // ≈сли после закрыти€ верхнее окно Ч модальное, оставл€ем блокировку, иначе снимаем
        if (windowStack.Count == 0 || !windowStack.Peek().IsPopup)
            SetBackgroundBlocker(false);
    }

    // «акрыть верхнее окно (по Esc)
    public void CloseCurrentWindow()
    {
        if (windowStack.Count == 0)
            return;

        UIWindow top = windowStack.Peek();
        if (top.CloseOnEscape)
            CloseWindow(top);
    }

    // «акрыть все окна
    public void CloseAll()
    {
        while (windowStack.Count > 0)
        {
            var win = windowStack.Pop();
            win.Hide();
        }
        SetBackgroundBlocker(false);
    }

    // ѕроверить, открыто ли окно
    public bool IsWindowOpen(UIWindow window) => windowStack.Contains(window);

    // ¬ернуть верхнее окно (может пригодитьс€)
    public UIWindow TopWindow => windowStack.Count > 0 ? windowStack.Peek() : null;

    private void RemoveFromStack(UIWindow window)
    {
        // ѕересоздаЄм стек без этого окна
        var temp = new Stack<UIWindow>();
        while (windowStack.Count > 0)
        {
            var w = windowStack.Pop();
            if (w != window)
                temp.Push(w);
        }
        while (temp.Count > 0)
            windowStack.Push(temp.Pop());
    }

    private void SetBackgroundBlocker(bool active)
    {
        if (backgroundBlocker != null)
        {
            backgroundBlocker.blocksRaycasts = active;
            backgroundBlocker.alpha = active ? 0.7f : 0f; // полупрозрачный
            backgroundBlocker.gameObject.SetActive(active);
        }
    }

    private void Update()
    {
        // ќбработка Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCurrentWindow();
        }
    }

    // »нициализаци€ (вызываетс€ из UIRoot после передачи менеджеров)
    public void Initialize(UIWindow[] windows = null)
    {
        if (windows != null)
            registeredWindows = windows;
        registeredSet = new HashSet<UIWindow>(registeredWindows);
        // —крываем все окна изначально
        foreach (var win in registeredWindows)
            win.gameObject.SetActive(false);
    }
}