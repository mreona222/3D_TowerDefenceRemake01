using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.Interaction
{
    public interface IInteractable : IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        bool Interactable { get; set; }
    }
}