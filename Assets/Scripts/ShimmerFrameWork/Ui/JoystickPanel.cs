using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ShimmerFramework
{
    public enum JoysticType
    {
        Normal,
        CanChangePos,
        CanMove,
    }

    public class JoystickPanel : BasePanel
    {
        public JoysticType joysticType = JoysticType.Normal;
        private Image joyTouchRect;
        private Image joyBk;
        private Image joyControl;

        private Vector3 joyPos;
        public override void Start()
        {
            base.Start();
            joyTouchRect = GetUiController<Image>("JoyTouchRect");
            joyBk = GetUiController<Image>("JoyBk");
            joyControl = GetUiController<Image>("JoyControl");

            joyPos = transform.Find("JoyTouchRect/JoyBk").position;

            UiManager.GetInstance().AddCustomEventTrigger(joyTouchRect, EventTriggerType.PointerDown, ClickDown);
            UiManager.GetInstance().AddCustomEventTrigger(joyTouchRect, EventTriggerType.PointerUp, ClickUp);

            UiManager.GetInstance().AddCustomEventTrigger(joyTouchRect, EventTriggerType.Drag, ClickDrag);


            if (joysticType != JoysticType.Normal)
            {
                joyBk.gameObject.SetActive(false);
            }
        }


        public void ClickDown(BaseEventData eventDate)
        {
            joyBk.gameObject.SetActive(true);

            if (joysticType != JoysticType.Normal)
            {
                Vector2 localPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, (eventDate as PointerEventData).position,
                    (eventDate as PointerEventData).pressEventCamera, out localPos);

                joyBk.transform.localPosition = localPos;

            }
        }

        public void ClickUp(BaseEventData eventDate)
        {
            joyControl.transform.localPosition = Vector3.zero;

            if (joysticType != JoysticType.Normal)
            {
                joyBk.gameObject.SetActive(false);
            }

            if (joysticType == JoysticType.CanMove)
                joyBk.rectTransform.localPosition = joyPos;


            EventManager.GetInstance().ActionTrigger("Joysick", Vector2.zero);
        }

        public void ClickDrag(BaseEventData eventDate)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joyBk.rectTransform, (eventDate as PointerEventData).position,
                (eventDate as PointerEventData).pressEventCamera, out localPos);

            joyControl.transform.localPosition = localPos;

            if (localPos.magnitude > 40)
            {
                if (joysticType == JoysticType.CanMove)
                    joyBk.transform.localPosition += (Vector3)(localPos.normalized * (localPos.magnitude - 40));

                joyControl.transform.localPosition = localPos.normalized * 40;
            }

            EventManager.GetInstance().ActionTrigger("Joysick", localPos.normalized);

        }
    }
}