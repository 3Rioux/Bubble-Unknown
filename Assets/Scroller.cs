using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Misc
{
    public class Scroller : MonoBehaviour
    {
        [SerializeField] private RawImage _shadow, _img;
        [SerializeField] private float _x, _y;

        private void Update()
        {
            _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
            _shadow.uvRect = _img.uvRect;
        }
    }
}