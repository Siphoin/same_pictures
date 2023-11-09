using Cysharp.Threading.Tasks;
using SamePictures.Repositories;
using SamePictures.Services;
using UnityEngine;
using UnityEngine.UI;

namespace SamePictures
{
    public static class Startup
	{
        private static RepositoryDb _dbRepository;

        private static ServiceLocator _serviceLocator;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Main()
        {
            _dbRepository = new RepositoryDb();

            _dbRepository.Initialize();

            _serviceLocator = new ServiceLocator();

            _serviceLocator.Initialize();

        }

        public static async UniTask StartGame (GridLayoutGroup grid)
        {
            var levelService = GetService<LevelService>();

            levelService.SetGrid(grid);

            await levelService.InitializeCards();

            levelService.NewLevel();
        }

        public static T GetRepository<T>() where T : IRepository
        {
            return _dbRepository.Get<T>();
        }

        public static T GetService<T>() where T : IService
        {
            return _serviceLocator.Get<T>();
        }
    }
}