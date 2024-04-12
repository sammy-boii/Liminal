using UnityEngine;
using UnityEngine.UI;

namespace OldElevator {
    public class InteractionIcon : MonoBehaviour {
        public static InteractionIcon inst; //global access
        private Image thisIcon;
        [SerializeField] private Color noInteractableColor = new Color(1, 1, 1, .1f);
        [SerializeField] private Color interactableColor = Color.green;

        void Awake() {
            inst = this;
            thisIcon = GetComponent<Image>();
        }


        public void SetInteractable(bool value) {
            thisIcon.color = value ? interactableColor : noInteractableColor;
        }

    }
}
