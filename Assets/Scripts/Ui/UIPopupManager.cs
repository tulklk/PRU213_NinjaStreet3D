using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UIPopupManager : MonoBehaviour
{
    public static UIPopupManager Instance;
    //list : SHOULD BE LOADED from resource
    [SerializeField] private UICanvasElement[] uiResources;


    private Dictionary<System.Type, UICanvasElement> uiCanvasPrefab = new Dictionary<System.Type, UICanvasElement>();
    //dictionary active ui canvas
    private Dictionary<System.Type, UICanvasElement> uiCanvas = new Dictionary<System.Type, UICanvasElement>();

    //canvas container, it should be a canvas - root
    public Transform canvasParent;

    #region Canvas
    void OnEnable()
    {
        Instance = this;
    }
    private void OnDisable()
    {
        Instance = null;
    }
    //open UI
    //mo UI canvas
    public T OpenUI<T>() where T : UICanvasElement
    {
        UICanvasElement canvas = GetUI<T>();

        canvas.Setup();
        canvas.Open();

        return canvas as T;
    }

    //close UI directly

    public void CloseUI<T>() where T : UICanvasElement
    {
        if (IsOpened<T>())
        {
            GetUI<T>().CloseDirectly();
        }
    }

    //close UI with delay time

    public void CloseUI<T>(float delayTime) where T : UICanvasElement
    {
        if (IsOpened<T>())
        {
            GetUI<T>().Close(delayTime);
        }
    }

    //check UI is Opened

    public bool IsOpened<T>() where T : UICanvasElement
    {
        return IsLoaded<T>() && uiCanvas[typeof(T)].gameObject.activeInHierarchy;
    }

    //check UI is loaded

    public bool IsLoaded<T>() where T : UICanvasElement
    {
        System.Type type = typeof(T);
        return uiCanvas.ContainsKey(type) && uiCanvas[type] != null;
    }

    //Get component UI 

    public T GetUI<T>() where T : UICanvasElement
    {
        //Debug.LogError(typeof(T));
        if (!IsLoaded<T>())
        {
            UICanvasElement canvas = Instantiate(GetUIPrefab<T>(), canvasParent);
            uiCanvas[typeof(T)] = canvas;
        }

        return uiCanvas[typeof(T)] as T;
    }

    //Close all UI

    public void CloseAll()
    {
        foreach (var item in uiCanvas)
        {
            if (item.Value != null && item.Value.gameObject.activeInHierarchy)
            {
                item.Value.CloseDirectly();
            }
        }
    }

    //Get prefab from resource

    private T GetUIPrefab<T>() where T : UICanvasElement
    {
        if (!uiCanvasPrefab.ContainsKey(typeof(T)))
        {


            for (int i = 0; i < uiResources.Length; i++)
            {
                if (uiResources[i] is T)
                {
                    uiCanvasPrefab[typeof(T)] = uiResources[i];
                    break;
                }
            }
        }

        return uiCanvasPrefab[typeof(T)] as T;
    }


    private Dictionary<UICanvasElement, UnityAction> BackActionEvents = new Dictionary<UICanvasElement, UnityAction>();
    private List<UICanvasElement> backCanvas = new List<UICanvasElement>();
    UICanvasElement BackTopUI
    {
        get
        {
            UICanvasElement canvas = null;
            if (backCanvas.Count > 0)
            {
                canvas = backCanvas[backCanvas.Count - 1];
            }

            return canvas;
        }
    }


    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape) && BackTopUI != null)
        {
            BackActionEvents[BackTopUI]?.Invoke();
        }
    }

    public void PushBackAction(UICanvasElement canvas, UnityAction action)
    {
        if (!BackActionEvents.ContainsKey(canvas))
        {
            BackActionEvents.Add(canvas, action);
        }
    }

    public void AddBackUI(UICanvasElement canvas)
    {
        if (!backCanvas.Contains(canvas))
        {
            backCanvas.Add(canvas);
        }
    }

    public void RemoveBackUI(UICanvasElement canvas)
    {
        backCanvas.Remove(canvas);
    }

    /// <summary>
    /// CLear backey when comeback index UI canvas
    /// </summary>
    public void ClearBackKey()
    {
        backCanvas.Clear();
    }

    #endregion
}