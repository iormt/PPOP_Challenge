using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGeneration
{
    public class TileHighlighter : MonoBehaviour
    {
        [SerializeField] private Color Normal;
        [SerializeField] private Color PathHighlighted;
        [SerializeField] private Color EndsHighlighted;

        [SerializeField] private Vector3 NormalScale;
        [SerializeField] private Vector3 HighlightedScale;

        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;

        public void Init()
        {
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
        }

        public void HighlightTile(bool isEnd)
        {
            // Get the current value of the material properties in the renderer.
            _renderer.GetPropertyBlock(_propBlock);
            // Assign our new value.
            if (isEnd)
            {
                _propBlock.SetColor("_Color", EndsHighlighted);
            }
            else
            {
                _propBlock.SetColor("_Color", PathHighlighted);
            }
            //_propBlock.SetColor("_Color", Color.Lerp(Normal, PathHighlighted, (Mathf.Sin(Time.time * Speed + Offset) + 1) / 2f));
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
            transform.localScale = HighlightedScale;
        }
        public void DehighlightTile()
        {
            // Get the current value of the material properties in the renderer.
            _renderer.GetPropertyBlock(_propBlock);
            // Assign our new value.
            _propBlock.SetColor("_Color", Normal);
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
            transform.localScale = NormalScale;
        }
    }
}