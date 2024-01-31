using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Template.Interaction
{
    public interface IPointerInteractable : IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        bool Interactable { get; set; }
    }
}