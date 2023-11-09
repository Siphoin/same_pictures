using System;
using System.Collections.Generic;
using UnityEngine;

namespace SamePictures.Repositories
{
    public class RepositoryDb : Initializiable
    {
        private Dictionary<Type, IRepository> _repositories;
        public RepositoryDb()
        {
            _repositories = new Dictionary<Type, IRepository>();
        }
        public void Initialize()
        {
            IRepository[] repositories =
            {
                new BackgroundRepository(nameof(Background)),
                new PictureRepository(nameof(Picture) + nameof(Sprite)),
            };

            foreach (var repository in repositories)
            {
                _repositories.Add(repository.GetType(), repository);

                repository.Initialize();
            }
        }

        public T Get<T>() where T : IRepository
        {
            return (T)_repositories[typeof(T)];
        }
    }
}
