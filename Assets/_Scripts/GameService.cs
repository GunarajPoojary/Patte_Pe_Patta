using Patte_pe_patta.Utility;
using UnityEngine;

namespace Patte_pe_patta
{
    public class GameService : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bgRenderer;
        [SerializeField] private SpriteRenderer _tableRenderer;

        private void Awake()
        {
            ResponsiveSpriteRenderer.FitToSafeAreaHeight(_tableRenderer);
            ResponsiveSpriteRenderer.FitToFullScreen(_bgRenderer);
        }
    }
}