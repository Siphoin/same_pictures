using UnityEngine;
using UnityEngine.UI;

namespace SamePictures
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _grid;

        private async void Start()
        {
            await Startup.StartGame(_grid);

            Destroy(gameObject);
        }
    }
}