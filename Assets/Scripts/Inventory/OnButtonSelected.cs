using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonSelected : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler
{
    private void Awake()
    {
        DOTween.Init();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        transform.DOScale(1.075f, 0.075f).SetEase(Ease.InOutQuad);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;
    }
}
