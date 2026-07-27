using UnityEngine;

namespace General
{
    /// <summary>
    /// Lets the player drag a unit card/token from its starting position and drop it
    /// onto a valid path zone to place and activate the unit.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Clickable : MonoBehaviour
    {
        [Header("Drag Settings")]
        [Tooltip("Only colliders on this layer are considered valid drop targets.")]
        [SerializeField] private LayerMask pathZoneLayer;

        private Camera _mainCamera;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _originalPosition;
        private int _originalSortingOrder;
        private bool _isDragging;

        /// <summary>True once this unit has been successfully placed on a path.</summary>
        public bool IsPlaced { get; private set; }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnMouseDown()
        {
            if (IsPlaced)
                return;

            _isDragging = true;
            _originalPosition = transform.position;

            if (_spriteRenderer != null)
            {
                _originalSortingOrder = _spriteRenderer.sortingOrder;
                _spriteRenderer.sortingOrder += 100;
            }
        }

        private void OnMouseDrag()
        {
            if (!_isDragging)
                return;

            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            transform.position = mouseWorldPos;
        }

        private void OnMouseUp()
        {
            if (!_isDragging)
                return;

            _isDragging = false;

            if (_spriteRenderer != null)
                _spriteRenderer.sortingOrder = _originalSortingOrder;

            TryDropOnPath();
        }

        private void TryDropOnPath()
        {
            Collider2D hit = Physics2D.OverlapPoint(transform.position, pathZoneLayer);
            if (hit != null)
            {
                transform.position = hit.transform.position;
                PlaceOnPath();
            }
            else
            {
                transform.position = _originalPosition;
            }
        }

        private void PlaceOnPath()
        {
            IsPlaced = true;

            if (TryGetComponent(out Unit mover))
                mover.Activate();
        }
    }
}
