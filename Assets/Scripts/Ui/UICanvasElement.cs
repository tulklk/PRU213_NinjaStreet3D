
using UnityEngine;
public class UICanvasElement : MonoBehaviour
{
    //public bool IsAvoidBackKey = false;
    public bool IsDestroyOnClose = false;
    public bool IsHandlingRabbitEars = false;
    public bool IsWidescreenProcessing = false;

    protected RectTransform m_RectTransform;
    private Animator m_Animator;
    private float m_OffsetY = 0;

    private void Start()
    {
        OnInit();
    }

    //Init default Canvas

    protected void OnInit()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Animator = GetComponent<Animator>();

        //RABBIT EARS HANDLE
        float ratio = (float)Screen.height / (float)Screen.width;
        if (IsHandlingRabbitEars)
        {
            if (ratio > 2.1f)
            {
                Vector2 leftBottom = m_RectTransform.offsetMin;
                Vector2 rightTop = m_RectTransform.offsetMax;
                rightTop.y = -100f;
                m_RectTransform.offsetMax = rightTop;
                leftBottom.y = 0f;
                m_RectTransform.offsetMin = leftBottom;
                m_OffsetY = 100f;
            }
        }


        if (IsWidescreenProcessing)
        {
            ratio = (float)Screen.width / (float)Screen.height;
            if (ratio < 2.1f)
            {
                //STANDARD size
                float ratioDefault = 850 / 1920f;
                float ratioThis = ratio;

                float value = 1 - (ratioThis - ratioDefault);

                float with = m_RectTransform.rect.width * value;

                m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, with);
            }

        }

        //Set parent popup child

        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].ParentsPopup = this;
        }
    }

    //Setup canvas to avoid flash UI

    public virtual void Setup()
    {
        UIPopupManager.Instance.AddBackUI(this);
        UIPopupManager.Instance.PushBackAction(this, BackKey);
    }


    //back key in android device

    public virtual void BackKey()
    {

    }

    //Open canvas

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    //close canvas directly

    public virtual void CloseDirectly()
    {
        UIPopupManager.Instance.RemoveBackUI(this);
        gameObject.SetActive(false);
        if (IsDestroyOnClose)
        {
            Destroy(gameObject);
        }

    }

    //close canvas with delay time, used to anim UI action

    public virtual void Close(float delayTime)
    {
        Debug.Log("Close");
        Invoke(nameof(CloseDirectly), delayTime);
    }



    [Header("Popup Child")]
    [SerializeField] UICanvasElement[] popups;
    public UICanvasElement ParentsPopup { get; set; }

    public T GetPopup<T>() where T : UICanvasElement
    {
        T ui = null;
        for (int i = 0; i < popups.Length; i++)
        {
            if (popups[i] is T)
            {
                ui = popups[i] as T;
                break;
            }
        }

        return ui;
    }

    public T OpenPopup<T>() where T : UICanvasElement
    {
        T ui = GetPopup<T>();
        ui.Setup();
        ui.Open();
        return ui;
    }

    public bool IsOpenedPopup<T>() where T : UICanvasElement
    {
        return GetPopup<T>().gameObject.activeSelf;
    }


    public void ClosePopup<T>(float delayTime) where T : UICanvasElement
    {
        GetPopup<T>().Close(delayTime);
    }

    public void ClosePopupDirect<T>() where T : UICanvasElement
    {
        GetPopup<T>().CloseDirectly();
    }

    public void CloseAllPopup()
    {
        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].CloseDirectly();
        }
    }


}