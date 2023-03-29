using DG.Tweening;
using UnityEngine;

namespace Mini_UI.Script
{
    public class CloseButton : MonoBehaviour
    {
        public GameObject Panel;

        public void Close()
        {
            Panel.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack).OnComplete(Disable);
            //this.gameObject.SetActive(false);
        }

        private void Disable()
        {
            Panel.SetActive(false);
        }
    }
}
