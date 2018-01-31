namespace Autrage.LEX.NET
{
    public static class Singleton<T> where T : class, new()
    {
        #region Fields

        private static T instance = null;

        #endregion Fields

        #region Properties

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }

                return instance;
            }
        }

        #endregion Properties
    }
}